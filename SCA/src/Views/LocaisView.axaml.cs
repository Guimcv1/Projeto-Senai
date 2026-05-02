using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Back.Data;
using SCA.Back.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class LocaisView : UserControl
{
    private MainWindow? _parent;
    private int _editingId = -1;

    public LocaisView()
    {
        InitializeComponent();
    }

    public LocaisView(MainWindow parent) : this()
    {
        _parent = parent;
        LoadLocais();
    }

    private void LoadLocais()
    {
        try
        {
            var salas = SalasService.ListarSala();
            
            var listUI = new List<LocalUI>();

            foreach (var sala in salas)
            {
                listUI.Add(new LocalUI
                {
                    Id = sala.Id,
                    Descricao = sala.Descricao,
                    StatusText = sala.isAtivo ? "Ativo" : "Inativo",
                    BadgeColor = sala.isAtivo ? "#16A34A" : "#94A3B8"
                });
            }

            dgLocais.ItemsSource = listUI;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar locais: {ex.Message}");
        }
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
            var sala = SalasService.BuscarPorIdSala(local.Id);
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
            if (SalasService.InativaSala(local.Id))
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
        string nome = txtLocalNome.Text?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(nome)) return;

        bool success;
        if (_editingId == -1) success = SalasService.CriarSala(nome);
        else success = SalasService.EditarSala(_editingId, nome, chkIsAtivo.IsChecked);

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
