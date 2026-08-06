namespace BD_Libreria1;

public sealed class FormularioPrincipal : Form
{
    private readonly TabControl _pestanas = new();

    private static readonly TablaConfig[] Tablas =
    [
        new("Socios", "SOCIO", "ID_SOCIO"),
        new("Autores", "AUTOR", "ID_AUTOR"),
        new("Editoriales", "EDITORIAL", "ID_EDITORIAL"),
        new("Libros", "LIBRO", "ID_LIBRO"),
        new("Préstamos", "PRESTAMO", "ID_PRESTAMO"),
        new("Detalles de préstamos", "DETALLE_PRESTAMO", "ID_DETALLE_PRESTAMO")
    ];

    public FormularioPrincipal()
    {
        Text = "Sistema de Biblioteca";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 550);
        Size = new Size(1120, 680);
        BackColor = Color.FromArgb(242, 244, 248);

        var titulo = new Label
        {
            Text = "Administración de Biblioteca",
            Dock = DockStyle.Top,
            Height = 66,
            Padding = new Padding(18, 17, 0, 0),
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(25, 31, 42)
        };

        _pestanas.Dock = DockStyle.Fill;
        _pestanas.Font = new Font("Segoe UI", 10);

        foreach (TablaConfig tabla in Tablas)
        {
            var vista = new VistaTabla(tabla);
            var pestana = new TabPage(tabla.NombreVisible) { Padding = new Padding(3) };
            pestana.Controls.Add(vista);
            _pestanas.TabPages.Add(pestana);
        }

        _pestanas.SelectedIndexChanged += async (_, _) => await CargarPestanaActualAsync();
        Shown += async (_, _) => await CargarPestanaActualAsync();

        Controls.Add(_pestanas);
        Controls.Add(titulo);
    }

    private async Task CargarPestanaActualAsync()
    {
        if (_pestanas.SelectedTab?.Controls.OfType<VistaTabla>().FirstOrDefault() is { } vista)
            await vista.CargarAsync();
    }
}
