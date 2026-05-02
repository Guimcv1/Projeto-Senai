using SCA.Core.Data;
﻿using SCA.Core.Models;
using SCA.Back.Execel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Core.Services
{
    public class EmprestimoItemService
    {
        //Filtrar EmprestimoItem para Exportação
        public static List<EmprestimoItem> FiltrarEmprestimoIntens(ExportacaoExcel.TipoExportacao tipo)
        {
            try
            {
                using var context = new BancoContext();

                //Monta a consulta inicial na tabela de EmprestimoItem
               
                //O .ThenInclude é usado para carregar uma entidade relacionada a partir da entidade incluída anteriormente.
                var query = context.EmprestimoItens
                    //Carrega os dados do Item relacionado
                    .Include(i => i.Item)
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
                Console.WriteLine($"Erro ao filtrar EmprestimoItem: {ex.Message}");
                // Retorna uma lista vazia caso ocorra algum erro durante a busca
                return new List<EmprestimoItem>();
            }
        }

        //Listar todos os EmprestimoItem
        public static List<EmprestimoItem> ListarEmprestimoIntens()
        {
            try
            {
                using var context = new BancoContext();
                /*Busca todos os registros populando as tabelas relacionadas básicas (Item e Emprestimos)
                retorna a lista completa de EmprestimoItem*/
                return context.EmprestimoItens
                    .Include(ei => ei.Item)
                    .Include(ei => ei.Emprestimos)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar EmprestimoItem: {ex.Message}");
                return new List<EmprestimoItem>();
            }
        }

        //Buscar por ID
        public static EmprestimoItem? BuscarPorId(int id)
        {
            try
            {
                using var context = new BancoContext();
                /*Realiza a busca pelo relacionamento específico utilizando Id,
                incluindo na consulta as informações do Item e Emprestimo atrelados*/
                return context.EmprestimoItens
                    .Include(i => i.Item)
                    .Include(i => i.Emprestimos)
                    .FirstOrDefault(i => i.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar EmprestimoItem: {ex.Message}");
                return null;
            }
        }
    }
}
