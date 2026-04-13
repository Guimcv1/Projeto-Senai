using SCA.Back.Data;
using SCA.Back.Execel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Back.Services
{
    public class EmprestimoIntensService
    {
        //Filtrar EmprestimoIntens para Exportação
        public static List<EmprestimoIntens> FiltrarEmprestimoIntens(ExportarExecel.TipoExeport tipo)
        {
            try
            {
                using var context = new BancoContext();

                //Monta a consulta inicial na tabela de EmprestimoIntens
               
                //O .ThenInclude é usado para carregar uma entidade relacionada a partir da entidade incluída anteriormente.
                var query = context.EmprestimoIntens
                    //Carrega os dados do Item relacionado
                    .Include(i => i.Itens)
                    //Carrega os dados do Empréstimo relacionado
                    .Include(i => i.Emprestimos)
                    //e para o empréstimo, carrega o respectivo Usuário
                    .ThenInclude(e => e.Usuario)
                    //Novamente acessa o Empréstimo
                    .Include(i => i.Emprestimos)
                    //para carregar a Sala vinculada a ele
                    .ThenInclude(e => e.Sala)
                    //Converte para IQueryable permitindo filtros adicionais no futuro se necessário
                    .AsQueryable(); 

                // Executa a consulta no banco de dados e retorna os resultados como uma lista
                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao filtrar EmprestimoIntens: {ex.Message}");
                // Retorna uma lista vazia caso ocorra algum erro durante a busca
                return new List<EmprestimoIntens>();
            }
        }

        //Listar todos os EmprestimoIntens
        public static List<EmprestimoIntens> ListarEmprestimoIntens()
        {
            try
            {
                using var context = new BancoContext();
                /*Busca todos os registros populando as tabelas relacionadas básicas (Itens e Emprestimos)
                retorna a lista completa de EmprestimoIntens*/
                return context.EmprestimoIntens
                    .Include(ei => ei.Itens)
                    .Include(ei => ei.Emprestimos)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar EmprestimoIntens: {ex.Message}");
                return new List<EmprestimoIntens>();
            }
        }

        //Buscar por ID
        public static EmprestimoIntens? BuscarPorId(int id)
        {
            try
            {
                using var context = new BancoContext();
                /*Realiza a busca pelo relacionamento específico utilizando Id,
                incluindo na consulta as informações do Item e Emprestimo atrelados*/
                return context.EmprestimoIntens
                    .Include(i => i.Itens)
                    .Include(i => i.Emprestimos)
                    .FirstOrDefault(i => i.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar EmprestimoIntens: {ex.Message}");
                return null;
            }
        }
    }
}
