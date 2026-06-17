using SCA.Core.Data;
using SCA.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using SCA.Back.Execel;

namespace SCA.Core.Services
{
    public class LogService
    {
        private static string LimitarTexto(string? texto, int maximo)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            var limpo = texto.Trim();
            return limpo.Length <= maximo ? limpo : limpo[..maximo];
        }

        private static string MontarAcaoDetalhada(string? solicitante = null, string? alvo = null)
        {
            var s = LimitarTexto(solicitante, 20);
            var t = LimitarTexto(alvo, 24);
            return $"s={s};t={t}";
        }

        public static string? ExtrairCampo(string? acao, string campo)
        {
            if (string.IsNullOrWhiteSpace(acao) || string.IsNullOrWhiteSpace(campo))
            {
                return null;
            }

            campo = campo switch
            {
                "solicitante" => "s",
                "alvo" => "t",
                _ => campo
            };

            var match = Regex.Match(acao, $@"(?:^|;\s*){Regex.Escape(campo)}=(.*?)(?=;\s*\w+=|$)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return null;
            }

            var valor = match.Groups[1].Value.Trim();
            return string.IsNullOrEmpty(valor) ? null : valor;
        }

        //Filtrar Log - Filtro básico de tempo e categoria compatível com o TipoExportacao
        public static List<Log> FiltrarLogs(ExportacaoExcel.TipoExportacao tipo, DateTime? inicio = null, DateTime? fim = null)
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

                if (tipo == ExportacaoExcel.TipoExportacao.LogsIntens) 
                { 
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Item);
                }
                else if (tipo == ExportacaoExcel.TipoExportacao.LogsSala)
                {
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Sala);
                }
                else if (tipo == ExportacaoExcel.TipoExportacao.LogsUsuario)
                {
                    query = query.Where(l => l.TipoAcao == AcaoTipo.Usuario);
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao filtrar logs: {ex.Message}");
                return new List<Log>();
            }
        }

        //Registrar Log - Apenas cadastrar, sem edição ou deleção
        public static bool RegistrarLog(string acao, string tipoAcao, int usuarioId, string? solicitante = null, string? alvo = null)
        {
            try
            {
                using var context = new BancoContext();
                
                var log = new Log
                {
                    Acao = MontarAcaoDetalhada(solicitante, alvo),
                    TipoAcao = tipoAcao,
                    UsuarioId = usuarioId,
                    DataAcao = DateTime.UtcNow
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

        //Listar Log - Apenas ler, sem edição ou deleção
        public static List<Log> ListarLogs()
        {
            try
            {
                using var context = new BancoContext();
                return context.Logs.Include(l => l.Usuario).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar logs: {ex.Message}");
                return new List<Log>();
            }
        }
    }
}
