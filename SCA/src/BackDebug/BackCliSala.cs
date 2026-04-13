using SCA.Back.Data;
using SCA.Back.Services;
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
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("Nome/DescriçÁo da Sala: ");
                string nome = Console.ReadLine();
                SalasService.CriarSala(nome);
                Console.WriteLine("Comando executado.");
            }
            else if (op == "2")
            {
                var salas = SalasService.ListarSala();
                foreach (var s in salas)
                    Console.WriteLine($"ID: {s.Id} | DescriçÁo: {s.Descricao} | Ativo: {s.isAtivo}");
            }
            else if (op == "3")
            {
                Console.Write("ID da Sala a editar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Nova DescriçÁo: ");
                    string nome = Console.ReadLine();
                    Console.Write("Novo Estado (vazio para nÁo mudar): ");
                    string estado = Console.ReadLine();
                    SalasService.EditarSala(id, nome, string.IsNullOrEmpty(estado) ? null : estado);
                }
            }
            else if (op == "4")
            {
                Console.Write("ID da sala para inativar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    SalasService.InativaSala(id);
                }
            }
        }
    }
}

