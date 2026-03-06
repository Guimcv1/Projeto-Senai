using gerenciador_chaves.Back.Data;
using Microsoft.EntityFrameworkCore;

namespace gerenciador_chaves.Back.Services
{
    public class Migration
    {
        //Testar conexão com o banco
        public static bool TestarConexao()
        {
            try
            {
                using var context = new BancoContext();
                bool conectado = context.Database.CanConnect();

                if (conectado == true)
                {
                    Console.WriteLine("Conexão com o banco estabelecida!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Não foi possível conectar ao banco de dados.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");
                return false;
            }
        }

        //Garantir que as tabelas existam - Aplicar migrations pendentes
        public static bool GarantirBancoCriado()
        {
            try
            {
                using var context = new BancoContext();

                //Verifica se há migrations pendentes
                var migrationsPendentes = context.Database.GetPendingMigrations();

                if (migrationsPendentes.Any())
                {
                    Console.WriteLine("Aplicando migrations pendentes ao banco de dados...");
                    context.Database.Migrate();
                    Console.WriteLine("Migrations aplicadas com sucesso!");
                }
                else
                {
                    Console.WriteLine("Banco de dados já está atualizado.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar/atualizar banco: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");
                return false;
            }
        }
    } 
}