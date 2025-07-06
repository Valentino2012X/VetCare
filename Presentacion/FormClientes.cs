using Datos;
using Negocio;
using Presentacion.Helpers;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class FormClientes : Form
    {
        private readonly NCliente negocioCliente = new NCliente();
        private int idSeleccionado = -1;
        private Timer logoTimer;

        public FormClientes()
        {
            InitializeComponent();
            InitializeUI();
            ConfigurarDgv();
            CargarClientes();
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
            UIHelpers.EstilizarBoton(btnVerMascotas, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnSalir, AppStyles.Colores.Error);

            foreach (Control c in panel1.Controls)
            {
                if (c is TextBox) UIHelpers.EstilizarTextBox((TextBox)c, AppStyles.Colores.Borde, AppStyles.Colores.Naranja);
                if (c is Label) c.ForeColor = AppStyles.Colores.TextoPrimario;
            }

            // Configurar animación del logo
            UIHelpers.ConfigurarAnimacionLogo(pictureBox1, AppStyles.Colores.Naranja);
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
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.CellClick += dgvClientes_CellClick;

            // Estilo del DataGridView
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = AppStyles.Colores.Borde;
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = AppStyles.Colores.TextoPrimario;
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = AppStyles.Fuentes.Subtitulo;
        }

        private void CargarClientes()
        {
            try
            {
                // Limpiar y configurar DataGridView
                dgvClientes.Columns.Clear();
                dgvClientes.DataSource = null;

                // Obtener datos y configurar fuente de datos
                var clientes = negocioCliente.ListarTodo()
                    .Where(c => !c.Eliminado) // Solo clientes no eliminados
                    .Select(c => new
                    {
                        c.IDCliente,
                        c.DNI,
                        c.Nombre,
                        c.Apellido,
                        c.Telefono,
                        c.Direccion,
                        FechaRegistro = c.FechaCreacion.ToString("dd/MM/yyyy")
                    }).ToList();

                dgvClientes.DataSource = clientes;

                // Configurar columnas
                dgvClientes.Columns["IDCliente"].Visible = false;
                dgvClientes.Columns["DNI"].HeaderText = "DNI";
                dgvClientes.Columns["Nombre"].HeaderText = "Nombre";
                dgvClientes.Columns["Apellido"].HeaderText = "Apellido";
                dgvClientes.Columns["Telefono"].HeaderText = "Teléfono";
                dgvClientes.Columns["Direccion"].HeaderText = "Dirección";
                dgvClientes.Columns["FechaRegistro"].HeaderText = "Fecha Registro";

                // Agregar columnas de botones
                AgregarColumnasBotones();

                // Configuración visual adicional
                dgvClientes.ClearSelection();
                idSeleccionado = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarColumnasBotones()
        {
            // Eliminar columnas existentes si las hay para evitar duplicados
            if (dgvClientes.Columns.Contains("Modificar"))
                dgvClientes.Columns.Remove("Modificar");
            if (dgvClientes.Columns.Contains("Eliminar"))
                dgvClientes.Columns.Remove("Eliminar");

            // Columna Modificar con estilo mejorado
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
                    BackColor = Color.FromArgb(255, 193, 7), // Amarillo
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                }
            };
            dgvClientes.Columns.Add(modCol);

            // Columna Eliminar con estilo mejorado
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
                    BackColor = Color.FromArgb(220, 53, 69), // Rojo
                    ForeColor = Color.White,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                }
            };
            dgvClientes.Columns.Add(delCol);
        }
        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                var row = dgvClientes.Rows[e.RowIndex];
                idSeleccionado = Convert.ToInt32(row.Cells["IDCliente"].Value);

                var columnName = dgvClientes.Columns[e.ColumnIndex].Name;

                if (columnName == "Modificar")
                {
                    CargarDatosCliente(idSeleccionado);
                }
                else if (columnName == "Eliminar")
                {
                    // Verificar si el cliente tiene mascotas
                    bool tieneMascotas;
                    using (var context = new BDEFEntities())
                    {
                        var mascotaService = new NMascota(context); // Pasar el contexto correctamente
                        tieneMascotas = mascotaService.ListarPorCliente(idSeleccionado).Any();
                    }

                    if (tieneMascotas)
                    {
                        MessageBox.Show("No se puede eliminar el cliente porque tiene mascotas registradas.",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Confirmación de eliminación
                    var confirm = MessageBox.Show("¿Está seguro de eliminar este cliente?",
                                                "Confirmar eliminación",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        string resultado = negocioCliente.EliminarLogico(idSeleccionado);
                        MessageBox.Show(resultado, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarClientes(); // Refrescar la lista
                        LimpiarCampos();  // Limpiar formulario
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la acción: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosCliente(int idCliente)
        {
            try
            {
                var cliente = negocioCliente.Obtener(idCliente);
                if (cliente == null)
                {
                    MessageBox.Show("Cliente no encontrado", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cargar datos en los controles
                txtDNI.Text = cliente.DNI;
                txtNombre.Text = cliente.Nombre;
                txtApellido.Text = cliente.Apellido;
                txtTelefono.Text = cliente.Telefono;
                txtDireccion.Text = cliente.Direccion;

                idSeleccionado = idCliente;

                // Cambiar el texto del botón principal a "Actualizar"
                btnRegistrar.Text = "Actualizar";
                btnRegistrar.BackColor = Color.FromArgb(40, 167, 69); // Verde
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar cliente: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var cliente = new Cliente
                {
                    IDCliente = idSeleccionado,
                    DNI = txtDNI.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    FechaModificacion = DateTime.Now
                };

                string resultado;
                if (idSeleccionado > 0)
                {
                    resultado = negocioCliente.Modificar(cliente);
                }
                else
                {
                    cliente.FechaCreacion = DateTime.Now;
                    cliente.Eliminado = false;
                    resultado = negocioCliente.Registrar(cliente);
                }

                MessageBox.Show(resultado, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RestablecerBotones(); // Usa el método corregido
                CargarClientes();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cliente: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                RestablecerBotones(); // Restaurar también en caso de error
            }
        }
        private void RestablecerBotones()
        {
            // Restablece el estilo de todos los botones
            UIHelpers.EstilizarBoton(btnRegistrar, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnVerMascotas, AppStyles.Colores.Naranja);
            UIHelpers.EstilizarBoton(btnSalir, AppStyles.Colores.Error);

            // Restablece texto del botón registrar si es necesario
            btnRegistrar.Text = idSeleccionado > 0 ? "Actualizar" : "Registrar";
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("DNI, Nombre y Apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnVerMascotas_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debes seleccionar un cliente primero", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvClientes.SelectedRows[0];
            if (selectedRow.Cells["IDCliente"].Value == null)
            {
                MessageBox.Show("El cliente seleccionado no es válido", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idCliente = Convert.ToInt32(selectedRow.Cells["IDCliente"].Value);
            string nombreCliente = $"{selectedRow.Cells["Nombre"].Value} {selectedRow.Cells["Apellido"].Value}";

            try
            {
                using (var context = new BDEFEntities())
                {
                    this.Hide();
                    using (var frm = new FormMascotas(idCliente, nombreCliente, context))
                    {
                        frm.ShowDialog();
                    }
                    this.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de mascotas: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LimpiarCampos()
        {
            txtDNI.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            idSeleccionado = -1;

            // Restaurar el botón principal
            btnRegistrar.Text = "Registrar";
            btnRegistrar.BackColor = AppStyles.Colores.Naranja;

            // Deseleccionar cualquier fila
            dgvClientes.ClearSelection();
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