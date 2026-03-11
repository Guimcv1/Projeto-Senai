using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace gerenciador_chaves.Back.Data
{
    //Classe que vai gerenciar a conexão do banco
    public class BancoContext : DbContext
    {
        //Mapeia as tabelas para o projeto
        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<Itens> Intens { get; set; }

        //Configuração da conexão com o banco de dados
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Evitar que o C# fica configurando varias vezes a conexão com o db
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    //Carrega as variáveis de ambiente
                    Env.Load();

                    /*Atribui os valores das variáveis de ambiente
                     casso eles sejam null ou não exista ele vai add string.Empty"(null de str)"*/
                    string? Host = Environment.GetEnvironmentVariable("DB_HOST");
                    string? Port = Environment.GetEnvironmentVariable("DB_PORT");
                    string? User = Environment.GetEnvironmentVariable("DB_USER");
                    string? Senha = Environment.GetEnvironmentVariable("DB_SENHA");
                    string? Database = Environment.GetEnvironmentVariable("DB_NAME");

                    /*Verifica se as variáveis de ambiente estão definidas
                        || -> or*/
                    if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(Port) ||
                        string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Senha) ||
                        string.IsNullOrEmpty(Database))
                    {
                        //Mostra quais variáveis estão faltando
                        var faltando = new System.Collections.Generic.List<string>();
                        if (string.IsNullOrEmpty(Host)) faltando.Add("DB_HOST");
                        if (string.IsNullOrEmpty(Port)) faltando.Add("DB_PORT");
                        if (string.IsNullOrEmpty(User)) faltando.Add("DB_USER");
                        if (string.IsNullOrEmpty(Senha)) faltando.Add("DB_SENHA");
                        if (string.IsNullOrEmpty(Database)) faltando.Add("DB_NAME");

                        string mensagem = $"Variáveis de ambiente faltando no arquivo .env: {string.Join(", ", faltando)}";
                        Console.WriteLine($"Erro: {mensagem}");

                        //Lança uma exceção se alguma variável de ambiente estiver faltando
                        throw new Exception(mensagem);
                    }

                    //Monta a string de conexão
                    string connectionString = $"Host={Host};Port={Port};Username={User};Password={Senha};Database={Database}";

                    //Configura o PostgreSQL
                    optionsBuilder.UseNpgsql(connectionString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao configurar conexão com banco: {ex.Message}");
                    throw;
                }
            }
        }
    }
}