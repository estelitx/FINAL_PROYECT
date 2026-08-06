# Conexion a BIBLIOTECA

## Crear la base de datos

Desde la carpeta del repositorio:

```powershell
sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i ".\Biblioteca.sql"
```

## Ejecutar el proyecto

```powershell
dotnet run --project .\BD_Libreria1
```

La conexión predeterminada usa SQL Server Express local con autenticación de Windows.

Si otro integrante utiliza un servidor diferente, debe definir su propia variable sin modificar el código:

```powershell
$env:BIBLIOTECA_CONNECTION_STRING="Server=SERVIDOR;Database=BIBLIOTECA;Integrated Security=True;TrustServerCertificate=True;"
dotnet run --project .\BD_Libreria1
```

No deben guardarse usuarios ni contraseñas en archivos que se suban a GitHub.
