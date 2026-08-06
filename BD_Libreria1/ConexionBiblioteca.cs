using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

// En esta clase dejé únicamente la conexión para no repetirla en todo el programa.
public static class ConexionBiblioteca
{
    public static SqlConnection CrearConexion()
    {
        // Primero busco una conexión personal. Esto permite que cada compañero use su servidor.
        string? conexionPersonal = Environment.GetEnvironmentVariable("BIBLIOTECA_CONNECTION_STRING");

        // Si no hay una conexión personal, uso SQL Server Express de esta computadora.
        string conexionLocal = @"Server=.\SQLEXPRESS;Database=BIBLIOTECA;Integrated Security=True;TrustServerCertificate=True;";

        string cadenaConexion = conexionPersonal ?? conexionLocal;
        SqlConnection conexion = new SqlConnection(cadenaConexion);

        return conexion;
    }
}
