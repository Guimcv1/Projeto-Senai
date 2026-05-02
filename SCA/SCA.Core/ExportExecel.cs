using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.IO;

using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Back.Execel
{
    public class ExportarExecel
    {
        //Garanta que não há erro de digitação
        public enum TipoExeport
        {
            Tudo, Intens, Usuario, Sala, Empresitmos,
            TodosLogs, LogsIntens, LogsSala, LogsUsuario, EmprestimoIntens
        }

        public static string ExportarParaExcel(string caminhoArquivo, TipoExeport tipo, DateTime? inicio = null, DateTime? fim = null)
        {
            try
            {
                //Garante que a extensão seja .xlsx para abrir no Excel
                if (!caminhoArquivo.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    caminhoArquivo = Path.ChangeExtension(caminhoArquivo, ".xlsx");
                    Console.WriteLine($"Extensão corrigida para: {caminhoArquivo}");
                }

                using var context = new BancoContext();

                //Busca os dados sem rastreamento (AsNoTracking) para economizar memória RAM
                var intens = context.Itens.AsNoTracking().ToList();
                var usuarios = context.Usuarios.AsNoTracking().ToList();
                var salas = context.Salas.AsNoTracking().ToList();

                //Cria um novo workbook (arquivo Excel)
                using var workbook = new XLWorkbook();

                //Aba de Itens
                ExportadorIntens.AdicionarAba(workbook, tipo, intens);

                //Aba de Usuários
                ExportadorUsuarios.AdicionarAba(workbook, tipo, usuarios);

                //Aba de Sala
                ExportadorSala.AdicionarAba(workbook, tipo, salas);

                //Aba de Emprestimos
                ExportadorEmprestimos.AdicionarAba(workbook, tipo, inicio, fim);

                //Aba de EmprestimoIntens
                ExportadorEmprestimoIntens.AdicionarAba(workbook, tipo);

                //Aba de Logs
                ExportadorLogs.AdicionarAba(workbook, tipo, inicio, fim);

                //Salva o arquivo
                workbook.SaveAs(caminhoArquivo);

                return $"Sucesso! Arquivo salvo em: {Path.GetFullPath(caminhoArquivo)}";
            }
            catch (Exception ex)
            {
                return $"Erro ao exportar: {ex.Message}";
            }
        }
    }
}
