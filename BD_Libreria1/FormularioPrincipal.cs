using System.Data;
using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

public class FormularioPrincipal : Form
{
    // Estos son los controles que se muestran en la ventana.
    private ComboBox comboTablas = new ComboBox();
    private DataGridView tablaDatos = new DataGridView();
    private Button botonCargar = new Button();
    private Button botonNuevo = new Button();
    private Button botonGuardar = new Button();
    private Button botonEliminar = new Button();
    private Label textoEstado = new Label();

    // Aquí guardo temporalmente los datos que vienen de SQL Server.
    private DataTable datos = new DataTable();
    private SqlDataAdapter? adaptador;

    public FormularioPrincipal()
    {
        // Configuración básica de la ventana principal.
        Text = "Sistema de Biblioteca";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1100, 650);
        MinimumSize = new Size(850, 500);
        BackColor = Color.FromArgb(240, 242, 246);

        CrearInterfaz();
        CargarDatos();
    }

    private void CrearInterfaz()
    {
        // Este título ayuda a identificar rápidamente el programa.
        Label titulo = new Label();
        titulo.Text = "Administración de Biblioteca";
        titulo.Dock = DockStyle.Top;
        titulo.Height = 65;
        titulo.Padding = new Padding(18, 16, 0, 0);
        titulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        titulo.ForeColor = Color.White;
        titulo.BackColor = Color.FromArgb(30, 36, 48);

        // En esta barra coloqué la selección de tabla y los botones principales.
        FlowLayoutPanel barra = new FlowLayoutPanel();
        barra.Dock = DockStyle.Top;
        barra.Height = 55;
        barra.Padding = new Padding(10);
        barra.BackColor = Color.FromArgb(48, 55, 70);

        comboTablas.Width = 190;
        comboTablas.DropDownStyle = ComboBoxStyle.DropDownList;
        comboTablas.Items.Add("SOCIO");
        comboTablas.Items.Add("AUTOR");
        comboTablas.Items.Add("EDITORIAL");
        comboTablas.Items.Add("LIBRO");
        comboTablas.Items.Add("PRESTAMO");
        comboTablas.Items.Add("DETALLE_PRESTAMO");
        comboTablas.SelectedIndex = 0;
        comboTablas.SelectedIndexChanged += CambiarTabla;

        PrepararBoton(botonCargar, "Cargar", Color.FromArgb(70, 78, 92));
        PrepararBoton(botonNuevo, "Nueva fila", Color.FromArgb(70, 78, 92));
        PrepararBoton(botonGuardar, "Guardar", Color.FromArgb(38, 132, 90));
        PrepararBoton(botonEliminar, "Eliminar", Color.FromArgb(170, 65, 65));

        botonCargar.Click += CargarClick;
        botonNuevo.Click += NuevoClick;
        botonGuardar.Click += GuardarClick;
        botonEliminar.Click += EliminarClick;

        textoEstado.AutoSize = true;
        textoEstado.ForeColor = Color.White;
        textoEstado.Margin = new Padding(15, 8, 0, 0);

        barra.Controls.Add(comboTablas);
        barra.Controls.Add(botonCargar);
        barra.Controls.Add(botonNuevo);
        barra.Controls.Add(botonGuardar);
        barra.Controls.Add(botonEliminar);
        barra.Controls.Add(textoEstado);

        // El DataGridView imprime los registros y también permite editar sus celdas.
        tablaDatos.Dock = DockStyle.Fill;
        tablaDatos.BackgroundColor = Color.White;
        tablaDatos.BorderStyle = BorderStyle.None;
        tablaDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tablaDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tablaDatos.MultiSelect = false;
        tablaDatos.AllowUserToAddRows = false;

        Controls.Add(tablaDatos);
        Controls.Add(barra);
        Controls.Add(titulo);
    }

    private void PrepararBoton(Button boton, string texto, Color color)
    {
        // Todos los botones usan el mismo estilo para que la interfaz se vea ordenada.
        boton.Text = texto;
        boton.AutoSize = true;
        boton.Height = 32;
        boton.BackColor = color;
        boton.ForeColor = Color.White;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Cursor = Cursors.Hand;
    }

    private void CargarDatos()
    {
        try
        {
            // Tomo la tabla seleccionada y hago un SELECT directo a SQL Server.
            string nombreTabla = comboTablas.Text;
            string consulta = "SELECT * FROM dbo.[" + nombreTabla + "]";

            SqlConnection conexion = ConexionBiblioteca.CrearConexion();
            adaptador = new SqlDataAdapter(consulta, conexion);

            // El CommandBuilder crea los comandos INSERT, UPDATE y DELETE automáticamente.
            SqlCommandBuilder comandos = new SqlCommandBuilder(adaptador);
            comandos.QuotePrefix = "[";
            comandos.QuoteSuffix = "]";
            adaptador.InsertCommand = comandos.GetInsertCommand();
            adaptador.UpdateCommand = comandos.GetUpdateCommand();
            adaptador.DeleteCommand = comandos.GetDeleteCommand();

            datos = new DataTable();
            adaptador.Fill(datos);
            tablaDatos.DataSource = datos;

            BloquearColumnaId();
            textoEstado.Text = datos.Rows.Count + " registros cargados";
        }
        catch (Exception error)
        {
            MostrarError("No se pudieron cargar los datos.\n\n" + error.Message);
        }
    }

    private void BloquearColumnaId()
    {
        // Los ID los genera SQL Server, por eso el usuario no debe escribirlos.
        if (tablaDatos.Columns.Count > 0)
        {
            tablaDatos.Columns[0].ReadOnly = true;
            tablaDatos.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
        }
    }

    private void AgregarFila()
    {
        // Creo una fila vacía para que el usuario capture los datos desde la tabla.
        DataRow filaNueva = datos.NewRow();
        datos.Rows.Add(filaNueva);
        textoEstado.Text = "Completa la fila nueva y presiona Guardar";
    }

    private void GuardarDatos()
    {
        try
        {
            // Finalizo la edición de la celda actual antes de enviar los cambios.
            tablaDatos.EndEdit();

            if (adaptador == null)
            {
                return;
            }

            int cambios = adaptador.Update(datos);
            CargarDatos();
            textoEstado.Text = cambios + " cambios guardados";
        }
        catch (Exception error)
        {
            MostrarError("No se pudieron guardar los cambios. Revisa los campos y los ID relacionados.\n\n" + error.Message);
        }
    }

    private void EliminarFila()
    {
        // Primero confirmo la acción para evitar borrar un registro por accidente.
        if (tablaDatos.CurrentRow == null)
        {
            return;
        }

        DialogResult respuesta = MessageBox.Show(
            "¿Quieres eliminar la fila seleccionada?",
            "Confirmar",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (respuesta == DialogResult.Yes)
        {
            int numeroFila = tablaDatos.CurrentRow.Index;
            datos.Rows[numeroFila].Delete();
            textoEstado.Text = "Presiona Guardar para confirmar la eliminación";
        }
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(mensaje, "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Estos eventos llaman a los métodos anteriores cuando el usuario usa la interfaz.
    private void CambiarTabla(object? sender, EventArgs e) { CargarDatos(); }
    private void CargarClick(object? sender, EventArgs e) { CargarDatos(); }
    private void NuevoClick(object? sender, EventArgs e) { AgregarFila(); }
    private void GuardarClick(object? sender, EventArgs e) { GuardarDatos(); }
    private void EliminarClick(object? sender, EventArgs e) { EliminarFila(); }
}
