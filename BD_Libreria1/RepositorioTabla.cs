using System.Data;
using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

public sealed class RepositorioTabla(TablaConfig config) : IDisposable
{
    private readonly SqlDataAdapter _adaptador = CrearAdaptador(config);

    public DataTable Datos { get; } = new(config.NombreSql);

    public async Task CargarAsync()
    {
        Datos.Clear();
        await using SqlConnection conexion = ConexionBiblioteca.CrearConexion();
        await conexion.OpenAsync();
        _adaptador.SelectCommand!.Connection = conexion;
        _adaptador.Fill(Datos);

        if (Datos.Columns.Contains(config.LlavePrimaria))
            Datos.Columns[config.LlavePrimaria]!.ReadOnly = true;
    }

    public async Task<int> GuardarAsync()
    {
        await using SqlConnection conexion = ConexionBiblioteca.CrearConexion();
        await conexion.OpenAsync();

        _adaptador.SelectCommand!.Connection = conexion;
        if (_adaptador.InsertCommand is not null) _adaptador.InsertCommand.Connection = conexion;
        if (_adaptador.UpdateCommand is not null) _adaptador.UpdateCommand.Connection = conexion;
        if (_adaptador.DeleteCommand is not null) _adaptador.DeleteCommand.Connection = conexion;

        return _adaptador.Update(Datos);
    }

    private static SqlDataAdapter CrearAdaptador(TablaConfig tabla)
    {
        var adaptador = new SqlDataAdapter(
            $"SELECT * FROM dbo.[{tabla.NombreSql}]",
            ConexionBiblioteca.ObtenerCadenaConexion())
        {
            MissingSchemaAction = MissingSchemaAction.AddWithKey
        };

        var constructor = new SqlCommandBuilder(adaptador)
        {
            QuotePrefix = "[",
            QuoteSuffix = "]"
        };

        adaptador.InsertCommand = constructor.GetInsertCommand();
        adaptador.UpdateCommand = constructor.GetUpdateCommand();
        adaptador.DeleteCommand = constructor.GetDeleteCommand();
        return adaptador;
    }

    public void Dispose()
    {
        _adaptador.Dispose();
        Datos.Dispose();
    }
}
