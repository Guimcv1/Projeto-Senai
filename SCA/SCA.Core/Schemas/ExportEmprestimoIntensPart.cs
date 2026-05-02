using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Back.Execel
{
    public class ExportadorEmprestimoIntens
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportarExecel.TipoExeport tipo)
        {
            //Verifica se o tipo solicitado envolve a relação EmprestimoItens ou se é para exportar tudo
            if (tipo == ExportarExecel.TipoExeport.Tudo || tipo == ExportarExecel.TipoExeport.EmprestimoIntens)
            {
                //Criação da aba e busca dos dados no serviço
                var worksheet = workbook.Worksheets.Add("EmprestimoItens");
                var emprestimoItens = EmprestimoIntensService.FiltrarEmprestimoIntens(tipo);

                //Configuração dos cabeçalhos das colunas
                worksheet.Cell(1, 1).Value = "ID da Relação";
                worksheet.Cell(1, 2).Value = "ID do Empréstimo";
                worksheet.Cell(1, 3).Value = "Usuário Solicitante";
                worksheet.Cell(1, 4).Value = "Sala";
                worksheet.Cell(1, 5).Value = "ID do Item";
                worksheet.Cell(1, 6).Value = "Descrição do Item";
                worksheet.Cell(1, 7).Value = "Estado do Empréstimo";

                //Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var i in emprestimoItens)
                {
                    //Prencher as linha com base nos dados banco
                    worksheet.Cell(linha, 1).Value = i.Id;
                    worksheet.Cell(linha, 2).Value = i.EmprestimoId;
                    worksheet.Cell(linha, 3).Value = i.Emprestimos?.Usuario?.Nome ?? "";
                    worksheet.Cell(linha, 4).Value = i.Emprestimos?.Sala?.Descricao ?? "";
                    worksheet.Cell(linha, 5).Value = i.ItemId;
                    worksheet.Cell(linha, 6).Value = i.Itens?.Descricao ?? "";
                    worksheet.Cell(linha, 7).Value = i.Emprestimos?.Estado ?? "";
                    linha++;
                }

                //Ajusta proporcionalmente o tamanho das colunas ao conteúdo
                worksheet.Columns().AdjustToContents();
            }
        }
    }
}
