using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Back.Execel
{
    public class ExportadorEmprestimos
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportarExecel.TipoExeport tipo, DateTime? inicio, DateTime? fim)
        {
            //Verifica se o tipo solicitado envolve empréstimos ou se é para exportar tudo
            if (tipo == ExportarExecel.TipoExeport.Tudo || tipo == ExportarExecel.TipoExeport.Empresitmos)
            {
                //Configuração inicial da aba
                var worksheet = workbook.Worksheets.Add("Lista de Emprestimos");
                var emprestimos = EmprestimosService.FiltrarEmprestimo(tipo, inicio, fim);

                //Configuração dos cabeçalhos das colunas
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Usuário";
                worksheet.Cell(1, 3).Value = "Sala";
                worksheet.Cell(1, 4).Value = "Estado";
                worksheet.Cell(1, 5).Value = "Data Estado";

                //Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var i in emprestimos)
                {   
                    //Preenche as linhas com base nos dados do banco 
                    worksheet.Cell(linha, 1).Value = i.Id;
                    worksheet.Cell(linha, 2).Value = i.Usuario?.Nome ?? "";
                    worksheet.Cell(linha, 3).Value = i.Sala?.Descricao ?? "";
                    worksheet.Cell(linha, 4).Value = i.Estado;
                    worksheet.Cell(linha, 5).Value = i.DataEstado;
                    linha++;
                }

                //Ajusta a largura das colunas automaticamente
                worksheet.Columns().AdjustToContents();
            }
        }
    }
}
