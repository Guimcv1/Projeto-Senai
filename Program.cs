
using Backend = gerenciador_chaves.Back.Services.UsuarioService;
using Migration = gerenciador_chaves.Back.Services.Migration;
using Execel = gerenciador_chaves.Back.Execel.ExportToExecel;

namespace gerenciador_chaves
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Gerenciador de Usuários");
             
           
            // Testa a conexão com o banco
            if (!Migration.TestarConexao())
            {
                Console.WriteLine("Não foi possível conectar ao banco. Verifique o arquivo .env");
                Console.ReadKey();
                return;
            }
            
            // Que as tabelas existam (aplica migrations se necessário)
            if (!Migration.GarantirBancoCriado())
            {
                Console.WriteLine("Não foi possível criar/atualizar as tabelas no banco.");
                Console.ReadKey();
                return;
            }
       

            Backend.CriarUser("test2", "test2", "123", true, true);
            // O @ -> faz que o C# considere tudo entre aspas(") str já de cara.
            Execel.ExportarUsuariosParaExcel(@"D:\n.xlsx");
            Console.ReadKey();
        }
    }
}
