using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using gerenciador_chaves.Back.Data;

namespace gerenciador_chaves.Back.Execel
{
    public class ExportToExecel
    {
        public static string ExportarUsuariosParaExcel(string caminhoArquivo)
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

                // Busca os dados sem rastreamento (AsNoTracking) para economizar memória RAM
                var usuarios = context.Usuarios.AsNoTracking().ToList();

                // Cria um novo workbook (arquivo Excel)
                using var workbook = new XLWorkbook();

                // Adiciona uma aba chamada "Lista de Usuarios"
                var worksheet = workbook.Worksheets.Add("Lista de Usuarios");

                // Define os cabeçalhos na primeira linha
                worksheet.Cell(1, 1).Value = "Nome";
                worksheet.Cell(1, 2).Value = "Login";
                worksheet.Cell(1, 3).Value = "Admin";
                worksheet.Cell(1, 4).Value = "Ativo";

                // Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var usuario in usuarios)
                {
                    worksheet.Cell(linha, 1).Value = usuario.Nome;
                    worksheet.Cell(linha, 2).Value = usuario.Login;
                    worksheet.Cell(linha, 3).Value = usuario.IsAdmin ? true : false;
                    worksheet.Cell(linha, 4).Value = usuario.IsAtivo ? true : false;
                    linha++;
                }

                // Ajusta a largura das colunas automaticamente
                worksheet.Columns().AdjustToContents();

                // Salva o arquivo
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
