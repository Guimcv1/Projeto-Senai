using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Back.Execel
{
    public class ExportadorUsuarios
    {
        public static void AdicionarAba(XLWorkbook workbook, ExportarExecel.TipoExeport tipo, List<Usuario> usuarios)
        {
            if (tipo == ExportarExecel.TipoExeport.Tudo || tipo == ExportarExecel.TipoExeport.Usuario)
            {
                var listaUsuarios = workbook.Worksheets.Add("Lista de Usuarios");

                //Define os cabeçalhos na primeira linha
                listaUsuarios.Cell(1, 1).Value = "ID";
                listaUsuarios.Cell(1, 2).Value = "Nome";
                listaUsuarios.Cell(1, 3).Value = "Login";
                listaUsuarios.Cell(1, 4).Value = "Admin";
                listaUsuarios.Cell(1, 5).Value = "Ativo";

                //Preenche os dados a partir da linha 2
                int linhaUsuarios = 2;
                foreach (var usuario in usuarios)
                {
                    //Prencher as linha com base nos dados banco
                    listaUsuarios.Cell(linhaUsuarios, 1).Value = usuario.Id;
                    listaUsuarios.Cell(linhaUsuarios, 2).Value = usuario.Nome;
                    listaUsuarios.Cell(linhaUsuarios, 3).Value = usuario.Login;
                    listaUsuarios.Cell(linhaUsuarios, 4).Value = usuario.IsAdmin ? "Sim" : "Não";
                    listaUsuarios.Cell(linhaUsuarios, 5).Value = usuario.IsAtivo ? "Sim" : "Não";
                    linhaUsuarios++;
                }

                //Ajusta a largura das colunas automaticamente
                listaUsuarios.Columns().AdjustToContents();
            }
        }
    }
}