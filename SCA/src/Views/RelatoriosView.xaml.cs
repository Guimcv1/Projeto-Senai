using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using SCA.Back.Data;
using SCA.Back.Services;
using SCA.Back.Execel;

namespace SCA.Views
{
    public partial class RelatoriosView : UserControl
    {
        private MainWindow _parent;

        public SeriesCollection OcupacaoSeries { get; set; }
        public SeriesCollection UsoSeries { get; set; }
        public List<string> LabelsSalas { get; set; }

        public RelatoriosView(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            
            LoadCharts();
            
            DataContext = this;
        }

        private void LoadCharts()
        {
            try
            {
                using var context = new BancoContext();
                var salas = context.Salas.ToList();
                var itens = context.Itens.ToList();
                var emprestimos = context.Emprestimos.ToList();

                // Chart 1: Ocupação (Status of items: Disponível, Emprestado, Pendente, Manutenção)
                int disponiveis = itens.Count(i => i.Estado == Estados.Livre);
                int emprestados = itens.Count(i => i.Estado == Estados.Emprestado);
                int pendentes = itens.Count(i => i.Estado == Estados.Pendente || i.Estado == Estados.Analise);

                OcupacaoSeries = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Disponíveis",
                        Values = new ChartValues<int> { disponiveis },
                        DataLabels = true,
                        Fill = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#16A34A"))
                    },
                    new PieSeries
                    {
                        Title = "Emprestados",
                        Values = new ChartValues<int> { emprestados },
                        DataLabels = true,
                        Fill = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626"))
                    },
                    new PieSeries
                    {
                        Title = "Em Análise",
                        Values = new ChartValues<int> { pendentes },
                        DataLabels = true,
                        Fill = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#94A3B8"))
                    }
                };

                chartOcupacao.Series = OcupacaoSeries;

                // Chart 2: Locais Mais Usados (Count emprestimos per Sala)
                var usosPorSala = emprestimos
                    .GroupBy(e => e.SalaId)
                    .Select(g => new { SalaId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                LabelsSalas = new List<string>();
                var usosValues = new ChartValues<int>();

                foreach (var uso in usosPorSala)
                {
                    var sala = salas.FirstOrDefault(s => s.Id == uso.SalaId);
                    LabelsSalas.Add(sala?.Descricao ?? $"Sala {uso.SalaId}");
                    usosValues.Add(uso.Count);
                }

                UsoSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Solicitações",
                        Values = usosValues,
                        Fill = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#002776"))
                    }
                };

                chartUso.Series = UsoSeries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar relatórios: {ex.Message}");
            }
        }

        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportarExecel.Exportar(ExportarExecel.TipoExeport.emprestimo);
                MessageBox.Show("Relatório de empréstimos exportado com sucesso (verifique a pasta do projeto/desktop).", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
