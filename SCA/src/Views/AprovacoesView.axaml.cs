using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Models;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class AprovacoesView : UserControl
{
    private MainWindow? _parent;

    public AprovacoesView()
    {
        InitializeComponent();
    }

    public AprovacoesView(MainWindow parent) : this()
    {
        _parent = parent;
        LoadPendencias();
    }

    private void LoadPendencias()
    {
        try
        {
            var emprestimos = EmprestimosService.ListarEmprestimo();
            Console.WriteLine($"[Aprovações] Total de empréstimos no banco: {emprestimos.Count}");

            var pendentes = emprestimos.Where(e => e.Estado == Estados.Analise).ToList();
            Console.WriteLine($"[Aprovações] Empréstimos em estado 'Analise': {pendentes.Count}");

            var listUI = new List<AprovacaoUI>();

            foreach (var emp in pendentes)
            {
                if (emp.EmprestimoIntens == null || !emp.EmprestimoIntens.Any())
                {
                    Console.WriteLine($"[Aprovações] Empréstimo {emp.Id} ignorado: sem itens vinculados.");
                    continue;
                }

                // Verifica qual tipo de solicitação é baseado no estado dos itens
                var primeiroItem = emp.EmprestimoIntens.First().Itens;
                bool isDevolucao = primeiroItem != null && primeiroItem.Estado == Estados.Emprestado;

                listUI.Add(new AprovacaoUI
                {
                    EmprestimoId = emp.Id,
                    DescricaoItems = string.Join(", ", emp.EmprestimoIntens.Select(ei => ei.Itens?.Descricao ?? "Item s/ Desc")),
                    Ambiente = emp.Sala?.Descricao ?? "Desconhecido",
                    Solicitante = emp.Usuario?.Nome ?? "Usuário Desconhecido",
                    Acao = isDevolucao ? "Devolução" : "Empréstimo",
                    BadgeColor = isDevolucao ? "#EA580C" : "#002776",
                    Tipo = isDevolucao ? EmprestimosService.TipoSolicitacao.Devolucao : EmprestimosService.TipoSolicitacao.Emprestimo
                });
            }

            Console.WriteLine($"[Aprovações] Itens para o DataGrid: {listUI.Count}");
            dgAprovacoes.ItemsSource = listUI;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar pendências: {ex.Message}");
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
                if (EmprestimosService.AprovarSolicitacao(item.EmprestimoId, item.Tipo, isAprovado))
                {
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
    public EmprestimosService.TipoSolicitacao Tipo { get; set; }
}
