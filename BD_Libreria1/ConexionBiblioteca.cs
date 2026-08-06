using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

public static class ConexionBiblioteca
{
    private const string VariableDeEntorno = "BIBLIOTECA_CONNECTION_STRING";

    private const string ConexionLocalPredeterminada =
        @"Server=.\SQLEXPRESS;Database=BIBLIOTECA;Integrated Security=True;TrustServerCertificate=True;";

    public static string ObtenerCadenaConexion()
    {
        return Environment.GetEnvironmentVariable(VariableDeEntorno)
            ?? ConexionLocalPredeterminada;
    }

    public static SqlConnection CrearConexion()
    {
        return new SqlConnection(ObtenerCadenaConexion());
    }

    public static async Task ProbarConexionAsync()
    {
        await using SqlConnection conexion = CrearConexion();
        await conexion.OpenAsync();
    }
}

