using SCA.Back.Data;
using SCA.Back.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliIten
    {
        public static void MenuItens()
        {
            Console.WriteLine("\n--- CRUD ITENS ---");
            Console.WriteLine("1. Criar");
            Console.WriteLine("2. Listar");
            Console.WriteLine("3. Editar");
            Console.WriteLine("4. Deletar (Inativar)");
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("Descricao: ");
                string desc = Console.ReadLine();
                AdminService.CriarIntens(desc);
                Console.WriteLine("Comando executado.");
            }
            else if (op == "2")
            {
                var itens = AdminService.ListarIntens();
                foreach (var i in itens)
                    Console.WriteLine($"ID: {i.Id} | Desc: {i.Descricao} | Estado: {i.Estado} | Ativo: {i.IsAtivo}");
            }
            else if (op == "3")
            {
                Console.Write("ID a editar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Nova DescriçÁo: ");
                    string desc = Console.ReadLine();
                    Console.Write("Novo Estado (vazio para nÁo mudar): ");
                    string estado = Console.ReadLine();
                    AdminService.EditarIntens(id, desc, string.IsNullOrEmpty(estado) ? null : estado);
                }
            }
            else if (op == "4")
            {
                Console.Write("ID para inativar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    AdminService.InativaIntens(id);
                }
            }
        }
    }
}

