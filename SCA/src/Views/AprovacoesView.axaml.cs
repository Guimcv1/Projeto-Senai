using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Models;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class AprovacoesView : UserControl, IReloadableView
{
    private JanelaPrincipal? _parent;
    private List<AprovacaoUI> _todasAprovacoes = new();

    public AprovacoesView()
    {
        InitializeComponent();
    }

    public AprovacoesView(JanelaPrincipal parent) : this()
    {
        _parent = parent;
        LoadPendencias();
    }

    public void Reload()
    {
        LoadPendencias();
    }

    private void LoadPendencias()
    {
        try
        {
            var oldList = dgAprovacoes?.ItemsSource as List<AprovacaoUI>;
            var selectedIds = oldList?.Where(x => x.IsSelected).Select(x => x.EmprestimoId).ToHashSet() ?? new HashSet<int>();

            var emprestimos = EmprestimoService.ListarEmprestimo();
            Console.WriteLine($"[Aprovações] Total de empréstimos no banco: {emprestimos.Count}");

            var pendentes = emprestimos.Where(e => e.Estado == Estados.Analise || e.Estado == Estados.AnaliseDevolucao).ToList();
            Console.WriteLine($"[Aprovações] Empréstimos em estado 'Analise' ou 'AnaliseDevolucao': {pendentes.Count}");

            var listUI = new List<AprovacaoUI>();

            foreach (var emp in pendentes)
            {
                if (emp.EmprestimoItem == null || !emp.EmprestimoItem.Any())
                {
                    Console.WriteLine($"[Aprovações] Empréstimo {emp.Id} ignorado: sem itens vinculados.");
                    continue;
                }

                var tipoSolicitacao = EmprestimoService.ObterTipoSolicitacao(emp.Id);
                bool isDevolucao = tipoSolicitacao == EmprestimoService.TipoSolicitacao.Devolucao;

                listUI.Add(new AprovacaoUI
                {
                    EmprestimoId = emp.Id,
                    IsSelected = selectedIds.Contains(emp.Id),
                    DescricaoItems = string.Join(", ", emp.EmprestimoItem.Select(ei => ei.Item?.Descricao ?? "Item s/ Desc")),
                    Ambiente = emp.Sala?.Descricao ?? "Desconhecido",
                    Solicitante = emp.Usuario?.Nome ?? "Usuário Desconhecido",
                    Acao = isDevolucao ? "Devolução" : "Empréstimo",
                    BadgeColor = isDevolucao ? "#EA580C" : "#002776",
                    Tipo = tipoSolicitacao
                });
            }

            _todasAprovacoes = listUI;
            FilterData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar pendências: {ex.Message}");
        }
    }

    private string NormalizeString(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToUpper();
    }

    private void FilterData()
    {
        if (dgAprovacoes == null) return;
        string searchText = NormalizeString(txtSearch?.Text ?? "");

        var filtrados = _todasAprovacoes.Where(a =>
            string.IsNullOrEmpty(searchText) ||
            NormalizeString(a.DescricaoItems).Contains(searchText) ||
            NormalizeString(a.Ambiente).Contains(searchText) ||
            NormalizeString(a.Solicitante).Contains(searchText)
        ).ToList();

        dgAprovacoes.ItemsSource = filtrados;
    }

    private void Search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        FilterData();
    }

    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        if (txtSearch == null) return;

        txtSearch.Text = "";
        FilterData();
    }

    private void ChkSelectAll_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox chk && dgAprovacoes != null && dgAprovacoes.ItemsSource is List<AprovacaoUI> items)
        {
            bool isChecked = chk.IsChecked == true;
            foreach (var item in items)
            {
                item.IsSelected = isChecked;
            }
            
            // Reassign to force UI update
            dgAprovacoes.ItemsSource = null;
            dgAprovacoes.ItemsSource = items;
        }
    }

    private void Aprovar_Click(object sender, RoutedEventArgs e)
    {
        Processar(true);
    }

    private void Recusar_Click(object sender, RoutedEventArgs e)
    {
        Processar(false);
    }

    private void Processar(bool isAprovado)
    {
        var items = dgAprovacoes.ItemsSource as IEnumerable<AprovacaoUI>;
        if (items == null) 
        {
            Console.WriteLine("[Aprovações] Erro: ItemsSource nulo ou tipo inválido.");
            return;
        }

        var selected = items.Where(x => x.IsSelected).ToList();
        
        if (!selected.Any())
        {
            _parent?.ShowMessage("Selecione ao menos um item.");
            return;
        }

        int successCount = 0;
        foreach (var item in selected)
        {
            try 
            {
                if (EmprestimoService.AprovarSolicitacao(item.EmprestimoId, item.Tipo, isAprovado))
                {
                    int adminId = _parent?.CurrentAdmin?.Id ?? 0;
                    if (adminId > 0)
                    {
                        string acaoStr = isAprovado ? "Aprovou" : "Recusou";
                        LogService.RegistrarLog($"{acaoStr} solicitação (ID: {item.EmprestimoId})",AcaoTipo.Emprestado, adminId);
                    }
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Aprovações] Erro ao processar ID {item.EmprestimoId}: {ex.Message}");
            }
        }

        if (successCount > 0)
        {
            _parent?.ShowMessage($"{successCount} solicitações processadas com sucesso!", false);
        }
        else
        {
            _parent?.ShowMessage("Ocorreu um erro ao processar as solicitações.");
        }
        
        LoadPendencias();
    }
}

public class AprovacaoUI
{
    public int EmprestimoId { get; set; }
    public bool IsSelected { get; set; }
    public string DescricaoItems { get; set; } = "";
    public string Ambiente { get; set; } = "";
    public string Solicitante { get; set; } = "";
    public string Acao { get; set; } = "";
    public string BadgeColor { get; set; } = "";
    public EmprestimoService.TipoSolicitacao Tipo { get; set; }
}
