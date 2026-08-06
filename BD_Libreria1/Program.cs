namespace BD_Libreria1;

// Este es el punto donde inicia el programa, básicamente prepara Windows Forms y abre la ventana.
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Aquí se carga la configuración visual que trae .NET para las aplicaciones de Windows.
        ApplicationConfiguration.Initialize();

        // Esta línea mantiene el programa funcionando hasta que se cierre el formulario principal.
        Application.Run(new FormularioPrincipal());
    }
}
