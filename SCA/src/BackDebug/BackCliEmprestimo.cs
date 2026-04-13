using SCA.Back.Data;
using SCA.Back.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliEmprestimo
    {
        public static void MenuEmprestimos()
        {
            Console.WriteLine("\n--- CRUD EMPRÉSTIMOS ---");
            Console.WriteLine("1. Solicitar Empréstimo");
            Console.WriteLine("2. Listar Empréstimos");
            Console.WriteLine("3. Aprovar/Rejeitar SolicitaçÁo");
            Console.WriteLine("4. Solicitar DevoluçÁo");
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("ID Usuário: ");
                if (!int.TryParse(Console.ReadLine(), out int usrId)) return;

                Console.Write("ID Sala: ");
                if (!int.TryParse(Console.ReadLine(), out int sID)) return;

                Console.Write("ID dos Itens (separados por vírgula): ");
                string itensStr = Console.ReadLine();
                var itensIds = new System.Collections.Generic.List<int>();
                if (!string.IsNullOrEmpty(itensStr))
                {
                    foreach (var i in itensStr.Split(','))
                    {
                        if (int.TryParse(i.Trim(), out int idItem))
                            itensIds.Add(idItem);
                    }
                }

                EmprestimosService.SolicitarEmprestimo(usrId, sID, itensIds);
                Console.WriteLine("Comando executado.");
            }
            else if (op == "2")
            {
                var emp = EmprestimosService.ListarEmprestimo();
                foreach (var e in emp)
                    Console.WriteLine($"ID: {e.Id} | UsrId: {e.UsuarioId} | SalaId: {e.SalaId} | Data: {e.DataEstado} | Estado: {e.Estado}");
            }
            else if (op == "3")
            {
                Console.Write("ID Empréstimo: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.Write("Tipo (0: Emprestimo, 1: Devolucao): ");
                    int tipo = int.Parse(Console.ReadLine());
                    Console.Write("Aprovar? (s/n): ");
                    bool aprova = Console.ReadLine()?.ToLower() == "s";
                    EmprestimosService.AprovarSolicitacao(id, tipo == 0 ? EmprestimosService.TipoSolicitacao.Emprestimo : EmprestimosService.TipoSolicitacao.Devolucao, aprova);
                }
            }
            else if (op == "4")
            {
                Console.Write("ID Empréstimo para devoluçÁo: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    EmprestimosService.SolicitarDevolucao(id);
                }
            }
        }

    }
}

