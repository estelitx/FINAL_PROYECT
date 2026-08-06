using System.Data;

namespace BD_Libreria1;

public sealed class VistaTabla : UserControl
{
    private readonly RepositorioTabla _repositorio;
    private readonly DataGridView _tabla = new();
    private readonly Label _estado = new();

    public VistaTabla(TablaConfig config)
    {
        _repositorio = new RepositorioTabla(config);
        Dock = DockStyle.Fill;

        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(8),
            BackColor = Color.FromArgb(35, 40, 50),
            WrapContents = false
        };

        barra.Controls.Add(CrearBoton("Recargar", async (_, _) => await CargarAsync()));
        barra.Controls.Add(CrearBoton("Nuevo", (_, _) => AgregarFila()));
        barra.Controls.Add(CrearBoton("Guardar cambios", async (_, _) => await GuardarAsync(), Color.FromArgb(38, 132, 90)));
        barra.Controls.Add(CrearBoton("Eliminar fila", (_, _) => EliminarFila(), Color.FromArgb(170, 65, 65)));

        _estado.AutoSize = true;
        _estado.ForeColor = Color.WhiteSmoke;
        _estado.Margin = new Padding(18, 10, 0, 0);
        barra.Controls.Add(_estado);

        _tabla.Dock = DockStyle.Fill;
        _tabla.AutoGenerateColumns = true;
        _tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _tabla.AllowUserToAddRows = false;
        _tabla.MultiSelect = false;
        _tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _tabla.BackgroundColor = Color.White;
        _tabla.BorderStyle = BorderStyle.None;
        _tabla.DataError += (_, e) =>
        {
            e.ThrowException = false;
            MostrarError("El valor introducido no es válido para esta columna.");
        };

        Controls.Add(_tabla);
        Controls.Add(barra);
    }

    public async Task CargarAsync()
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            await _repositorio.CargarAsync();
            _tabla.DataSource = _repositorio.Datos;
            _estado.Text = $"{_repositorio.Datos.Rows.Count} registros";
        }
        catch (Exception ex)
        {
            MostrarError(ex.Message);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void AgregarFila()
    {
        DataRow nueva = _repositorio.Datos.NewRow();
        _repositorio.Datos.Rows.Add(nueva);
        DataGridViewCell? editable = _tabla.Rows[^1].Cells.Cast<DataGridViewCell>().FirstOrDefault(c => !c.ReadOnly);
        if (editable is not null) _tabla.CurrentCell = editable;
        _tabla.BeginEdit(true);
        _estado.Text = "Completa la fila y pulsa Guardar cambios";
    }

    private void EliminarFila()
    {
        if (_tabla.CurrentRow?.DataBoundItem is not DataRowView fila) return;

        DialogResult respuesta = MessageBox.Show(
            "¿Marcar esta fila para eliminarla? Se aplicará al guardar los cambios.",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (respuesta == DialogResult.Yes)
        {
            fila.Delete();
            _estado.Text = "Fila marcada para eliminar";
        }
    }

    private async Task GuardarAsync()
    {
        try
        {
            Validate();
            _tabla.EndEdit();
            int cambios = await _repositorio.GuardarAsync();
            await CargarAsync();
            _estado.Text = $"{cambios} cambios guardados correctamente";
        }
        catch (Exception ex)
        {
            MostrarError("No se pudieron guardar los cambios. Revisa campos obligatorios, valores repetidos y relaciones.\n\n" + ex.Message);
        }
    }

    private static Button CrearBoton(string texto, EventHandler accion, Color? color = null)
    {
        var boton = new Button
        {
            Text = texto,
            AutoSize = true,
            Height = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = color ?? Color.FromArgb(70, 78, 92),
            ForeColor = Color.White,
            Margin = new Padding(3, 1, 5, 1),
            Cursor = Cursors.Hand
        };
        boton.FlatAppearance.BorderSize = 0;
        boton.Click += accion;
        return boton;
    }

    private static void MostrarError(string mensaje) =>
        MessageBox.Show(mensaje, "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Error);

    protected override void Dispose(bool disposing)
    {
        if (disposing) _repositorio.Dispose();
        base.Dispose(disposing);
    }
}
