namespace SCA
{
    using SCA.Back.Services;
    using SCA.Back.Data;
    using _ = SCA.Back.Debug.BackCliDebug;
    using System;
    using System.Reflection;
    using System.IO;

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Cria a pasta chamada dll e pega e a ponta onde ela está para salvar as dll's do sistemas
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll");
                //Aqui usamos o nome completo para não dar erro
                string assemblyName = new System.Reflection.AssemblyName(resolveArgs.Name).Name + ".dll";
                string assemblyPath = Path.Combine(folderPath, assemblyName);

                if (File.Exists(assemblyPath))
                {
                    //Aponta para o .exe oonde fica estão as dlls
                    return System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
                return null;
            };

            //Testa a conexão com o banco
            if (!Migration.TestarConexao())
            {
                //Pop de erro
                System.Windows.MessageBox.Show("Não foi possível conectar ao banco. Verifique o arquivo .env", 
                    "Erro de Conexão",
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            //Garante que as tabelas existam
            if (!Migration.GarantirBancoCriado())
            {
                System.Windows.MessageBox.Show("Não foi possível criar/atualizar as tabelas no banco.", 
                    "Erro no Banco de Dados", 
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            Console.WriteLine("Conexão estabelecida com sucesso!\n");

            _.MenuPrincipal();

             
            /*Start interface
            SCA.App app = new SCA.App();
            app.InitializeComponent();
            app.Run();*/
             
        }   
    }

}

