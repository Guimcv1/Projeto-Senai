using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Views
{
    public partial class ItensView : UserControl
    {
        private MainWindow _parent;
        private int _editingId = -1;
        private List<Sala> _salasDisponiveis;

        public ItensView(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _salasDisponiveis = SalasService.ListarSala();
                
                // Populate Combos for Dialog
                cbSala.Items.Clear();
                foreach (var sala in _salasDisponiveis)
                {
                    cbSala.Items.Add(new ComboBoxItem { Content = sala.Descricao, Tag = sala.Id });
                }

                cbEstado.Items.Clear();
                foreach (var est in Estados.TodosEstados)
                {
                    cbEstado.Items.Add(new ComboBoxItem { Content = est, Tag = est });
                }

                // Load DataGrid
                using var context = new BancoContext();
                var itens = context.Itens.ToList();
                var listUI = new List<ItemAdminUI>();

                foreach (var item in itens)
                {
                    var salaNome = _salasDisponiveis.FirstOrDefault(s => s.Id == item.SalaId)?.Descricao ?? "Desconhecido";
                    
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
                MessageBox.Show($"Erro ao carregar itens: {ex.Message}");
            }
        }

        private string GetBadgeColor(string estado)
        {
            if (estado == Estados.Livre) return "#16A34A";
            if (estado == Estados.Emprestado) return "#DC2626";
            if (estado == Estados.Pendente) return "#EA580C";
            if (estado == Estados.Analise) return "#94A3B8";
            return "#64748B";
        }

        private void NovoItem_Click(object sender, RoutedEventArgs e)
        {
            _editingId = -1;
            txtDialogTitle.Text = "Novo Item";
            txtItemDescricao.Text = "";
            cbSala.SelectedIndex = -1;
            cbEstado.SelectedIndex = 0; // Default to first (usually Livre)
            cbEstado.IsEnabled = false; // Novo item = Livre (handled by AdminService/IntensService)
            
            MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "ItemDialog");
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
                    
                    // Select Sala
                    foreach (ComboBoxItem comboItem in cbSala.Items)
                    {
                        if ((int)comboItem.Tag == item.SalaId)
                        {
                            cbSala.SelectedItem = comboItem;
                            break;
                        }
                    }

                    // Select Estado
                    cbEstado.IsEnabled = true;
                    foreach (ComboBoxItem comboItem in cbEstado.Items)
                    {
                        if (comboItem.Tag.ToString() == item.Estado)
                        {
                            cbEstado.SelectedItem = comboItem;
                            break;
                        }
                    }

                    MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "ItemDialog");
                }
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            string descricao = txtItemDescricao.Text.Trim();
            
            if (string.IsNullOrEmpty(descricao))
            {
                MessageBox.Show("A descrição é obrigatória.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbSala.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma sala para vincular este item.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int salaId = (int)((ComboBoxItem)cbSala.SelectedItem).Tag;

            bool success;
            if (_editingId == -1)
            {
                // New
                // O método original é AdminService.CriarIntens ou IntensService.CriarIntens? (Modificamos o AdminService que era chamado de IntensService)
                // Let's call the updated AdminService
                success = AdminService.CriarIntens(descricao, salaId);
            }
            else
            {
                // Edit
                string estado = ((ComboBoxItem)cbEstado.SelectedItem)?.Tag.ToString();
                success = AdminService.EditarIntens(_editingId, descricao, estado, salaId);
            }

            if (success)
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                LoadData();
            }
            else
            {
                MessageBox.Show("Ocorreu um erro ao salvar o item. Pode ser que a descrição já exista.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ItemAdminUI
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string SalaNome { get; set; }
        public string Estado { get; set; }
        public string BadgeColor { get; set; }
    }
}
