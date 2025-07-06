using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Datos;
using Negocio;
using Presentacion.Helpers;


namespace Presentacion
{
    public partial class FormInicio : Form
    {
        public FormInicio()
        {
            InitializeComponent();
            ConfigurarFormulario();
            EstilizarBotones();
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);
        }

        private void ConfigurarFormulario()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.BackColor = AppStyles.Colores.Fondo; 

            UIHelpers.RedondearBordes(this, 40);
            UIHelpers.HabilitarArrastre(this); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen borde = new Pen(AppStyles.Colores.Borde, 5)) 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawRectangle(borde, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }

        private void EstilizarBotones()
        {
            UIHelpers.EstilizarBoton(btnIniciarSesion, AppStyles.Colores.Fondo);


            UIHelpers.EstilizarBoton(btnSalir, Color.Red); 
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            OpenForm<FormLogin>();
        }
        private void OpenForm<T>() where T : Form, new()
        {
            this.Hide();
            using (T form = new T())
            {
                form.ShowDialog();
            }
            this.Show();
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}