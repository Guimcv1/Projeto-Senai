using SCA.Core.Models;
using SCA.Core.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliSala
    {
        public static void MenuSalas()
        {
            Console.WriteLine("\n--- CRUD SALAS ---");
            Console.WriteLine("1. Criar");
            Console.WriteLine("2. Listar");
            Console.WriteLine("3. Editar");
            Console.WriteLine("4. Deletar (Inativar)");
            string op = Console.ReadLine() ?? "";

            if (op == "1")
            {
                Console.Write("Nome/Descrição da Sala: ");
                string nome = Console.ReadLine() ?? "";
                SalaService.CriarSala(nome);
                Console.WriteLine("Comando executado.");
            }
            else if (op == "2")
            {
                var salas = SalaService.ListarSala();
                foreach (var s in salas)
                    Console.WriteLine($"ID: {s.Id} | Descrição: {s.Descricao} | Ativo: {s.isAtivo}");
            }
            else if (op == "3")
            {
                Console.Write("ID da Sala a editar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Nova Descrição: ");
                    string nome = Console.ReadLine() ?? "";
                    SalaService.EditarSala(id, nome);
                }
            }
            else if (op == "4")
            {
                Console.Write("ID da sala para inativar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    SalaService.InativaSala(id);
                }
            }
        }
    }
}
