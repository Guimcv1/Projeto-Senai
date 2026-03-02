using gerenciador_cahves.Back.Data;
using Microsoft.EntityFrameworkCore;

//Isso seria a mesma coisa de "import bibliotecas as outronome" do Python
using Has = BCrypt.Net.BCrypt;

namespace gerenciador_cahves.Back.Services
{
    public class UsuarioService
    {
        //CriarUser - Criar novo usuário
        public static bool CriarUser(string nome, string login, string senha, bool isAdmin = false, bool isAtivo = true)
        {
            try
            {
                //Declarando um objeto para acessar a db
                using var context = new BancoContext();

                //Verifica se o login já existe
                if (context.Usuarios.Any(u => u.Login == login))
                {
                    Console.WriteLine($"Erro: Login '{login}' já existe.");
                    return false;
                }

                //Criptografa a senha com BCrypt
                string senhaHash = Has.HashPassword(senha);

                var usuario = new Usuario
                {
                    Nome = nome,
                    Login = login,
                    Senha = senhaHash,
                    IsAdmin = isAdmin,
                    IsAtivo = isAtivo
                };

                context.Usuarios.Add(usuario);
                context.SaveChanges();

                Console.WriteLine($"Usuário '{nome}' criado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar usuário: {ex.Message}");
                return false;
            }
        }

        //ListarUser - Buscar todos os usuários (sem mostrar senhas)
        public static List<Usuario> ListarUser()
        {
            try
            {
                using var context = new BancoContext();
                return context.Usuarios.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar usuários: {ex.Message}");
                return new List<Usuario>();
            }
        }

        //BuscarPorId - Buscar usuário por ID
        public static Usuario? BuscarPorId(int id)
        {
            try
            {
                using var context = new BancoContext();
                return context.Usuarios.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar usuário: {ex.Message}");
                return null;
            }
        }

        //AtualizarUser - Atualizar usuário
        public static bool AtualizarUser(int id, string? novoNome = null, string? novoLogin = null, string? novaSenha = null, bool? isAdmin = null, bool? isAtivo = null)
        {
            try
            {
                using var context = new BancoContext();
                var usuario = context.Usuarios.Find(id);

                if (usuario == null)
                {
                    Console.WriteLine($"Erro: Usuário com ID {id} não encontrado.");
                    return false;
                }

                //Atualiza o nome se foi fornecido
                if (!string.IsNullOrEmpty(novoNome))
                {
                    usuario.Nome = novoNome;
                }

                //Atualiza o login se foi fornecido
                if (!string.IsNullOrEmpty(novoLogin))
                {
                    //Verifica se o novo login já existe
                    if (context.Usuarios.Any(u => u.Login == novoLogin && u.Id != id))
                    {
                        Console.WriteLine($"Erro: Login '{novoLogin}' já existe.");
                        return false;
                    }
                    usuario.Login = novoLogin;
                }

                //Atualiza a senha se foi fornecida (criptografando novamente)
                if (!string.IsNullOrEmpty(novaSenha))
                {
                    usuario.Senha = Has.HashPassword(novaSenha);
                }

                //Atualiza IsAdmin se foi fornecido
                if (isAdmin.HasValue)
                {
                    usuario.IsAdmin = isAdmin.Value;
                }

                //Atualiza IsAtivo se foi fornecido
                if (isAtivo.HasValue)
                {
                    usuario.IsAtivo = isAtivo.Value;
                }

                context.SaveChanges();
                Console.WriteLine($"Usuário ID {id} atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar usuário: {ex.Message}");
                return false;
            }
        }

        //DeletarUser - Deletar usuário
        public static bool DeletarUser(int id)
        {
            try
            {
                using var context = new BancoContext();
                var usuario = context.Usuarios.Find(id);

                if (usuario == null)
                {
                    Console.WriteLine($"Erro: Usuário com ID {id} não encontrado.");
                    return false;
                }

                context.Usuarios.Remove(usuario);
                context.SaveChanges();

                Console.WriteLine($"Usuário ID {id} deletado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar usuário: {ex.Message}");
                return false;
            }
        }

        //Login - Autenticar usuário
        public static Usuario? LoginUser(string login, string senha)
        {
            try
            {
                using var context = new BancoContext();
                var usuario = context.Usuarios.FirstOrDefault(u => u.Login == login);

                if (usuario == null)
                {
                    Console.WriteLine("Erro: Usuário não encontrado.");
                    return null;
                }

                //Verifica se o usuário está ativo
                if (!usuario.IsAtivo)
                {
                    Console.WriteLine("Erro: Usuário inativo.");
                    return null;
                }

                //Verifica se a senha está correta usando BCrypt
                if (Has.Verify(senha, usuario.Senha))
                {
                    Console.WriteLine($"Login bem-sucedido! Bem-vindo, {usuario.Nome}!");
                    return usuario;
                }
                else
                {
                    Console.WriteLine("Erro: Senha incorreta.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao autenticar: {ex.Message}");
                return null;
            }
        }

        //Testar conexão com o banco
        public static bool TestarConexao()
        {
            try
            {
                using var context = new BancoContext();
                context.Database.CanConnect();
                Console.WriteLine("Conexão com o banco estabelecida!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco: {ex.Message}");
                return false;
            }
        }
    }
}
