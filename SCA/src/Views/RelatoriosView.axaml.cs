using SCA.Core.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SCA.Core.Models;
using SCA.Core.Services;
using SCA.Back.Execel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class RelatoriosView : UserControl
{
    private MainWindow? _parent;

    public RelatoriosView()
    {
        InitializeComponent();
    }

    public RelatoriosView(MainWindow parent) : this()
    {
        _parent = parent;
        LoadCharts();
    }

    private void LoadCharts()
    {
        try
        {
            using var context = new BancoContext();
            var salas = context.Salas.ToList();
            var itens = context.Itens.ToList();
            var emprestimos = context.Emprestimos.ToList();

            // Chart 1: Ocupação
            int disponiveis = itens.Count(i => i.Estado == Estados.Livre);
            int emprestados = itens.Count(i => i.Estado == Estados.Emprestado);
            int pendentes = itens.Count(i => i.Estado == Estados.Analise);

            chartOcupacao.Series = new ISeries[]
            {
                new PieSeries<int>
                {
                    Name = "Disponíveis",
                    Values = new int[] { disponiveis },
                    Fill = new SolidColorPaint(SKColor.Parse("#16A34A"))
                },
                new PieSeries<int>
                {
                    Name = "Emprestados",
                    Values = new int[] { emprestados },
                    Fill = new SolidColorPaint(SKColor.Parse("#DC2626"))
                },
                new PieSeries<int>
                {
                    Name = "Em Análise",
                    Values = new int[] { pendentes },
                    Fill = new SolidColorPaint(SKColor.Parse("#94A3B8"))
                }
            };

            // Chart 2: Locais Mais Usados
            var usosPorSala = emprestimos
                .GroupBy(e => e.SalaId)
                .Select(g => new { SalaId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            var labelsSalas = new List<string>();
            var usosValues = new List<int>();

            foreach (var uso in usosPorSala)
            {
                var sala = salas.FirstOrDefault(s => s.Id == uso.SalaId);
                labelsSalas.Add(sala?.Descricao ?? $"Sala {uso.SalaId}");
                usosValues.Add(uso.Count);
            }

            chartUso.Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Solicitações",
                    Values = usosValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#002776"))
                }
            };

            chartUso.XAxes = new Axis[]
            {
                new Axis { Labels = labelsSalas }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar relatórios: {ex.Message}");
        }
    }

    private async void ExportarExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Salvar Relatório",
                SuggestedFileName = "Relatorio_SCA.xlsx",
                DefaultExtension = ".xlsx",
                FileTypeChoices = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("Planilha do Excel")
                    {
                        Patterns = new[] { "*.xlsx" }
                    }
                }
            });

            if (file != null)
            {
                string filePath = file.Path.LocalPath;
                ExportarExecel.ExportarParaExcel(filePath, ExportarExecel.TipoExeport.Empresitmos);
                _parent?.ShowMessage($"Relatório exportado com sucesso!", false);
                Console.WriteLine($"Relatório de empréstimos exportado com sucesso para {filePath}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao exportar: {ex.Message}");
            _parent?.ShowMessage($"Erro ao exportar o relatório.");
        }
    }
}
