using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using SCA.Core.Models;
using SCA.Core.Services;

namespace SCA.Back.Execel
{ 
    public class ExportadorLogs
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportacaoExcel.TipoExportacao tipo, DateTime? inicio, DateTime? fim)
        {
            //Verifica se o tipo solicitado envolve logs
            if (tipo == ExportacaoExcel.TipoExportacao.Tudo || tipo == ExportacaoExcel.TipoExportacao.TodosLogs ||
                tipo == ExportacaoExcel.TipoExportacao.LogsIntens || tipo == ExportacaoExcel.TipoExportacao.LogsSala || tipo == ExportacaoExcel.TipoExportacao.LogsUsuario)
            {
  
                var nomeAba = tipo switch
                {
                    ExportacaoExcel.TipoExportacao.LogsIntens => "Log Item",
                    ExportacaoExcel.TipoExportacao.LogsSala => "Log Sala",
                    ExportacaoExcel.TipoExportacao.LogsUsuario => "Log Usuarios",
                    _ => "Log Gerais"
                };

                var worksheet = workbook.Worksheets.Add(nomeAba);

                var logs = LogService.FiltrarLogs(tipo, inicio, fim);

                //Define os cabeçalhos na primeira linha
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Solicitante";
                worksheet.Cell(1, 3).Value = "Executor";
                worksheet.Cell(1, 4).Value = "Hora";
                worksheet.Cell(1, 5).Value = "Item Da Que Sofreu a Ação";

                //Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var log in logs)
                {
                    var solicitante = LogService.ExtrairCampo(log.Acao, "solicitante") ?? string.Empty;
                    var alvo = LogService.ExtrairCampo(log.Acao, "alvo") ?? string.Empty;

                    //Prencher as linha com base nos dados banco
                    worksheet.Cell(linha, 1).Value = log.Id;
                    worksheet.Cell(linha, 2).Value = solicitante;
                    worksheet.Cell(linha, 3).Value = log.Usuario?.Nome ?? "N/A";
                    worksheet.Cell(linha, 4).Value = log.DataAcao.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cell(linha, 5).Value = alvo;
                    linha++;
                }

                //Ajusta a largura das colunas automaticamente
                worksheet.Columns().AdjustToContents();
            }
        }
    }
}