using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Back.Execel
{ 
    public class ExportadorLogs
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportarExecel.TipoExeport tipo, DateTime? inicio, DateTime? fim)
        {
            //Verifica se o tipo solicitado envolve logs
            if (tipo == ExportarExecel.TipoExeport.Tudo || tipo == ExportarExecel.TipoExeport.TodosLogs ||
                tipo == ExportarExecel.TipoExeport.LogsIntens || tipo == ExportarExecel.TipoExeport.LogsSala || tipo == ExportarExecel.TipoExeport.LogsUsuario)
            {
  
                var nomeAba = tipo switch
                {
                    ExportarExecel.TipoExeport.LogsIntens => "Logs Itens",
                    ExportarExecel.TipoExeport.LogsSala => "Logs Salas",
                    ExportarExecel.TipoExeport.LogsUsuario => "Logs Usuarios",
                    _ => "Logs Gerais"
                };

                var worksheet = workbook.Worksheets.Add(nomeAba);

                var logs = LogsService.FiltrarLogs(tipo, inicio, fim);

                //Define os cabeçalhos na primeira linha
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Ação";
                worksheet.Cell(1, 3).Value = "Tipo";
                worksheet.Cell(1, 4).Value = "Usuário";
                worksheet.Cell(1, 5).Value = "Data";

                //Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var log in logs)
                {
                    //Prencher as linha com base nos dados banco
                    worksheet.Cell(linha, 1).Value = log.Id;
                    worksheet.Cell(linha, 2).Value = log.Acao;
                    worksheet.Cell(linha, 3).Value = log.TipoAcao.ToString();
                    worksheet.Cell(linha, 4).Value = log.Usuario?.Nome ?? "N/A";
                    worksheet.Cell(linha, 5).Value = log.DataAcao;
                    linha++;
                }

                //Ajusta a largura das colunas automaticamente
                worksheet.Columns().AdjustToContents();
            }
        }
    }
}