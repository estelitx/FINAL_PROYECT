using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

public class FormularioRegistro : Form
{
    // Guarda el nombre de la tabla donde se va a agregar el registro.
    private string nombreTabla;

    // Estos controles se reutilizan porque cada tabla pide campos diferentes.
    // PrepararCampos decide cuáles se necesitan en cada caso.
    private Label etiqueta1 = new Label();
    private Label etiqueta2 = new Label();
    private Label etiqueta3 = new Label();
    private TextBox caja1 = new TextBox();
    private TextBox caja2 = new TextBox();
    private TextBox caja3 = new TextBox();
    private ComboBox lista1 = new ComboBox();
    private ComboBox lista2 = new ComboBox();
    private DateTimePicker fecha1 = new DateTimePicker();
    private DateTimePicker fecha2 = new DateTimePicker();
    private Button botonGuardar = new Button();
    private Button botonCancelar = new Button();

    public FormularioRegistro(string tablaSeleccionada)
    {
        nombreTabla = tablaSeleccionada;

        Text = "Agregar registro - " + nombreTabla;
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(500, 370);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        CrearControles();
        PrepararCampos();
    }

    private void CrearControles()
    {
        // El panel acomoda las etiquetas de un lado y los campos del otro para que no queden sueltos.
        TableLayoutPanel panel = new TableLayoutPanel();
        panel.Dock = DockStyle.Fill;
        panel.Padding = new Padding(25);
        panel.ColumnCount = 2;
        panel.RowCount = 5;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

        PrepararCampo(etiqueta1, caja1);
        PrepararCampo(etiqueta2, caja2);
        PrepararCampo(etiqueta3, caja3);

        lista1.DropDownStyle = ComboBoxStyle.DropDownList;
        lista1.Dock = DockStyle.Fill;
        lista2.DropDownStyle = ComboBoxStyle.DropDownList;
        lista2.Dock = DockStyle.Fill;
        fecha1.Dock = DockStyle.Fill;
        fecha1.Format = DateTimePickerFormat.Short;
        fecha2.Dock = DockStyle.Fill;
        fecha2.Format = DateTimePickerFormat.Short;

        panel.Controls.Add(etiqueta1, 0, 0);
        panel.Controls.Add(caja1, 1, 0);
        panel.Controls.Add(etiqueta2, 0, 1);
        panel.Controls.Add(caja2, 1, 1);
        panel.Controls.Add(etiqueta3, 0, 2);
        panel.Controls.Add(caja3, 1, 2);

        FlowLayoutPanel botones = new FlowLayoutPanel();
        botones.Dock = DockStyle.Fill;
        botones.FlowDirection = FlowDirection.RightToLeft;

        PrepararBoton(botonGuardar, "Guardar", Color.FromArgb(38, 132, 90));
        PrepararBoton(botonCancelar, "Cancelar", Color.FromArgb(100, 105, 115));
        botonGuardar.Click += GuardarClick;
        botonCancelar.Click += CancelarClick;

        botones.Controls.Add(botonGuardar);
        botones.Controls.Add(botonCancelar);
        panel.Controls.Add(botones, 0, 4);
        panel.SetColumnSpan(botones, 2);

        Controls.Add(panel);
        AcceptButton = botonGuardar;
        CancelButton = botonCancelar;
    }

    private void PrepararCampo(Label etiqueta, TextBox caja)
    {
        etiqueta.AutoSize = true;
        etiqueta.Anchor = AnchorStyles.Left;
        etiqueta.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        caja.Dock = DockStyle.Fill;
        caja.Font = new Font("Segoe UI", 10);
    }

    private void PrepararBoton(Button boton, string texto, Color color)
    {
        boton.Text = texto;
        boton.AutoSize = true;
        boton.Height = 35;
        boton.BackColor = color;
        boton.ForeColor = Color.White;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
    }

