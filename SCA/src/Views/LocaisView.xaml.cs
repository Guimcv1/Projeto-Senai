using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Views
{
    public partial class LocaisView : UserControl
    {
        private MainWindow _parent;
        private int _editingId = -1;

        public LocaisView(MainWindow parent)
        {
            InitializeComponent();
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
                MessageBox.Show($"Erro ao carregar locais: {ex.Message}");
            }
        }

        private void NovoLocal_Click(object sender, RoutedEventArgs e)
        {
            _editingId = -1;
            txtDialogTitle.Text = "Novo Local";
            txtLocalNome.Text = "";
            chkIsAtivo.IsChecked = true;
            MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "LocalDialog");
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
                    MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "LocalDialog");
                }
            }
        }

        private void Remover_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LocalUI local)
            {
                var result = MessageBox.Show($"Tem certeza que deseja remover o local '{local.Descricao}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SalasService.RemoverSala(local.Id))
                    {
                        LoadLocais();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível remover o local. Verifique se existem itens vinculados.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtLocalNome.Text.Trim();
            bool isAtivo = chkIsAtivo.IsChecked ?? false;

            if (string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("O nome do local é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success;
            if (_editingId == -1)
            {
                // New
                success = SalasService.CriarSala(nome);
                // Note: SalasService.CriarSala automatically sets isAtivo to true, we might need to change it if isAtivo == false
                if (success && !isAtivo)
                {
                    var nova = SalasService.ListarSala().FirstOrDefault(s => s.Descricao == nome);
                    if (nova != null) SalasService.AtivarDesativar(nova.Id);
                }
            }
            else
            {
                // Edit
                success = SalasService.EditarSala(_editingId, nome);
                var sala = SalasService.BuscarPorIdSala(_editingId);
                if (sala != null && sala.isAtivo != isAtivo)
                {
                    SalasService.AtivarDesativar(_editingId);
                }
            }

            if (success)
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                LoadLocais();
            }
            else
            {
                MessageBox.Show("Ocorreu um erro ao salvar o local. Tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class LocalUI
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string StatusText { get; set; }
        public string BadgeColor { get; set; }
    }
}
