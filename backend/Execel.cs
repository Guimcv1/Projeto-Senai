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
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Nome";
                worksheet.Cell(1, 3).Value = "Login";
                worksheet.Cell(1, 4).Value = "Admin";
                worksheet.Cell(1, 5).Value = "Ativo";

                // Preenche os dados a partir da linha 2
                int linha = 2;
                foreach (var usuario in usuarios)
                {
                    worksheet.Cell(linha, 1).Value = usuario.Id;
                    worksheet.Cell(linha, 2).Value = usuario.Nome;
                    worksheet.Cell(linha, 3).Value = usuario.Login;
                    worksheet.Cell(linha, 4).Value = usuario.IsAdmin ? "Sim" : "Não";
                    worksheet.Cell(linha, 5).Value = usuario.IsAtivo ? "Sim" : "Não";
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

        public static string ExportarIntensParaExcel(string caminhoArquivo)
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
                var intens = context.Intens.AsNoTracking().ToList();

                // Cria um novo workbook (arquivo Excel)
                using var workbook = new XLWorkbook();

                // Adiciona uma aba chamada "Lista de Intens"
                var worksheet = workbook.Worksheets.Add("Lista de Intens");

                // Define os cabeçalhos na primeira linha
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Descrição";
                worksheet.Cell(1, 3).Value = "Estado";
                worksheet.Cell(1, 4).Value = "Abiente";

                // Preenche os dados a partir da linha 2, isso se deve pq o um é o cabesalho
                int linha = 2;
                foreach (var item in intens)
                {
                    worksheet.Cell(linha, 1).Value = item.Id;
                    worksheet.Cell(linha, 2).Value = item.Descricao;
                    worksheet.Cell(linha, 3).Value = $"abiente{linha}";
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

        public static string ExportarTudoParaExcel(string caminhoArquivo)
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
                var intens = context.Intens.AsNoTracking().ToList();
                var usuarios = context.Usuarios.AsNoTracking().ToList();

                // Cria um novo workbook (arquivo Excel)
                using var workbook = new XLWorkbook();

                // === ABA 1: ITENS ===
                var listaIntens = workbook.Worksheets.Add("Lista de Itens");

                // Define os cabeçalhos na primeira linha
                listaIntens.Cell(1, 1).Value = "ID";
                listaIntens.Cell(1, 2).Value = "Descrição";
                listaIntens.Cell(1, 3).Value = "Estado";
                listaIntens.Cell(1, 4).Value = "Abiente";

                // Preenche os dados a partir da linha 2
                int linhaIntens = 2;
                foreach (var item in intens)
                {
                    listaIntens.Cell(linhaIntens, 1).Value = item.Id;
                    listaIntens.Cell(linhaIntens, 2).Value = item.Descricao;
                    listaIntens.Cell(linhaIntens, 3).Value = item.Estado;
                    listaIntens.Cell(linhaIntens, 4).Value = $"abiente{linhaIntens}";
                    linhaIntens++;
                }

                // Ajusta a largura das colunas automaticamente
                listaIntens.Columns().AdjustToContents();

                // === ABA 2: USUÁRIOS ===
                var listaUsuarios = workbook.Worksheets.Add("Lista de Usuarios");

                // Define os cabeçalhos na primeira linha
                listaUsuarios.Cell(1, 1).Value = "ID";
                listaUsuarios.Cell(1, 2).Value = "Nome";
                listaUsuarios.Cell(1, 3).Value = "Login";
                listaUsuarios.Cell(1, 4).Value = "Admin";
                listaUsuarios.Cell(1, 5).Value = "Ativo";

                // Preenche os dados a partir da linha 2
                int linhaUsuarios = 2;
                foreach (var usuario in usuarios)
                {
                    listaUsuarios.Cell(linhaUsuarios, 1).Value = usuario.Id;
                    listaUsuarios.Cell(linhaUsuarios, 2).Value = usuario.Nome;
                    listaUsuarios.Cell(linhaUsuarios, 3).Value = usuario.Login;
                    listaUsuarios.Cell(linhaUsuarios, 4).Value = usuario.IsAdmin ? "Sim" : "Não";
                    listaUsuarios.Cell(linhaUsuarios, 5).Value = usuario.IsAtivo ? "Sim" : "Não";
                    linhaUsuarios++;
                }

                // Ajusta a largura das colunas automaticamente
                listaUsuarios.Columns().AdjustToContents();

                // Salva o arquivo
                workbook.SaveAs(caminhoArquivo);

                return $"Sucesso! Arquivo com 2 abas salvo em: {Path.GetFullPath(caminhoArquivo)}";
            }
            catch (Exception ex)
            {
                return $"Erro ao exportar: {ex.Message}";
            }
        }


    }
}
