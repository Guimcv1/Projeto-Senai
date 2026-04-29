using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Views
{
    public partial class AprovacoesView : UserControl
    {
        private MainWindow _parent;

        public AprovacoesView(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            LoadPendencias();
        }

        private void LoadPendencias()
        {
            try
            {
                var emprestimos = EmprestimosService.ListarEmprestimo();
                var pendentes = emprestimos.Where(e => e.Estado == Estados.Analise).ToList();

                var listUI = new List<AprovacaoUI>();

                foreach (var emp in pendentes)
                {
                    if (emp.EmprestimoIntens == null || !emp.EmprestimoIntens.Any()) continue;

                    // Verifica qual tipo de solicitação é baseado no estado dos itens
                    // Empréstimo novo: Itens estão em Analise
                    // Devolução: Itens estão Emprestados
                    var primeiroItem = emp.EmprestimoIntens.First().Itens;
                    bool isDevolucao = primeiroItem != null && primeiroItem.Estado == Estados.Emprestado;

                    listUI.Add(new AprovacaoUI
                    {
                        EmprestimoId = emp.Id,
                        DescricaoItems = string.Join(", ", emp.EmprestimoIntens.Select(ei => ei.Itens?.Descricao)),
                        Ambiente = emp.Sala?.Descricao ?? "Desconhecido",
                        Solicitante = emp.Usuario?.Nome ?? "Usuário Desconhecido",
                        Acao = isDevolucao ? "Devolução" : "Empréstimo",
                        BadgeColor = isDevolucao ? "#EA580C" : "#002776",
                        Tipo = isDevolucao ? EmprestimosService.TipoSolicitacao.Devolucao : EmprestimosService.TipoSolicitacao.Emprestimo
                    });
                }

                dgAprovacoes.ItemsSource = listUI;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pendências: {ex.Message}");
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
            if (dgAprovacoes.ItemsSource is List<AprovacaoUI> list)
            {
                var selected = list.Where(x => x.IsSelected).ToList();
                if (!selected.Any())
                {
                    MessageBox.Show("Selecione ao menos um item.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int successCount = 0;
                foreach (var item in selected)
                {
                    if (EmprestimosService.AprovarSolicitacao(item.EmprestimoId, item.Tipo, isAprovado))
                    {
                        successCount++;
                    }
                }

                MessageBox.Show($"{successCount} solicitações processadas com sucesso!");
                LoadPendencias();
            }
        }
    }

    public class AprovacaoUI
    {
        public int EmprestimoId { get; set; }
        public bool IsSelected { get; set; }
        public string DescricaoItems { get; set; }
        public string Ambiente { get; set; }
        public string Solicitante { get; set; }
        public string Acao { get; set; }
        public string BadgeColor { get; set; }
        public EmprestimosService.TipoSolicitacao Tipo { get; set; }
    }
}
