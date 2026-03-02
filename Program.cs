using gerenciador_cahves.Back.Services;


namespace gerenciador_cahves
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HELLO WORLD \n");
            Console.WriteLine("Gerenciador de Usuários");

            //Testa a conexão com o banco
            if (!UsuarioService.TestarConexao())
            {
                Console.WriteLine("Não foi possível conectar ao banco. Verifique o arquivo .env");
                return;
            }

        }
    }
}
