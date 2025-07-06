using Datos;
using Negocio;
using Presentacion.Helpers; // Añadido para usar los helpers
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            ConfigurarFormulario();
            EstilizarControles();
        }

        private void ConfigurarFormulario()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.BackColor = AppStyles.Colores.Fondo;

            UIHelpers.RedondearBordes(this, 40);
            UIHelpers.HabilitarArrastre(this);
        }

        private void EstilizarControles()
        {
            // Logo animado
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);

            // TextBox
            UIHelpers.EstilizarTextBox(txtUsuario, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarTextBox(txtContraseña, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
            txtContraseña.UseSystemPasswordChar = true;

            // Botones
            UIHelpers.EstilizarBoton(btnAcceder, AppStyles.Colores.Fondo);
            UIHelpers.EstilizarBoton(btnCancelar, Color.Red);

            // Checkbox
            chkMostrar.CheckedChanged += (s, e) =>
                txtContraseña.UseSystemPasswordChar = !chkMostrar.Checked;
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
        private void OpenForm<T>() where T : Form, new()
        {
            this.Hide();
            using (T form = new T())
            {
                form.ShowDialog();
            }
            this.Show();
        }

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContraseña.Text;

            NUsuario negocioUsuario = new NUsuario();
            Usuario u = negocioUsuario.ValidarCredenciales(usuario, contrasena);

            // Agrega esto para depuración:
            Console.WriteLine($"Usuario: {usuario}, Contraseña: {contrasena}");
            Console.WriteLine($"Resultado validación: {(u != null ? "Éxito" : "Fallo")}");

            if (u != null)
            {
                MessageBox.Show($"Bienvenido {u.NombreUsuario}\nRol: {u.Rol.Nombre}");

                Form formDestino;
                if (u.IDRol == 1) // Recepcionista
                {
                    formDestino = new FormRecepcionista();
                }
                else // Veterinario
                {
                    formDestino = new FormVeterinario(u); // Asegúrate que este constructor existe
                }

                this.Hide(); // Oculta el login en lugar de cerrarlo
                formDestino.ShowDialog(); // Usa ShowDialog para mantener el contexto
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            // Mostrar u ocultar la contraseña según el estado del checkbox
            if (chkMostrar.Checked)
            {
                txtContraseña.PasswordChar = '\0'; // Mostrar texto normal
            }
            else
            {
                txtContraseña.PasswordChar = '*'; // Ocultar con caracteres
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}