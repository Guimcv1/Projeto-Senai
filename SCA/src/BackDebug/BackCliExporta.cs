using SCA.Core.Models;
using SCA.Core.Services;
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
            Console.WriteLine("1 - Item");
            Console.WriteLine("2 - Usuarios");
            Console.WriteLine("3 - Sala");
            Console.WriteLine("4 - Emprestimos");
            Console.WriteLine("5 - Todos logs");
            Console.WriteLine("6 - Log de Item");
            Console.WriteLine("7 - Log de Sala");
            Console.WriteLine("8 - Log de Usuarios");
            Console.WriteLine("9 - EmprestimoItens");
            Console.Write("OpçÁo: " );

            if (int.TryParse(Console.ReadLine(), out int opIndex) && opIndex >= 0 && opIndex <= 9)
            {
                var tipo = (SCA.Back.Execel.ExportacaoExcel.TipoExportacao)opIndex;
                string resultado = SCA.Back.Execel.ExportacaoExcel.ExportarParaExcel(arquivo, tipo);
                Console.WriteLine(resultado);
            }
            else
            {
                Console.WriteLine("OpçÁo inválida para tipo.");
            }
        }

    }
}

