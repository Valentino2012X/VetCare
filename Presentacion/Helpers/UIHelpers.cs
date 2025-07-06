using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Presentacion.Helpers
{
    public static class UIHelpers
    {

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static void RedondearBordes(Control control, int radio)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, radio, radio, 180, 90);
                path.AddArc(control.Width - radio, 0, radio, radio, 270, 90);
                path.AddArc(control.Width - radio, control.Height - radio, radio, radio, 0, 90);
                path.AddArc(0, control.Height - radio, radio, radio, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }
        public static void EstilizarComboBox(ComboBox cb, Color colorBorde, Color colorHover)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.BackColor = Color.White;

            Panel underline = new Panel
            {
                Height = 2,
                Dock = DockStyle.Bottom,
                BackColor = colorBorde
            };
            cb.Controls.Add(underline);

            cb.Enter += (s, e) => underline.BackColor = colorHover;
            cb.Leave += (s, e) => underline.BackColor = colorBorde;
        }

        public static void HabilitarArrastre(Form form)
        {
            form.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(form.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
        }


        public static void EstilizarBoton(Button btn, Color colorPrincipal, Color? colorHover = null)
        {
            Color colorBase = Color.White;
            Color hoverColor = colorHover ?? colorPrincipal;

            // Configuración inicial del botón
            btn.BackColor = colorBase;
            btn.ForeColor = colorPrincipal;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = colorPrincipal;
            btn.Cursor = Cursors.Hand;
            btn.Font = AppStyles.Fuentes.Normal;

            // Guardar colores en la propiedad Tag
            btn.Tag = new
            {
                Normal = new { Back = colorBase, Fore = colorPrincipal, Border = colorPrincipal },
                Hover = new { Back = hoverColor, Fore = colorBase, Border = colorBase }
            };

            // Manejador MouseEnter seguro
            btn.MouseEnter += (s, e) =>
            {
                if (s is Button b && !b.IsDisposed)
                {
                    var state = (dynamic)b.Tag;
                    b.BackColor = state.Hover.Back;
                    b.ForeColor = state.Hover.Fore;
                    b.FlatAppearance.BorderColor = state.Hover.Border;
                }
            };

            // Manejador MouseLeave seguro
            btn.MouseLeave += (s, e) =>
            {
                if (s is Button b && !b.IsDisposed)
                {
                    var state = (dynamic)b.Tag;
                    b.BackColor = state.Normal.Back;
                    b.ForeColor = state.Normal.Fore;
                    b.FlatAppearance.BorderColor = state.Normal.Border;
                }
            };

            // Manejador MouseUp seguro
            btn.MouseUp += (s, e) =>
            {
                if (s is Button b && !b.IsDisposed && b.IsHandleCreated)
                {
                    try
                    {
                        var state = (dynamic)b.Tag;
                        var cursorPos = b.PointToClient(Cursor.Position);

                        if (!b.ClientRectangle.Contains(cursorPos))
                        {
                            b.BackColor = state.Normal.Back;
                            b.ForeColor = state.Normal.Fore;
                            b.FlatAppearance.BorderColor = state.Normal.Border;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Silenciar excepción si el control fue eliminado
                    }
                }
            };
        }

        public static void EstilizarTextBox(TextBox txt, Color colorSubrayado, Color colorHover)
        {
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = Color.White;

            Panel underline = new Panel
            {
                Height = 2,
                Dock = DockStyle.Bottom,
                BackColor = colorSubrayado
            };
            txt.Controls.Add(underline);

            txt.Enter += (s, e) => underline.BackColor = colorHover;
            txt.Leave += (s, e) => underline.BackColor = colorSubrayado;
        }

        public static void ConfigurarAnimacionLogo(PictureBox pb, Color colorBorde, int grosorBorde = 4)
        {
            pb.SizeMode = PictureBoxSizeMode.Zoom;

            Point posicionOriginal = pb.Location;
            Size tamañoOriginal = pb.Size;
            int tamañoMaximo = tamañoOriginal.Width + 15;
            int pasoAnimacion = 2;
            bool agrandando = true;

            Timer timer = new Timer { Interval = 15, Enabled = false };

            timer.Tick += (sender, e) =>
            {
                int nuevoAncho = pb.Width + (agrandando ? pasoAnimacion : -pasoAnimacion);

                if (nuevoAncho >= tamañoMaximo)
                {
                    nuevoAncho = tamañoMaximo;
                    agrandando = false;
                }
                else if (nuevoAncho <= tamañoOriginal.Width)
                {
                    nuevoAncho = tamañoOriginal.Width;
                    agrandando = true;
                }

                int offsetX = (tamañoOriginal.Width - nuevoAncho) / 2;
                pb.Size = new Size(nuevoAncho, nuevoAncho);
                pb.Location = new Point(
                    posicionOriginal.X + offsetX,
                    posicionOriginal.Y + offsetX
                );

                HacerCircularConBorde(pb, colorBorde, grosorBorde);
            };

            pb.MouseEnter += (s, e) => timer.Start();
            pb.MouseLeave += (s, e) =>
            {
                timer.Stop();

                pb.Size = tamañoOriginal;
                pb.Location = posicionOriginal;
                HacerCircularConBorde(pb, colorBorde, grosorBorde);
            };

            // Configuración inicial
            HacerCircularConBorde(pb, colorBorde, grosorBorde);
        }

        private static void HacerCircularConBorde(PictureBox pb, Color colorBorde, int grosorBorde)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pb.Width, pb.Height);
            pb.Region = new Region(gp);

            pb.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(
                    new Pen(colorBorde, grosorBorde),
                    new Rectangle(
                        grosorBorde / 2,
                        grosorBorde / 2,
                        pb.Width - grosorBorde,
                        pb.Height - grosorBorde
                    )
                );
            };
        }
    }
}