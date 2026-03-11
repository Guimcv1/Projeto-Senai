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

                //Verifica se o banco de dados existe, se não, cria
                Console.WriteLine("Verificando banco de dados...");

                //Tenta aplicar todas as migrations pendentes (cria o banco e tabelas se não existirem)
                var migrationsPendentes = context.Database.GetPendingMigrations().ToList();

                if (migrationsPendentes.Any())
                {
                    Console.WriteLine($"Encontradas {migrationsPendentes.Count} migrations pendentes.");
                    Console.WriteLine("Aplicando migrations ao banco de dados...");

                    foreach (var migration in migrationsPendentes)
                    {
                        Console.WriteLine($"  - {migration}");
                    }

                    context.Database.Migrate();
                    Console.WriteLine(" Migrations aplicadas com sucesso!");
                    Console.WriteLine(" Tabelas criadas: usuarios, itens");
                }
                else
                {
                    Console.WriteLine(" Banco de dados já está atualizado.");
                    Console.WriteLine(" Tabelas disponíveis: usuarios, itens");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Erro ao aplicar migrations: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");

                //Tenta criar o banco usando EnsureCreated como fallback
                try
                {
                    Console.WriteLine("\n→ Tentando criar banco e tabelas diretamente...");
                    using var context = new BancoContext();
                    bool criado = context.Database.EnsureCreated();

                    if (criado)
                    {
                        Console.WriteLine(" Banco de dados e tabelas criadas com sucesso!");
                        Console.WriteLine("Tabelas criadas: usuarios, itens");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(" Banco de dados e tabelas já existiam.");
                        return true;
                    }
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"\n Erro ao criar banco: {ex2.Message}");
                    Console.WriteLine($"Detalhes: {ex2.InnerException?.Message}");
                    return false;
                }
            }
        }
    } 
}