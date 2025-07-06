using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Datos;
using Negocio;
using Presentacion.Helpers;

namespace Presentacion
{
    public partial class FormRecepcionista : Form
    {
        private Timer logoTimer;

        public FormRecepcionista()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Configuración básica del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppStyles.Colores.Fondo;
            this.DoubleBuffered = true;
            this.Padding = new Padding(2);

            UIHelpers.HabilitarArrastre(this);
            UIHelpers.RedondearBordes(this, AppStyles.Tamanos.RadioBordes);

            // Configurar etiqueta de bienvenida
            lblBienvenido.Text = "Bienvenido, Recepcionista";
            lblBienvenido.Font = AppStyles.Fuentes.Titulo;
            lblBienvenido.ForeColor = AppStyles.Colores.TextoPrimario;

            // Estilizar botones
            UIHelpers.EstilizarBoton(btnClientes, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnCitas, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnPagos, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnReportes, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnSalir, AppStyles.Colores.Error);

            // Configurar animación del logo
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);
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

        private void btnClientes_Click(object sender, EventArgs e)
        {
            OpenForm<FormClientes>();
        }

        private void btnCitas_Click(object sender, EventArgs e)
        {
            OpenForm<FormCitas>();
        }

        private void btnPagos_Click(object sender, EventArgs e)
        {
            OpenForm<FormPagos>();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            OpenForm<FormReportes>();
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
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logoTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}