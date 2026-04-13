using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace SCA.Back.Data
{
    //Classe que vai gerenciar a conexão do banco
    public class BancoContext : DbContext
    {
        //Mapeia as tabelas para o projeto
        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<Itens> Itens { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Emprestimos> Emprestimos { get; set; }
        public DbSet<EmprestimoIntens> EmprestimoIntens { get; set; }
        public DbSet<Logs> Logs { get; set; }

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
                     caso eles sejam null ou não exista ele vai add string.Empty"(null de str)"*/
                    string? Host = Environment.GetEnvironmentVariable("DB_HOST");
                    string? Port = Environment.GetEnvironmentVariable("DB_PORT");
                    string? User = Environment.GetEnvironmentVariable("DB_USER");
                    string? Senha = Environment.GetEnvironmentVariable("DB_SENHA");
                    string? Database = Environment.GetEnvironmentVariable("DB_NAME");
                    string? UserAdminLogin = Environment.GetEnvironmentVariable("USER_ADMIN_LOGIN");
                    string? UserAdminSenha = Environment.GetEnvironmentVariable("USER_ADMIN_SENHA");

                    /*Verifica se as variáveis de ambiente estão definidas*/
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
                        if (string.IsNullOrEmpty(Database)) faltando.Add("USER_ADMIN_LOGIN");
                        if (string.IsNullOrEmpty(Database)) faltando.Add("USER_ADMIN_SENHA");

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

        //Configuração das relações entre as tabelas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Relacionamento 1:N (Um Usuário para Muitos Empréstimos)
            modelBuilder.Entity<Emprestimos>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Emprestimos)
                .HasForeignKey(m => m.UsuarioId);

            //Relacionamento 1:N (Uma Sala para Muitos Empréstimos)
            modelBuilder.Entity<Emprestimos>()
                .HasOne(m => m.Sala)
                .WithMany(s => s.Emprestimos)
                .HasForeignKey(m => m.SalaId);

            //Relacionamento 1:N (Um Empréstimo para Muitos Itens na lista de itens do empréstimo)
            //Parte da relação N:N entre Emprestimos e Itens
            modelBuilder.Entity<EmprestimoIntens>()
                .HasOne(ei => ei.Emprestimos)
                .WithMany(e => e.EmprestimoIntens)
                .HasForeignKey(ei => ei.EmprestimoId);

            //Relacionamento 1:N (Um Item para Muitos registros na tabela de junção EmprestimoIntens)
            //Parte da relação N:N entre Emprestimos e Itens
            modelBuilder.Entity<EmprestimoIntens>()
                .HasOne(ei => ei.Itens)
                .WithMany(i => i.EmprestimoIntens)
                .HasForeignKey(ei => ei.ItemId);

            //Relacionamento 1:N (Um Usuário para Muitos Logs)
            modelBuilder.Entity<Logs>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UsuarioId);

            base.OnModelCreating(modelBuilder);
        }
    }
}