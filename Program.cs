
using Backend = gerenciador_chaves.Back.Services.UsuarioService;

using AdminServ = gerenciador_chaves.Back.Services.AdminService;

using Migration = gerenciador_chaves.Back.Services.Migration;

using Execel = gerenciador_chaves.Back.Execel.ExportToExecel;

using gerenciador_chaves.Back.Data;

using System.Reflection;
using System.IO;

namespace gerenciador_chaves
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Cria apas chamadas dll e pega a ponta onde ela está
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll");
                // Aqui usamos o nome completo para não dar erro:
                string assemblyName = new System.Reflection.AssemblyName(resolveArgs.Name).Name + ".dll";
                string assemblyPath = Path.Combine(folderPath, assemblyName);

                if (File.Exists(assemblyPath))
                {
                    return System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
                return null;
            };

            // Testa a conexão com o banco
            if (!Migration.TestarConexao())
            {
                Console.WriteLine("Não foi possível conectar ao banco. Verifique o arquivo .env");
                return;
            }
            
            // Garante que as tabelas existam
            if (!Migration.GarantirBancoCriado())
            {
                Console.WriteLine("Não foi possível criar/atualizar as tabelas no banco.");
                return;
            }

            Console.WriteLine("Conexão estabelecida com sucesso!\n");

            // Loop principal do menu
            bool sair = false;
            while (!sair)
            {
                MostrarMenuPrincipal();
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        MenuUsuarios();
                        break;
                    case "2":
                        MenuItens();
                        break;
                    case "3":
                        MenuExportar();
                        break;
                    case "0":
                        sair = true;
                        Console.WriteLine("\nEncerrando o sistema...");
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida!");
                        break;
                }
            }
        }

        static void MostrarMenuPrincipal()
        {
            Console.WriteLine("\n=== MENU PRINCIPAL ===");
            Console.WriteLine("1 - Gerenciar Usuários");
            Console.WriteLine("2 - Gerenciar Itens/Chaves");
            Console.WriteLine("3 - Exportar para Excel");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");
        }

        #region Menu Usuários
        static void MenuUsuarios()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine("\n=== GERENCIAR USUÁRIOS ===");
                Console.WriteLine("1 - Criar Usuário");
                Console.WriteLine("2 - Listar Usuários");
                Console.WriteLine("3 - Buscar Usuário por ID");
                Console.WriteLine("4 - Atualizar Usuário");
                Console.WriteLine("5 - Deletar Usuário");
                Console.WriteLine("6 - Testar Login");
                Console.WriteLine("7 - Verificar se Usuário é Admin");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha uma opção: ");

                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CriarUsuario();
                        break;
                    case "2":
                        ListarUsuarios();
                        break;
                    case "3":
                        BuscarUsuarioPorId();
                        break;
                    case "4":
                        AtualizarUsuario();
                        break;
                    case "5":
                        DeletarUsuario();
                        break;
                    case "6":
                        TestarLogin();
                        break;
                    case "7":
                        VerificarAdmin();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida!");
                        break;
                }
            }
        }

        static void CriarUsuario()
        {
            Console.WriteLine("\n--- CRIAR USUÁRIO ---");

            Console.Write("Nome: ");
            string? nome = Console.ReadLine();

            Console.Write("Login: ");
            string? login = Console.ReadLine();

            Console.Write("Senha: ");
            string? senha = Console.ReadLine();

            Console.Write("É Admin? (s/n): ");
            bool isAdmin = Console.ReadLine()?.ToLower() == "s";

            Console.Write("Está Ativo? (s/n): ");
            bool isAtivo = Console.ReadLine()?.ToLower() == "s";

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
            {
                Console.WriteLine("\nErro: Todos os campos são obrigatórios!");
                return;
            }

            Backend.CriarUser(nome, login, senha, isAdmin, isAtivo);
        }

        static void ListarUsuarios()
        {
            Console.WriteLine("\n--- LISTA DE USUÁRIOS ---");
            List<Usuario> usuarios = Backend.ListarUser();

            if (usuarios.Count == 0)
            {
                Console.WriteLine("Nenhum usuário encontrado.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"Nome",-20} {"Login",-20} {"Admin",-8} {"Ativo",-8}");
            Console.WriteLine(new string('-', 65));

            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"{usuario.Id,-5} {usuario.Nome,-20} {usuario.Login,-20} {(usuario.IsAdmin ? "Sim" : "Não"),-8} {(usuario.IsAtivo ? "Sim" : "Não"),-8}");
            }
        }

        static void BuscarUsuarioPorId()
        {
            Console.WriteLine("\n--- BUSCAR USUÁRIO ---");
            Console.Write("Digite o ID do usuário: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var usuario = Backend.BuscarPorIdUser(id);

            if (usuario != null)
            {
                Console.WriteLine($"\nID: {usuario.Id}");
                Console.WriteLine($"Nome: {usuario.Nome}");
                Console.WriteLine($"Login: {usuario.Login}");
                Console.WriteLine($"Admin: {(usuario.IsAdmin ? "Sim" : "Não")}");
                Console.WriteLine($"Ativo: {(usuario.IsAtivo ? "Sim" : "Não")}");
            }
        }

        static void AtualizarUsuario()
        {
            Console.WriteLine("\n--- ATUALIZAR USUÁRIO ---");
            Console.Write("Digite o ID do usuário: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            Console.WriteLine("\nDeixe em branco para não alterar o campo.");

            Console.Write("Novo Nome: ");
            string? nome = Console.ReadLine();

            Console.Write("Novo Login: ");
            string? login = Console.ReadLine();

            Console.Write("Nova Senha: ");
            string? senha = Console.ReadLine();

            Console.Write("É Admin? (s/n/deixe em branco): ");
            string? adminInput = Console.ReadLine();
            bool? isAdmin = string.IsNullOrEmpty(adminInput) ? null : adminInput.ToLower() == "s";

            Console.Write("Está Ativo? (s/n/deixe em branco): ");
            string? ativoInput = Console.ReadLine();
            bool? isAtivo = string.IsNullOrEmpty(ativoInput) ? null : ativoInput.ToLower() == "s";

            Backend.AtualizarUser(id, 
                string.IsNullOrEmpty(nome) ? null : nome,
                string.IsNullOrEmpty(login) ? null : login,
                string.IsNullOrEmpty(senha) ? null : senha,
                isAdmin,
                isAtivo);
        }

        static void DeletarUsuario()
        {
            Console.WriteLine("\n--- DELETAR USUÁRIO ---");
            Console.Write("Digite o ID do usuário: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            Console.Write($"Tem certeza que deseja deletar o usuário ID {id}? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Backend.DeletarUser(id);
            }
            else
            {
                Console.WriteLine("Operação cancelada.");
            }
        }

        static void TestarLogin()
        {
            Console.WriteLine("\n--- TESTAR LOGIN ---");

            Console.Write("Login: ");
            string? login = Console.ReadLine();

            Console.Write("Senha: ");
            string? senha = Console.ReadLine();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
            {
                Console.WriteLine("\nErro: Login e senha são obrigatórios!");
                return;
            }

            var usuario = Backend.LoginUser(login, senha);

            if (usuario != null)
            {
                Console.WriteLine($"\n✓ Login realizado com sucesso!");
                Console.WriteLine($"Bem-vindo(a), {usuario.Nome}!");
                Console.WriteLine($"Privilégios: {(usuario.IsAdmin ? "Administrador" : "Usuário comum")}");
            }
        }

        static void VerificarAdmin()
        {
            Console.WriteLine("\n--- VERIFICAR SE USUÁRIO É ADMIN ---");
            Console.Write("Digite o ID do usuário: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var usuario = Backend.BuscarPorIdUser(id);

            if (usuario != null)
            {
                bool ehAdmin = AdminServ.IsAdmin(usuario);
                Console.WriteLine($"\nUsuário: {usuario.Nome}");
                Console.WriteLine($"É Administrador? {(ehAdmin ? "✓ SIM" : "✗ NÃO")}");
            }
        }
        #endregion

        #region Menu Itens
        static void MenuItens()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine("\n=== GERENCIAR ITENS/CHAVES ===");
                Console.WriteLine("1 - Criar Item");
                Console.WriteLine("2 - Listar Itens");
                Console.WriteLine("3 - Buscar Item por ID");
                Console.WriteLine("4 - Editar Item");
                Console.WriteLine("5 - Inativar Item");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha uma opção: ");

                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CriarItem();
                        break;
                    case "2":
                        ListarItens();
                        break;
                    case "3":
                        BuscarItemPorId();
                        break;
                    case "4":
                        EditarItem();
                        break;
                    case "5":
                        InativarItem();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida!");
                        break;
                }
            }
        }

        static void CriarItem()
        {
            Console.WriteLine("\n--- CRIAR ITEM ---");

            Console.Write("Descrição: ");
            string? descricao = Console.ReadLine();

            if (string.IsNullOrEmpty(descricao))
            {
                Console.WriteLine("\nErro: Descrição é obrigatória!");
                return;
            }

            AdminServ.CriarInten(descricao);
        }

        static void ListarItens()
        {
            Console.WriteLine("\n--- LISTA DE ITENS ---");
            List<Itens> itens = AdminServ.ListarIntens();

            if (itens.Count == 0)
            {
                Console.WriteLine("Nenhum item encontrado.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"Descrição",-40} {"Estado",-15}");
            Console.WriteLine(new string('-', 65));

            foreach (var item in itens)
            {
                Console.WriteLine($"{item.Id,-5} {item.Descricao,-40} {item.Estado,-15}");
            }
        }

        static void BuscarItemPorId()
        {
            Console.WriteLine("\n--- BUSCAR ITEM ---");
            Console.Write("Digite o ID do item: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var item = AdminServ.BuscarPorIdIntens(id);

            if (item != null)
            {
                Console.WriteLine($"\nID: {item.Id}");
                Console.WriteLine($"Descrição: {item.Descricao}");
                Console.WriteLine($"Estado: {item.Estado}");
            }
        }

        static void EditarItem()
        {
            Console.WriteLine("\n--- EDITAR ITEM ---");
            Console.Write("Digite o ID do item: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            Console.WriteLine("\nDeixe em branco para não alterar o campo.");

            Console.Write("Nova Descrição: ");
            string? descricao = Console.ReadLine();

            Console.WriteLine("\nEstados disponíveis:");
            Console.WriteLine("1 - Livre");
            Console.WriteLine("2 - Analise");
            Console.WriteLine("3 - Emprestado");
            Console.WriteLine("4 - Inativo");
            Console.Write("Novo Estado (número ou deixe em branco): ");
            string? estadoInput = Console.ReadLine();

            string? novoEstado = null;
            if (!string.IsNullOrEmpty(estadoInput))
            {
                novoEstado = estadoInput switch
                {
                    "1" => Estados.Livre,
                    "2" => Estados.Analise,
                    "3" => Estados.Emprestado,
                    "4" => Estados.Inativo,
                    _ => null
                };
            }

            AdminServ.EditarInten(id,
                string.IsNullOrEmpty(descricao) ? null : descricao,
                novoEstado);
        }

        static void InativarItem()
        {
            Console.WriteLine("\n--- INATIVAR ITEM ---");
            Console.Write("Digite o ID do item: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            Console.Write($"Tem certeza que deseja inativar o item ID {id}? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                AdminServ.InativaIntens(id);
            }
            else
            {
                Console.WriteLine("Operação cancelada.");
            }
        }
        #endregion

        #region Menu Exportar
        static void MenuExportar()
        {
            Console.WriteLine("\n=== EXPORTAR PARA EXCEL ===");
            Console.WriteLine("1 - Exportar Usuários");
            Console.WriteLine("2 - Exportar Itens");
            Console.WriteLine("3 - Exportar Tudo (Usuários + Itens)");
            Console.WriteLine("0 - Voltar");
            Console.Write("\nEscolha uma opção: ");

            string? opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    ExportarUsuarios();
                    break;
                case "2":
                    ExportarItens();
                    break;
                case "3":
                    ExportarTudo();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("\nOpção inválida!");
                    break;
            }
        }

        static void ExportarUsuarios()
        {
            Console.WriteLine("\n--- EXPORTAR USUÁRIOS PARA EXCEL ---");
            Console.Write("Digite o caminho completo do arquivo (ex: D:\\usuarios.xlsx): ");
            string? caminho = Console.ReadLine();

            if (string.IsNullOrEmpty(caminho))
            {
                Console.WriteLine("\nErro: Caminho é obrigatório!");
                return;
            }

            string resultado = Execel.ExportarUsuariosParaExcel(caminho);
            Console.WriteLine(resultado);

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void ExportarItens()
        {
            Console.WriteLine("\n--- EXPORTAR ITENS PARA EXCEL ---");
            Console.Write("Digite o caminho completo do arquivo (ex: D:\\itens.xlsx): ");
            string? caminho = Console.ReadLine();

            if (string.IsNullOrEmpty(caminho))
            {
                Console.WriteLine("\nErro: Caminho é obrigatório!");
                return;
            }

            string resultado = Execel.ExportarIntensParaExcel(caminho);
            Console.WriteLine(resultado);

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void ExportarTudo()
        {
            Console.WriteLine("\n--- EXPORTAR TUDO PARA EXCEL ---");
            Console.WriteLine("(Será criado um arquivo com 2 abas: Usuários e Itens)");
            Console.Write("Digite o caminho completo do arquivo (ex: D:\\completo.xlsx): ");
            string? caminho = Console.ReadLine();

            if (string.IsNullOrEmpty(caminho))
            {
                Console.WriteLine("\nErro: Caminho é obrigatório!");
                return;
            }

            string resultado = Execel.ExportarTudoParaExcel(caminho);
            Console.WriteLine(resultado);

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
        #endregion
    }
}
