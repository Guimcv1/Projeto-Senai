using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Models;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class LocaisView : UserControl, IReloadableView
{
    private JanelaPrincipal? _parent;
    private int _editingId = -1;
    private List<Sala> _todasSalas = new();

    public LocaisView()
    {
        InitializeComponent();
    }

    public LocaisView(JanelaPrincipal parent) : this()
    {
        _parent = parent;
        LoadLocais();
    }

    public void Reload()
    {
        LoadLocais();
    }

    private void LoadLocais()
    {
        try
        {
            _todasSalas = SalaService.ListarSala();
            FilterData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar locais: {ex.Message}");
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
        if (dgLocais == null) return;
        string searchText = NormalizeString(txtSearch?.Text ?? "");

        var listUI = new List<LocalUI>();

        var filtrados = _todasSalas.Where(s =>
            string.IsNullOrEmpty(searchText) ||
            NormalizeString(s.Descricao).Contains(searchText)
        );

        foreach (var sala in filtrados)
            {
                listUI.Add(new LocalUI
                {
                    Id = sala.Id,
                    Descricao = sala.Descricao?.ToUpper() ?? "",
                    StatusText = sala.isAtivo ? "Ativo" : "Inativo",
                    BadgeColor = sala.isAtivo ? "#16A34A" : "#94A3B8"
                });
            }

            dgLocais.ItemsSource = listUI;
    }

    private void Search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        FilterData();
    }

    private void NovoLocal_Click(object sender, RoutedEventArgs e)
    {
        _editingId = -1;
        txtDialogTitle.Text = "Novo Local";
        txtLocalNome.Text = "";
        
        chkIsAtivo.IsChecked = true;
        LocalDialogOverlay.IsVisible = true;
    }

    private void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is LocalUI local)
        {
            var sala = SalaService.BuscarPorIdSala(local.Id);
            if (sala != null)
            {
                _editingId = sala.Id;
                txtDialogTitle.Text = "Editar Local";
                txtLocalNome.Text = sala.Descricao;
                
                chkIsAtivo.IsChecked = sala.isAtivo;
                LocalDialogOverlay.IsVisible = true;
            }
        }
    }

    private void Remover_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is LocalUI local)
        {
            if (SalaService.InativaSala(local.Id))
            {
                LoadLocais();
            }
        }
    }

    private void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        LocalDialogOverlay.IsVisible = false;
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        string nome = txtLocalNome.Text?.Trim().ToUpper() ?? "";
        
        if (string.IsNullOrEmpty(nome)) return;

        bool success;
        if (_editingId == -1) success = SalaService.CriarSala(nome);
        else success = SalaService.EditarSala(_editingId, nome, chkIsAtivo.IsChecked);

        if (success)
        {
            LocalDialogOverlay.IsVisible = false;
            LoadLocais();
        }
    }
}

public class LocalUI
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public string StatusText { get; set; } = "";
    public string BadgeColor { get; set; } = "";
}
