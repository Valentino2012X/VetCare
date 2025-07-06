using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Datos;
using Negocio;
using Presentacion.Helpers;

namespace Presentacion
{
    public partial class FormVeterinario : Form
    {
        private Usuario usuarioActual;
        private Veterinario veterinarioLogueado;
        private Timer refreshTimer;

        public FormVeterinario(Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
            InitializeVeterinario();
            InitializeUI();
            InitializeRefreshTimer();
        }

        private void InitializeVeterinario()
        {
            try
            {
                using (var contexto = new BDEFEntities())
                {
                    veterinarioLogueado = contexto.Veterinario
                        .FirstOrDefault(v => v.IDUsuario == usuarioActual.IDUsuario);

                    if (veterinarioLogueado == null)
                    {
                        MessageBox.Show($"No se encontró información del veterinario asociado al usuario {usuarioActual.NombreUsuario}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    lblBienvenido.Text = $"Bienvenido, Dr. {veterinarioLogueado.Nombre}";
                    lblBienvenido.Font = AppStyles.Fuentes.Titulo;
                    lblBienvenido.ForeColor = AppStyles.Colores.TextoPrimario;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del veterinario: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeUI()
        {
            // Configuración básica del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppStyles.Colores.Fondo;
            UIHelpers.HabilitarArrastre(this);
            UIHelpers.RedondearBordes(this, AppStyles.Tamanos.RadioBordes);

            // Estilizar controles
            UIHelpers.EstilizarBoton(btnCompletar, AppStyles.Colores.Naranja, AppStyles.Colores.Borde);
            UIHelpers.EstilizarBoton(btnCerrar, AppStyles.Colores.Error);
            UIHelpers.EstilizarTextBox(txtDiagnostico, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);

            // Configurar DataGridView
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            dgvCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCitas.EnableHeadersVisualStyles = false;
            dgvCitas.ColumnHeadersDefaultCellStyle.BackColor = AppStyles.Colores.Borde;
            dgvCitas.ColumnHeadersDefaultCellStyle.ForeColor = AppStyles.Colores.TextoPrimario;
            dgvCitas.ColumnHeadersDefaultCellStyle.Font = AppStyles.Fuentes.Subtitulo;
            dgvCitas.RowHeadersVisible = false;
            dgvCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCitas.DefaultCellStyle.Font = AppStyles.Fuentes.Normal;
            dgvCitas.BackgroundColor = Color.White;
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new Timer { Interval = 300000 }; // 5 minutos
            refreshTimer.Tick += (s, e) => LoadVeterinarianAppointments();
            refreshTimer.Start();
        }

        private void FormVeterinario_Load(object sender, EventArgs e)
        {
            LoadVeterinarianAppointments();
        }

        private void LoadVeterinarianAppointments()
        {
            try
            {
                using (var contexto = new BDEFEntities())
                {
                    var citas = from c in contexto.Cita
                                where c.IDVeterinario == veterinarioLogueado.IDVeterinario &&
                                      c.EstadoCita.Nombre == "Programada" &&
                                      c.Eliminado == false
                                orderby c.FechaHora ascending
                                select new
                                {
                                    ID = c.IDCita,
                                    Fecha = c.FechaHora,
                                    Mascota = c.Mascota.Nombre,
                                    Cliente = c.Mascota.Cliente.Nombre + " " + c.Mascota.Cliente.Apellido,
                                    Tipo = c.TipoCita.Nombre,
                                    Motivo = c.Motivo
                                };

                    dgvCitas.DataSource = citas.ToList();
                    dgvCitas.Columns["ID"].Visible = false;

                    if (dgvCitas.Columns["Fecha"] != null)
                    {
                        dgvCitas.Columns["Fecha"].DefaultCellStyle.Format = "g";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCompletar_Click(object sender, EventArgs e)
        {
            if (dgvCitas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una cita para completar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("Por favor, ingrese un diagnóstico antes de completar la cita.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Está seguro que desea marcar esta cita como completada?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            CompleteSelectedAppointment();
        }

        private void CompleteSelectedAppointment()
        {
            try
            {
                int idCita = Convert.ToInt32(dgvCitas.SelectedRows[0].Cells["ID"].Value);
                string diagnostico = txtDiagnostico.Text.Trim();

                using (var contexto = new BDEFEntities())
                {
                    var cita = contexto.Cita.FirstOrDefault(c => c.IDCita == idCita);

                    if (cita != null)
                    {
                        cita.Diagnostico = diagnostico;
                        cita.IDEstadoCita = contexto.EstadoCita
                            .FirstOrDefault(ec => ec.Nombre == "Completada")?.IDEstadoCita ?? cita.IDEstadoCita;
                        cita.FechaModificacion = DateTime.Now;

                        contexto.SaveChanges();

                        MessageBox.Show("La cita ha sido completada correctamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtDiagnostico.Clear();
                        LoadVeterinarianAppointments();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al completar la cita: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvCitas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCitas.SelectedRows.Count > 0)
            {
                txtDiagnostico.Enabled = true;
                btnCompletar.Enabled = true;
            }
            else
            {
                txtDiagnostico.Enabled = false;
                btnCompletar.Enabled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}