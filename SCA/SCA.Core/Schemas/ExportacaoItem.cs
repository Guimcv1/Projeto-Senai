using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using SCA.Core.Models;
using SCA.Core.Services;

namespace SCA.Back.Execel
{
    public class ExportadorIntens
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportacaoExcel.TipoExportacao tipo, List<Item> intens)
        {
            if (tipo == ExportacaoExcel.TipoExportacao.Tudo || tipo == ExportacaoExcel.TipoExportacao.Item)
            {
                var listaIntens = workbook.Worksheets.Add("Lista de Item");

                // Define os cabeçalhos na primeira linha
                listaIntens.Cell(1, 1).Value = "ID";
                listaIntens.Cell(1, 2).Value = "Descrição";
                listaIntens.Cell(1, 3).Value = "Estado";
                listaIntens.Cell(1, 4).Value = "Abiente";

                //Preenche os dados a partir da linha 2
                int linhaIntens = 2;
                foreach (var item in intens)
                {
                    //Prencha as linhas com os dados do banco 
                    listaIntens.Cell(linhaIntens, 1).Value = item.Id;
                    listaIntens.Cell(linhaIntens, 2).Value = item.Descricao;
                    listaIntens.Cell(linhaIntens, 3).Value = item.Estado;
                    listaIntens.Cell(linhaIntens, 4).Value = $"abiente{linhaIntens}";
                    linhaIntens++;
                }

                //Ajusta a largura das colunas automaticamente
                listaIntens.Columns().AdjustToContents();
            }
        }
    }
}