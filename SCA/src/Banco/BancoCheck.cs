using SCA.Back.Data;
using Microsoft.EntityFrameworkCore;

namespace SCA.Back.Services
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

        //Garantir que as tabelas existam
        public static bool GarantirBancoCriado()
        {
            try
            {
                using var context = new BancoContext();

                Console.WriteLine("Verificando banco de dados...");

                bool criado = context.Database.EnsureCreated();

                if (criado)
                {
                    Console.WriteLine("Banco de dados e tabelas criadas com sucesso!");
                }
                else
                {
                    Console.WriteLine("Banco de dados já existe.");
                }

                GarantirAdminPadrao();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar banco: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");
                return false;
            }
        }

        private static void GarantirAdminPadrao()
        {
            try
            {
                using var context = new BancoContext();

                string loginAdmin = Environment.GetEnvironmentVariable("ADMIN_LOGIN") ?? "admin";
                string senhaAdmin = Environment.GetEnvironmentVariable("ADMIN_SENHA") ?? "admin";

                if (!context.Usuarios.Any(u => u.Login == loginAdmin))
                {
                    SCA.Back.Services.UsuarioService.CriarUser("AdminDefalte", loginAdmin, senhaAdmin, true, true);
                    Console.WriteLine("Usuário admin padrão criado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Usuário admin já existe. Ignorando a criação.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ignorando erro ao tentar criar admin: {ex.Message}");
            }
        }
    } 
}