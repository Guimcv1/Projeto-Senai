using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace gerenciador_cahves.Back.Data
{
    //Classe que vai gerenciar a conexão do banco
    public class BancoContext : DbContext
    {
        //Termina de mapear as tabelas para o projeto
        public DbSet<Usuario> Usuarios { get; set; }

        //Configuração da conexão com o banco de dados
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Evitar que o C# fica configurando varias vezes a conexão com o db
            if (!optionsBuilder.IsConfigured)
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
                    //Lança uma exceção se alguma variável de ambiente estiver faltando
                    throw new Exception("As variáveis de ambiente para a conexão com o banco de dados não estão definidas.");
                }

                //Monta a string de conexão
                string connectionString = $"Host={Host};Port={Port};Username={User};Password={Senha};Database={Database}";

                //Configura o PostgreSQL
                optionsBuilder.UseNpgsql(connectionString);

                Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso!");
            }
        }
    }
}