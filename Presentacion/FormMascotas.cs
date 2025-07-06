using Datos;
using Negocio;
using Presentacion.Helpers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class FormMascotas : Form
    {
        private readonly NMascota negocioMascota;
        private readonly NEspecie negocioEspecie;
        private int idSeleccionado = -1;
        private int idClienteActual = -1;

        // Cambia el constructor para recibir el nombre del cliente
        public FormMascotas(int idCliente, string nombreCliente, BDEFEntities context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (idCliente <= 0)
                throw new ArgumentException("ID de cliente inválido", nameof(idCliente));

            InitializeComponent();
            idClienteActual = idCliente;

            // Establecer el texto del label
            lblCliente.Text = $"Mascotas de {nombreCliente}";

            // Resto de la inicialización...
            negocioEspecie = new NEspecie(context);
            negocioMascota = new NMascota(context);

            InitializeUI();
            ConfigurarDgv();
            CargarDatosIniciales();
            CargarMascotas();
        }
        private void RestablecerBotones()
        {
            UIHelpers.EstilizarBoton(btnRegistrar, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnCerrar, AppStyles.Colores.Error);
            // Agrega otros botones si es necesario
        }
        private void InitializeUI()
        {
            // Configuración básica del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppStyles.Colores.Naranja;
            this.DoubleBuffered = true;
            this.Padding = new Padding(2);

            UIHelpers.HabilitarArrastre(this);
            UIHelpers.RedondearBordes(this, AppStyles.Tamanos.RadioBordes);

            // Configurar panel
            panel1.BackColor = AppStyles.Colores.Fondo;
            panel1.Padding = new Padding(10);
            panel1.BorderStyle = BorderStyle.FixedSingle;

            // Estilizar controles
            UIHelpers.EstilizarBoton(btnRegistrar, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnCerrar, AppStyles.Colores.Error);

            foreach (Control c in panel1.Controls)
            {
                if (c is TextBox) UIHelpers.EstilizarTextBox((TextBox)c, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
                if (c is ComboBox) UIHelpers.EstilizarComboBox((ComboBox)c, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
                if (c is Label) c.ForeColor = AppStyles.Colores.TextoPrimario;
            }

            // Configurar animación del logo
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Opcional: puedes agregar dibujado personalizado aquí
            // Por ejemplo, para bordes redondeados:
            using (var path = new GraphicsPath())
            {
                int radius = AppStyles.Tamanos.RadioBordes;
                Rectangle rect = panel1.ClientRectangle;

                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                panel1.Region = new Region(path);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(AppStyles.Colores.Borde, 5))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void ConfigurarDgv()
        {
            dgvMascotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMascotas.MultiSelect = false;
            dgvMascotas.ReadOnly = true;
            dgvMascotas.AllowUserToAddRows = false;
            dgvMascotas.CellClick += dgvMascotas_CellClick;

            // Estilo del DataGridView
            dgvMascotas.EnableHeadersVisualStyles = false;
            dgvMascotas.ColumnHeadersDefaultCellStyle.BackColor = AppStyles.Colores.Borde;
            dgvMascotas.ColumnHeadersDefaultCellStyle.ForeColor = AppStyles.Colores.TextoPrimario;
            dgvMascotas.ColumnHeadersDefaultCellStyle.Font = AppStyles.Fuentes.Subtitulo;
        }

        private void CargarDatosIniciales()
        {
            try
            {
                // Cargar especies
                cbEspecie.DataSource = negocioEspecie.ObtenerEspeciesParaComboBox();
                cbEspecie.DisplayMember = "Nombre";
                cbEspecie.ValueMember = "IDEspecie";

                // Configurar sexo
                cbSexo.Items.AddRange(new object[] { "M", "F" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos iniciales: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarMascotas()
        {
            try
            {
                dgvMascotas.Columns.Clear();
                dgvMascotas.DataSource = null;

                var mascotas = negocioMascota.ListarPorCliente(idClienteActual)
                    .Where(m => !m.Eliminado)
                    .Select(m => new
                    {
                        m.IDMascota,
                        m.Nombre,
                        Especie = m.Especie.Nombre,
                        m.Edad,
                        m.Sexo,
                        FechaRegistro = m.FechaCreacion.ToString("dd/MM/yyyy")
                    }).ToList();

                dgvMascotas.DataSource = mascotas;

                // Configurar columnas
                dgvMascotas.Columns["IDMascota"].Visible = false;
                dgvMascotas.Columns["Nombre"].HeaderText = "Nombre";
                dgvMascotas.Columns["Especie"].HeaderText = "Especie";
                dgvMascotas.Columns["Edad"].HeaderText = "Edad";
                dgvMascotas.Columns["Sexo"].HeaderText = "Sexo";
                dgvMascotas.Columns["FechaRegistro"].HeaderText = "Fecha Registro";

                // Agregar columnas de botones
                AgregarColumnasBotones();

                dgvMascotas.ClearSelection();
                idSeleccionado = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar mascotas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarColumnasBotones()
        {
            if (dgvMascotas.Columns.Contains("Modificar"))
                dgvMascotas.Columns.Remove("Modificar");
            if (dgvMascotas.Columns.Contains("Eliminar"))
                dgvMascotas.Columns.Remove("Eliminar");

            // Columna Modificar
            var modCol = new DataGridViewButtonColumn
            {
                Name = "Modificar",
                HeaderText = "Acciones",
                Text = "✏️ Editar",
                UseColumnTextForButtonValue = true,
                Width = 100,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 193, 7),
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                }
            };
            dgvMascotas.Columns.Add(modCol);

            // Columna Eliminar
            var delCol = new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "",
                Text = "🗑️ Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 100,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                }
            };
            dgvMascotas.Columns.Add(delCol);
        }

        private void dgvMascotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                int idMascota = Convert.ToInt32(dgvMascotas.Rows[e.RowIndex].Cells["IDMascota"].Value);
                idSeleccionado = idMascota;

                var columnName = dgvMascotas.Columns[e.ColumnIndex].Name;

                if (columnName == "Modificar")
                {
                    CargarDatosMascota(idMascota);
                }
                else if (columnName == "Eliminar")
                {
                    var confirm = MessageBox.Show("¿Está seguro de eliminar esta mascota?",
                                              "Confirmar eliminación",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        string resultado = negocioMascota.EliminarLogico(idMascota);
                        MessageBox.Show(resultado, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarMascotas();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la acción: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosMascota(int idMascota)
        {
            try
            {
                var mascota = negocioMascota.ListarPorCliente(idClienteActual)
                    .FirstOrDefault(m => m.IDMascota == idMascota);

                if (mascota == null)
                {
                    MessageBox.Show("Mascota no encontrada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtNombre.Text = mascota.Nombre;
                txtEdad.Text = mascota.Edad?.ToString();
                cbEspecie.SelectedValue = mascota.IDEspecie;
                cbSexo.SelectedItem = mascota.Sexo;

                btnRegistrar.Text = "Actualizar";
                btnRegistrar.BackColor = Color.FromArgb(40, 167, 69);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar mascota: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var mascota = new Mascota
                {
                    IDMascota = idSeleccionado,
                    Nombre = txtNombre.Text.Trim(),
                    IDEspecie = (int)cbEspecie.SelectedValue,
                    Edad = string.IsNullOrWhiteSpace(txtEdad.Text) ? (int?)null : int.Parse(txtEdad.Text),
                    Sexo = cbSexo.SelectedItem?.ToString(),
                    IDCliente = idClienteActual,
                    FechaModificacion = DateTime.Now
                };

                string resultado;
                if (idSeleccionado > 0) // Modificación
                {
                    resultado = negocioMascota.Modificar(mascota);
                }
                else // Registro nuevo
                {
                    mascota.FechaCreacion = DateTime.Now;
                    mascota.Eliminado = false;
                    resultado = negocioMascota.Registrar(mascota);
                }

                MessageBox.Show(resultado, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RestablecerBotones(); // Añade esta línea
                CargarMascotas();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar mascota: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbEspecie.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una especie", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEdad.Text) && !int.TryParse(txtEdad.Text, out _))
            {
                MessageBox.Show("La edad debe ser un número válido", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtEdad.Clear();
            if (cbEspecie.Items.Count > 0) cbEspecie.SelectedIndex = 0;
            cbSexo.SelectedIndex = -1;

            btnRegistrar.Text = "Registrar";
            btnRegistrar.BackColor = AppStyles.Colores.Naranja;
            dgvMascotas.ClearSelection();
            idSeleccionado = -1;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}