    private void PrepararCampos()
    {
        // Al principio se ocultan los campos y luego se muestran los que ocupa cada tabla.
        // Por ejemplo, para las relaciones se usan listas con nombres en vez de pedir un ID.
        etiqueta1.Visible = false;
        etiqueta2.Visible = false;
        etiqueta3.Visible = false;
        caja1.Visible = false;
        caja2.Visible = false;
        caja3.Visible = false;

        if (nombreTabla == "SOCIO")
        {
            MostrarCaja(etiqueta1, caja1, "Nombre:");
            MostrarCaja(etiqueta2, caja2, "Teléfono:");
            MostrarCaja(etiqueta3, caja3, "Correo:");
        }
        else if (nombreTabla == "AUTOR")
        {
            MostrarCaja(etiqueta1, caja1, "Nombre del autor:");
        }
        else if (nombreTabla == "EDITORIAL")
        {
            MostrarCaja(etiqueta1, caja1, "Nombre de editorial:");
        }
        else if (nombreTabla == "LIBRO")
        {
            MostrarCaja(etiqueta1, caja1, "Título:");
            CambiarPorLista(etiqueta2, caja2, lista1, "Autor:");
            CambiarPorLista(etiqueta3, caja3, lista2, "Editorial:");
            CargarLista(lista1, "SELECT ID_AUTOR, AUTOR FROM AUTOR ORDER BY AUTOR", "AUTOR", "ID_AUTOR");
            CargarLista(lista2, "SELECT ID_EDITORIAL, EDITORIAL FROM EDITORIAL ORDER BY EDITORIAL", "EDITORIAL", "ID_EDITORIAL");
        }
        else if (nombreTabla == "PRESTAMO")
        {
            CambiarPorFecha(etiqueta1, caja1, fecha1, "Fecha préstamo:");
            CambiarPorFecha(etiqueta2, caja2, fecha2, "Fecha devolución:");
            CambiarPorLista(etiqueta3, caja3, lista1, "Socio:");
            CargarLista(lista1, "SELECT ID_SOCIO, NOMBRE FROM SOCIO ORDER BY NOMBRE", "NOMBRE", "ID_SOCIO");
        }
        else if (nombreTabla == "DETALLE_PRESTAMO")
        {
            CambiarPorLista(etiqueta1, caja1, lista1, "Préstamo:");
            CambiarPorLista(etiqueta2, caja2, lista2, "Libro:");
            CargarLista(lista1, "SELECT ID_PRESTAMO, CONCAT('Préstamo ', ID_PRESTAMO) AS TEXTO FROM PRESTAMO ORDER BY ID_PRESTAMO", "TEXTO", "ID_PRESTAMO");
            CargarLista(lista2, "SELECT ID_LIBRO, TITULO FROM LIBRO ORDER BY TITULO", "TITULO", "ID_LIBRO");
        }
    }

    private void MostrarCaja(Label etiqueta, TextBox caja, string texto)
    {
        etiqueta.Text = texto;
        etiqueta.Visible = true;
        caja.Visible = true;
    }

    private void CambiarPorLista(Label etiqueta, TextBox caja, ComboBox lista, string texto)
    {
        etiqueta.Text = texto;
        etiqueta.Visible = true;
        caja.Visible = false;
        TableLayoutPanel panel = (TableLayoutPanel)etiqueta.Parent!;
        int fila = panel.GetRow(etiqueta);
        panel.Controls.Remove(caja);
        panel.Controls.Add(lista, 1, fila);
    }

    private void CambiarPorFecha(Label etiqueta, TextBox caja, DateTimePicker fecha, string texto)
    {
        etiqueta.Text = texto;
        etiqueta.Visible = true;
        caja.Visible = false;
        TableLayoutPanel panel = (TableLayoutPanel)etiqueta.Parent!;
        int fila = panel.GetRow(etiqueta);
        panel.Controls.Remove(caja);
        panel.Controls.Add(fecha, 1, fila);
    }

    private void CargarLista(ComboBox lista, string consulta, string mostrar, string valor)
    {
        // Aquí se llena una lista usando los datos de otra tabla.
        // Se ve el nombre en pantalla, pero internamente se conserva el ID correspondiente.
        using SqlConnection conexion = ConexionBiblioteca.CrearConexion();
        SqlDataAdapter adaptadorLista = new SqlDataAdapter(consulta, conexion);
        System.Data.DataTable datosLista = new System.Data.DataTable();
        adaptadorLista.Fill(datosLista);

        lista.DataSource = datosLista;
        lista.DisplayMember = mostrar;
        lista.ValueMember = valor;
    }

