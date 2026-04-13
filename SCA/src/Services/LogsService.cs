using SCA.Back.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using SCA.Back.Execel;

namespace SCA.Back.Services
{
    public class LogsService
    {
        //Filtrar Logs - Filtro básico de tempo e categoria compatível com o TipoExeport
        public static List<Logs> FiltrarLogs(ExportarExecel.TipoExeport tipo, DateTime? inicio = null, DateTime? fim = null)
        {
            try
            {
                using var context = new BancoContext();
                var query = context.Logs.Include(l => l.Usuario).AsQueryable();

                //Ver ser o foi passado o inicio da data para o filtro
                if (inicio.HasValue) 
                { 
                    query = query.Where(l => l.DataAcao >= inicio.Value); 
                }

                //Ver ser o foi passado o fim da data para o filtro
                if (fim.HasValue) 
                { 
                    query = query.Where(l => l.DataAcao <= fim.Value);
                }

                if (tipo == ExportarExecel.TipoExeport.LogsIntens) 
                { 
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Item);
                }
                else if (tipo == ExportarExecel.TipoExeport.LogsSala)
                {
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Sala);
                }
                else if (tipo == ExportarExecel.TipoExeport.LogsUsuario)
                {
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Usuario);
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao filtrar logs: {ex.Message}");
                return new List<Logs>();
            }
        }

        //Registrar Log - Apenas cadastrar, sem edição ou deleção
        public static bool RegistrarLog(string acao, string tipoAcao, int usuarioId)
        {
            try
            {
                using var context = new BancoContext();
                
                var log = new Logs
                {
                    Acao = acao,
                    TipoAcao = tipoAcao,
                    UsuarioId = usuarioId,
                    DataAcao = DateTime.Now
                };

                context.Logs.Add(log);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar log: {ex.Message}");
                return false;
            }
        }

        //Listar Logs - Apenas ler, sem edição ou deleção
        public static List<Logs> ListarLogs()
        {
            try
            {
                using var context = new BancoContext();
                return context.Logs.Include(l => l.Usuario).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar logs: {ex.Message}");
                return new List<Logs>();
            }
        }
    }
}
