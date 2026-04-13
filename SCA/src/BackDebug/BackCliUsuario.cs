using SCA.Back.Data;
using SCA.Back.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliUsuario
    {
        public static void MenuUsuario()
        {
            Console.WriteLine("\n--- CRUD USUÁRIOS ---");
            Console.WriteLine("1. Criar");
            Console.WriteLine("2. Listar");
            Console.WriteLine("3. Editar");
            Console.WriteLine("4. Deletar (Inativar/Ativar)");
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("Nome: ");
                string nome = Console.ReadLine();
                Console.Write("Login: ");
                string login = Console.ReadLine();
                Console.Write("Senha: ");
                string senha = Console.ReadLine();
                Console.Write("Á‰ Admin? (s/n): ");
                bool isAdmin = Console.ReadLine()?.ToLower() == "s";

                UsuarioService.CriarUser(nome, login, senha, isAdmin, true);
                Console.WriteLine("Comando executado.");
            }
            else if (op == "2")
            {
                var usuarios = UsuarioService.ListarUser();
                foreach (var u in usuarios)
                    Console.WriteLine($"ID: {u.Id} | Nome: {u.Nome} | Login: {u.Login} | Admin: {u.IsAdmin} | Ativo: {u.IsAtivo}");
            }
            else if (op == "3")
            {
                Console.Write("ID do Usuário a editar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Novo Nome: ");
                    string nome = Console.ReadLine();
                    Console.Write("Novo Login: ");
                    string login = Console.ReadLine();
                    Console.Write("Nova Senha (vazio para nÁo mudar): ");
                    string senha = Console.ReadLine();
                    Console.Write("Novo Admin (s/n): ");
                    bool isAdmin = Console.ReadLine()?.ToLower() == "s";

                    UsuarioService.AtualizarUser(id, nome, login, string.IsNullOrEmpty(senha) ? null : senha, isAdmin, null);
                }
            }
            else if (op == "4")
            {
                Console.Write("ID do Usuário para inativar (ou ativar): ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Novo estado (1 para ativo, 0 para inativo): ");
                    bool isAtivo = Console.ReadLine() == "1";
                    UsuarioService.InativarAtivarUser(id, isAtivo);
                }
            }
        } 
    }
}

