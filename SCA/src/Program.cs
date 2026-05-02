using Avalonia;
using Avalonia.ReactiveUI;
using System;
using SCA.Core.Services;
using SCA.Core.Data;
using DotNetEnv;

namespace SCA;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Load environment variables
            Env.Load();

            // Setup DB
            if (!Migration.TestarConexao())
            {
                Console.WriteLine("Erro crítico: Não foi possível conectar ao banco de dados.");
                // In a real app, we might want to show a native dialog here if possible, 
                // but for now, console is safer during migration.
            }

            if (!Migration.GarantirBancoCriado())
            {
                Console.WriteLine("Erro crítico: Falha ao garantir integridade do banco de dados.");
            }

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha catastrófica ao iniciar aplicação: {ex.Message}");
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
