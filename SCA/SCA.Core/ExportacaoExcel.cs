using SCA.Core.Data;
﻿using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.IO;

using SCA.Core.Models;
using SCA.Core.Services;

namespace SCA.Back.Execel
{
    public class ExportacaoExcel
    {
        //Garanta que não há erro de digitação
        public enum TipoExportacao
        {
            Tudo, Item, Usuario, Sala, Empresitmos,
            TodosLogs, LogsIntens, LogsSala, LogsUsuario, EmprestimoItem
        }

        public static string ExportarParaExcel(string caminhoArquivo, TipoExportacao tipo, DateTime? inicio = null, DateTime? fim = null, string? statusItem = null, bool? isAdmin = null, bool expLogs = true, bool expUsers = true, bool expItems = true, bool expRooms = true)
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

                //Busca os dados sem rastreamento (AsNoTracking) para economizar memória RAM
                var intens = context.Itens.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(statusItem)) intens = intens.Where(i => i.Estado == statusItem).ToList();

                var usuarios = context.Usuarios.AsNoTracking().ToList();
                if (isAdmin.HasValue) usuarios = usuarios.Where(u => u.IsAdmin == isAdmin.Value).ToList();

                var salas = context.Salas.AsNoTracking().ToList();

                //Cria um novo workbook (arquivo Excel)
                using var workbook = new XLWorkbook();

                //Aba de Item
                if (expItems) ExportadorIntens.AdicionarAba(workbook, tipo, intens);

                //Aba de Usuários
                if (expUsers) ExportadorUsuarios.AdicionarAba(workbook, tipo, usuarios);

                //Aba de Sala
                if (expRooms) ExportadorSala.AdicionarAba(workbook, tipo, salas);

                //Aba de Emprestimos
                if (expLogs) ExportadorEmprestimos.AdicionarAba(workbook, tipo, inicio, fim);

                //Aba de EmprestimoItem
                if (expLogs) ExportadorEmprestimoIntens.AdicionarAba(workbook, tipo);

                //Aba de Log
                if (expLogs) ExportadorLogs.AdicionarAba(workbook, tipo, inicio, fim);

                //Salva o arquivo
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
