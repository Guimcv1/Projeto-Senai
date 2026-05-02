using ClosedXML.Excel;
using SCA.Core.Models;

namespace SCA.Back.Execel
{
    public class ExportadorSala
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportacaoExcel.TipoExportacao tipo, List<Sala> salas)
        {
            if (tipo == ExportacaoExcel.TipoExportacao.Tudo || tipo == ExportacaoExcel.TipoExportacao.Sala)
            {
                var listaSalas = workbook.Worksheets.Add("Lista de Sala");

                //Define os cabeçalhos na primeira linha
                listaSalas.Cell(1, 1).Value = "ID";
                listaSalas.Cell(1, 2).Value = "Descrição";
                listaSalas.Cell(1, 3).Value = "Ativo";

                //Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var s in salas)
                {
                    //Prencher as linha com base nos dados banco
                    listaSalas.Cell(linha, 1).Value = s.Id;
                    listaSalas.Cell(linha, 2).Value = s.Descricao;
                    listaSalas.Cell(linha, 3).Value = s.isAtivo ? "Sim" : "Não";
                    linha++;
                }

                //Ajusta a largura das colunas automaticamente
                listaSalas.Columns().AdjustToContents();
            }
        }
    }
}
