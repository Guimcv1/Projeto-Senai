using SCA.Back.Data;
using SCA.Back.Services;
using System;

namespace SCA.Back.Debug
{
    public static class BackCliExporta
    {
        public static void MenuExportar()
        {
            Console.WriteLine("\n--- EXPORTAR PARA EXCEL ---");
            Console.Write("Caminho/Nome do Arquivo (ex: relatorio.xlsx): ");
            string arquivo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(arquivo))
            {
                Console.WriteLine("Caminho inválido!");
                return;
            }

            Console.WriteLine("Tipo de ExportaçÁo (Digite o número correspondente):");
            Console.WriteLine("0 - Tudo");
            Console.WriteLine("1 - Itens");
            Console.WriteLine("2 - Usuarios");
            Console.WriteLine("3 - Salas");
            Console.WriteLine("4 - Emprestimos");
            Console.WriteLine("5 - Todos logs");
            Console.WriteLine("6 - Logs de Itens");
            Console.WriteLine("7 - Logs de Salas");
            Console.WriteLine("8 - Logs de Usuarios");
            Console.WriteLine("9 - EmprestimoItens");
            Console.Write("OpçÁo: " );

            if (int.TryParse(Console.ReadLine(), out int opIndex) && opIndex >= 0 && opIndex <= 9)
            {
                var tipo = (SCA.Back.Execel.ExportarExecel.TipoExeport)opIndex;
                string resultado = SCA.Back.Execel.ExportarExecel.ExportarParaExcel(arquivo, tipo);
                Console.WriteLine(resultado);
            }
            else
            {
                Console.WriteLine("OpçÁo inválida para tipo.");
            }
        }

    }
}

