using Microsoft.Data.SqlClient;

namespace BD_Libreria1;

// Esta clase sirve para tener la conexión en un solo lugar y no repetirla en cada formulario.
public static class ConexionBiblioteca
{
    // El método regresa una conexión cerrada, o sea que se abre solamente cuando se va a usar.
    public static SqlConnection CrearConexion()
    {
        // Primero se revisa si existe una conexión personal, por ejemplo para otra computadora.
        // Así no es necesario cambiar el código ni subir contraseñas a GitHub.
        string? conexionPersonal = Environment.GetEnvironmentVariable("BIBLIOTECA_CONNECTION_STRING");

        // Si no se configuró otra conexión se utiliza esta, que apunta al SQL Express local.
        // En este caso entra con la cuenta de Windows y abre la base BIBLIOTECA.
        string conexionLocal = @"Server=.\SQLEXPRESS;Database=BIBLIOTECA;Integrated Security=True;TrustServerCertificate=True;";

        string cadenaConexion = conexionPersonal ?? conexionLocal;
        SqlConnection conexion = new SqlConnection(cadenaConexion);

        return conexion;
    }
}
