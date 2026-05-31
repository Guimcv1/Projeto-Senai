using SCA.Core.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Models;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class ItensView : UserControl, IReloadableView
{
    private JanelaPrincipal? _parent;
    private int _editingId = -1;
    private List<Sala> _salasDisponiveis = new();

    private void RegistrarAcao(string acao)
    {
        int usuarioId = _parent?.CurrentAdmin?.Id ?? 0;
        if (usuarioId > 0)
        {
            SCA.Core.Services.LogService.RegistrarLog(acao, AcaoTipo.Item, usuarioId);
        }
    }

    public ItensView()
    {
        InitializeComponent();
    }

    public ItensView(JanelaPrincipal parent) : this()
    {
        _parent = parent;
        LoadData();
    }

    public void Reload()
    {
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _salasDisponiveis = SalaService.ListarSala();
            
            // Populate Combos for Dialog
            cbSala.ItemsSource = _salasDisponiveis.Select(s => s.Descricao).ToList();
            cbEstado.ItemsSource = Estados.TodosEstados.Where(e => !string.IsNullOrEmpty(e)).ToList();

            // Load DataGrid using Service
            var itens = AdminService.ListarIntens();

            var listUI = new List<ItemAdminUI>();

            foreach (var item in itens)
            {
                var sala = _salasDisponiveis.FirstOrDefault(s => s.Id == item.SalaId);
                var salaNome = sala != null ? sala.Descricao : "Sem Sala";
                
                listUI.Add(new ItemAdminUI
                {
                    Id = item.Id,
                    Descricao = item.Descricao,
                    SalaNome = salaNome,
                    Estado = item.Estado,
                    BadgeColor = GetBadgeColor(item.Estado)
                });
            }

            dgItens.ItemsSource = listUI;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar itens: {ex.Message}");
        }
    }

    private string GetBadgeColor(string estado)
    {
        if (estado == Estados.Livre) return "#16A34A";
        if (estado == Estados.Emprestado) return "#DC2626";
        if (estado == Estados.Analise) return "#94A3B8";
        return "#64748B";
    }

    private void NovoItem_Click(object sender, RoutedEventArgs e)
    {
        _editingId = -1;
        txtDialogTitle.Text = "Novo Item";
        txtItemDescricao.Text = "";
        cbSala.SelectedIndex = -1;
        cbEstado.SelectedIndex = 0; 
        cbEstado.IsEnabled = false; 
        ItemDialogOverlay.IsVisible = true;
        RegistrarAcao("Abriu cadastro de novo item");
    }

    private void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ItemAdminUI itemUI)
        {
            using var context = new BancoContext();
            var item = context.Itens.Find(itemUI.Id);
            if (item != null)
            {
                _editingId = item.Id;
                txtDialogTitle.Text = "Editar Item";
                txtItemDescricao.Text = item.Descricao;
                
                var sala = _salasDisponiveis.FirstOrDefault(s => s.Id == item.SalaId);
                if (sala != null)
                {
                    cbSala.SelectedItem = sala.Descricao;
                }

                cbEstado.IsEnabled = true;
                cbEstado.SelectedItem = item.Estado;

                ItemDialogOverlay.IsVisible = true;
                RegistrarAcao($"Abriu edição do item {item.Id}");
            }
        }
    }

    private void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        ItemDialogOverlay.IsVisible = false;
        RegistrarAcao("Fechou dialogo de item");
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        string descricao = txtItemDescricao.Text?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(descricao)) return;
        if (cbSala.SelectedItem == null) return;

        string selectedSalaDesc = cbSala.SelectedItem.ToString() ?? "";
        int salaId = _salasDisponiveis.FirstOrDefault(s => s.Descricao == selectedSalaDesc)?.Id ?? 0;

        using var context = new BancoContext();
        if (_editingId == -1 && context.Itens.Any(i => i.Descricao == descricao))
        {
            _parent?.ShowMessage($"O item '{descricao}' já existe no banco de dados!");
            return;
        }
        if (_editingId != -1 && context.Itens.Any(i => i.Descricao == descricao && i.Id != _editingId))
        {
            _parent?.ShowMessage($"O item '{descricao}' já existe no banco de dados!");
            return;
        }

        bool success;
        if (_editingId == -1)
        {
            success = AdminService.CriarIntens(descricao, salaId);
        }
        else
        {
            string estado = cbEstado.SelectedItem?.ToString() ?? Estados.Livre;
            success = AdminService.EditarIntens(_editingId, descricao, estado, salaId);
        }

        if (success)
        {
            ItemDialogOverlay.IsVisible = false;
            LoadData();
            _parent?.ShowMessage("Item salvo com sucesso!", false);
            RegistrarAcao(_editingId == -1 ? $"Criou item: {descricao}" : $"Editou item {_editingId}: {descricao}");
        }
        else
        {
            _parent?.ShowMessage("Ocorreu um erro ao salvar o item.");
        }
    }
}

public class ItemAdminUI
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public string SalaNome { get; set; } = "";
    public string Estado { get; set; } = "";
    public string BadgeColor { get; set; } = "";
}