    private void GuardarRegistro()
    {
        using SqlConnection conexion = ConexionBiblioteca.CrearConexion();
        conexion.Open();

        SqlCommand comando = conexion.CreateCommand();

        // Dependiendo de la tabla se usa un INSERT diferente. El ID no se manda porque
        // SQL Server lo genera solo y SCOPE_IDENTITY permite obtener el número que quedó.
        if (nombreTabla == "SOCIO")
        {
            ValidarTexto(caja1, "nombre");
            ValidarTexto(caja2, "teléfono");
            ValidarTexto(caja3, "correo");
            comando.CommandText = "INSERT INTO SOCIO (NOMBRE, TELEFONO, CORREO) VALUES (@dato1, @dato2, @dato3); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", caja1.Text.Trim());
            comando.Parameters.AddWithValue("@dato2", caja2.Text.Trim());
            comando.Parameters.AddWithValue("@dato3", caja3.Text.Trim());
        }
        else if (nombreTabla == "AUTOR")
        {
            ValidarTexto(caja1, "autor");
            comando.CommandText = "INSERT INTO AUTOR (AUTOR) VALUES (@dato1); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", caja1.Text.Trim());
        }
        else if (nombreTabla == "EDITORIAL")
        {
            ValidarTexto(caja1, "editorial");
            comando.CommandText = "INSERT INTO EDITORIAL (EDITORIAL) VALUES (@dato1); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", caja1.Text.Trim());
        }
        else if (nombreTabla == "LIBRO")
        {
            ValidarTexto(caja1, "título");
            ValidarLista(lista1, "autor");
            ValidarLista(lista2, "editorial");
            comando.CommandText = "INSERT INTO LIBRO (TITULO, ID_AUTOR, ID_EDITORIAL) VALUES (@dato1, @dato2, @dato3); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", caja1.Text.Trim());
            comando.Parameters.AddWithValue("@dato2", lista1.SelectedValue!);
            comando.Parameters.AddWithValue("@dato3", lista2.SelectedValue!);
        }
        else if (nombreTabla == "PRESTAMO")
        {
            ValidarLista(lista1, "socio");

            if (fecha2.Value.Date < fecha1.Value.Date)
            {
                throw new Exception("La fecha de devolución no puede ser anterior a la fecha de préstamo.");
            }

            comando.CommandText = "INSERT INTO PRESTAMO (FECHA_PRESTAMO, FECHA_DEVOLUCION, ID_SOCIO) VALUES (@dato1, @dato2, @dato3); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", fecha1.Value.Date);
            comando.Parameters.AddWithValue("@dato2", fecha2.Value.Date);
            comando.Parameters.AddWithValue("@dato3", lista1.SelectedValue!);
        }
        else if (nombreTabla == "DETALLE_PRESTAMO")
        {
            ValidarLista(lista1, "préstamo");
            ValidarLista(lista2, "libro");
            comando.CommandText = "INSERT INTO DETALLE_PRESTAMO (ID_PRESTAMO, ID_LIBRO) VALUES (@dato1, @dato2); SELECT SCOPE_IDENTITY();";
            comando.Parameters.AddWithValue("@dato1", lista1.SelectedValue!);
            comando.Parameters.AddWithValue("@dato2", lista2.SelectedValue!);
        }

        object? resultado = comando.ExecuteScalar();
        int nuevoId = Convert.ToInt32(resultado);

        MessageBox.Show(
            "Registro agregado correctamente.\nID generado: " + nuevoId,
            "Biblioteca",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ValidarTexto(TextBox caja, string campo)
    {
        if (string.IsNullOrWhiteSpace(caja.Text))
        {
            caja.Focus();
            throw new Exception("Debes escribir el campo " + campo + ".");
        }
    }

    private void ValidarLista(ComboBox lista, string campo)
    {
        if (lista.SelectedIndex < 0)
        {
            lista.Focus();
            throw new Exception("Debes seleccionar " + campo + ".");
        }
    }

    private void GuardarClick(object? sender, EventArgs e)
    {
        try
        {
            GuardarRegistro();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception error)
        {
            MessageBox.Show(error.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CancelarClick(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
