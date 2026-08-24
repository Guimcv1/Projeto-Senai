using Avalonia;
using Avalonia.ReactiveUI;
using System;
using SCA.Core.Services;
using SCA.Core.Data;
using DotNetEnv;

using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace SCA;

class Program
{
    //pega do propio os a função para mostra a caixa de erro
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    private static void ShowErrorBox(string text, string caption)
    {
        //ver se está rodando no windwos
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //gera a caixa de erro, o 0x10 -> é o código de erro na api do windwos
            MessageBox(IntPtr.Zero, text, caption, 0x10);
        }
        else 
        {
            Console.WriteLine($"{caption}: {text}");
        }
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Exibe o console no Windows, caso os parâmetros --console ou -c sejam passados
        bool showConsole = Array.IndexOf(args, "--console") != -1 || Array.IndexOf(args, "-c") != -1;
        if (showConsole && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AllocConsole();
        }

        // Cria pasta chamadas dll e pega a ponta onde ela está
        AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll");
            // Aqui usamos o nome completo para não dar erro:
            string assemblyName = new System.Reflection.AssemblyName(resolveArgs.Name).Name + ".dll";
            string assemblyPath = Path.Combine(folderPath, assemblyName);

            if (File.Exists(assemblyPath))
            {
                return System.Reflection.Assembly.LoadFrom(assemblyPath);
            }
            return null;
        };

        // Chama um método separado para evitar que o compilador JIT tente resolver o DotNetEnv
        // antes de anexar o AssemblyResolve.
        RunApp(args);
    }

    private static void RunApp(string[] args)
    {
        try
        {
            // Load environment variables
            Env.Load();

            

            //Testa a conexão com o banco
            if (!Migration.TestarConexao())
            {
                //Pop de erro
                ShowErrorBox("Não foi possível conectar ao banco. Verifique o arquivo .env", "Erro de Conexão");
                return;
            }

            //Garante que as tabelas existam
            if (!Migration.GarantirBancoCriado())
            {
                ShowErrorBox("Não foi possível criar/atualizar as tabelas no banco.", "Erro no Banco de Dados");
                return;
            }


            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            ShowErrorBox($"Falha catastrófica ao iniciar aplicação: {ex.Message}", "Erro Desconhecido");
            return;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()=> AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
