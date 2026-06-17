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

    private void RegistrarAcao(string acao)
    {
        int usuarioId = _parent?.CurrentAdmin?.Id ?? 0;
        if (usuarioId > 0)
        {
            SCA.Core.Services.LogService.RegistrarLog(acao, AcaoTipo.Sala, usuarioId);
        }
    }

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
            var salas = SalaService.ListarSala();
            
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
        RegistrarAcao("Abriu cadastro de novo local");
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
                RegistrarAcao($"Abriu edição do local {sala.Id}");
            }
        }
    }

    private void Remover_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is LocalUI local)
        {
            int usuarioId = _parent?.CurrentAdmin?.Id ?? 0;
            if (SalaService.InativaSala(local.Id, usuarioId))
            {
                LoadLocais();
            }
        }
    }

    private void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        LocalDialogOverlay.IsVisible = false;
        RegistrarAcao("Fechou dialogo de local");
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        string nome = txtLocalNome.Text?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(nome)) return;

        bool success;
        int usuarioId = _parent?.CurrentAdmin?.Id ?? 0;
        if (_editingId == -1) success = SalaService.CriarSala(nome, usuarioId);
        else success = SalaService.EditarSala(_editingId, nome, chkIsAtivo.IsChecked, usuarioId);

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
