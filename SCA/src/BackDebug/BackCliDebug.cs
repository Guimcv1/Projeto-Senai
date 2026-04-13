using SCA.Back.Data;
using SCA.Back.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliDebug
    {
        public static void MenuPrincipal()
        {
            Console.WriteLine("====== Módulo de Debug do Backend (CRUD) ======");
            bool sair = false;
            while (!sair)
            {
                Console.WriteLine("\nEscolha a entidade:");
                Console.WriteLine("1 - Usuários");
                Console.WriteLine("2 - Salas");
                Console.WriteLine("3 - Itens");
                Console.WriteLine("4 - Empréstimos"); 
                Console.WriteLine("5 - Exportar para Excel");
                Console.WriteLine("0 - Sair");
                Console.Write("OpçÁo: ");
                
                string opcao = Console.ReadLine();
                switch (opcao)
                {
                    case "1":
                        BackCliUsuario.MenuUsuario();
                        break;
                    case "2":
                        BackCliSala.MenuSalas();
                        break;
                    case "3":
                        BackCliIten.MenuItens();
                        break;
                    case "4":
                        BackCliEmprestimo.MenuEmprestimos();
                        break;
                    case "5": 
                        BackCliExporta.MenuExportar();
                        break;
                    case "0":
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }
    }
}

