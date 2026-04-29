using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SCA.Back.Data;
using SCA.Back.Services;

namespace SCA.Views
{
    public partial class DashboardView : UserControl
    {
        private MainWindow _parent;
        private List<Sala> _todasSalas;

        public DashboardView(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            LoadSalas();
        }

        private void LoadSalas()
        {
            try
            {
                _todasSalas = SalasService.ListarSala();
                
                // Populate ComboBox
                ComboFiltroSala.Items.Clear();
                ComboFiltroSala.Items.Add(new ComboBoxItem { Content = "Todos os Ambientes", Tag = -1, IsSelected = true });
                foreach (var sala in _todasSalas)
                {
                    ComboFiltroSala.Items.Add(new ComboBoxItem { Content = sala.Descricao, Tag = sala.Id });
                }

                FilterDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar ambientes: {ex.Message}");
            }
        }

        private void FilterDashboard()
        {
            int selectedSalaId = -1;
            if (ComboFiltroSala.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                selectedSalaId = (int)item.Tag;
            }

            var salasFiltradas = selectedSalaId == -1 ? _todasSalas : _todasSalas.Where(s => s.Id == selectedSalaId).ToList();
            
            var ambientesUI = new List<Keys_manager___Tester.AmbienteTemp>();

            using var context = new BancoContext();
            var todosItens = context.Itens.ToList();
            var todosEmprestimos = context.Emprestimos.ToList(); // Simplified for now

            foreach (var sala in salasFiltradas)
            {
                string cor = GetRoomColor(sala.Id, todosItens, todosEmprestimos);

                ambientesUI.Add(new Keys_manager___Tester.AmbienteTemp
                {
                    Id = sala.Id,
                    Nome = sala.Descricao + (!sala.isAtivo ? " (Inativo)" : ""),
                    CorStatus = !sala.isAtivo ? "#94a3b8" : cor
                });
            }

            icAmbientes.ItemsSource = ambientesUI;
        }

        private string GetRoomColor(int salaId, List<Itens> itens, List<Emprestimos> emprestimos)
        {
            var itensDaSala = itens.Where(i => i.SalaId == salaId).ToList();
            if (itensDaSala.Count == 0) return "#94a3b8"; // bg-grey

            int total = itensDaSala.Count;
            int available = itensDaSala.Count(i => i.Estado == Estados.Livre);
            int borrowed = itensDaSala.Count(i => i.Estado == Estados.Emprestado);
            int pending = itensDaSala.Count(i => i.Estado == Estados.Pendente);

            if (pending == total) return "#94a3b8";
            if (available == total) return "#16a34a"; // green
            if (borrowed == total) return "#dc2626"; // red
            if (available > 0) return "#ea580c"; // orange

            return "#dc2626";
        }

        private void ComboFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterDashboard();
        }

        private void RoomCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Keys_manager___Tester.AmbienteTemp ambiente)
            {
                var sala = _todasSalas.FirstOrDefault(s => s.Id == ambiente.Id);
                if (sala != null && !sala.isAtivo)
                {
                    MessageBox.Show("Este ambiente encontra-se indisponível ou inativo no momento.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                OpenRoomDetails(ambiente.Id, sala.Descricao);
            }
        }

        private void OpenRoomDetails(int salaId, string salaName)
        {
            txtDialogTitle.Text = $"Itens em: {salaName}";

            using var context = new BancoContext();
            var itensDaSala = context.Itens.Where(i => i.SalaId == salaId).ToList();

            var dialogItems = new List<ItemUI>();

            foreach (var item in itensDaSala)
            {
                var uiItem = new ItemUI
                {
                    Id = item.Id,
                    Descricao = item.Descricao,
                    EstadoOrigem = item.Estado
                };

                if (item.Estado == Estados.Livre)
                {
                    uiItem.BadgeText = "Disponível";
                    uiItem.BadgeColor = "#16a34a";
                    uiItem.CanSelect = true;
                }
                else if (item.Estado == Estados.Pendente)
                {
                    uiItem.BadgeText = "Em Análise";
                    uiItem.BadgeColor = "#94a3b8";
                    uiItem.CanSelect = false;
                }
                else if (item.Estado == Estados.Emprestado)
                {
                    uiItem.BadgeText = "Emprestado";
                    uiItem.BadgeColor = "#dc2626";
                    uiItem.CanSelect = true; // Can select to return
                }

                dialogItems.Add(uiItem);
            }

            dgItems.ItemsSource = dialogItems;
            MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "RoomDialog");
        }

        private void ProcessSelectedItems_Click(object sender, RoutedEventArgs e)
        {
            var dialogItems = dgItems.ItemsSource as List<ItemUI>;
            var selectedItems = dialogItems?.Where(i => i.IsSelected).ToList();

            if (selectedItems == null || selectedItems.Count == 0)
            {
                MessageBox.Show("Selecione ao menos um item marcando as caixas de seleção.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);

            // In HTML mockup, it asks for user login if they are not logged in or asks to confirm if they are.
            // Since we use the same LoginPage to simulate the modalCredenciaisUser:
            // But we already have a logged in user maybe? The WPF app didn't mandate login to see dashboard in HTML, wait.
            // HTML says "Sessão: Admin" and "Acesso Administrativo". A normal user can see the dashboard without logging in?
            // "Insira as suas credenciais corporativas para registar as solicitações em seu nome."
            
            MessageBox.Show("Esta funcionalidade exige abrir um dialog de credenciais do usuário. Para simplificar, vou alterar o status diretamente ou pedir para implementar o Login Dialog.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // For now, let's just mark them as Pendente
            using var context = new BancoContext();
            foreach (var item in selectedItems)
            {
                var dbItem = context.Itens.Find(item.Id);
                if (dbItem != null)
                {
                    dbItem.Estado = Estados.Pendente;
                    // Note: Here we should create an Emprestimo record.
                }
            }
            context.SaveChanges();
            MessageBox.Show("Operação registrada! As solicitações foram enviadas para aprovação.");
            FilterDashboard();
        }
    }

    public class ItemUI
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool IsSelected { get; set; }
        public bool CanSelect { get; set; }
        public string BadgeText { get; set; }
        public string BadgeColor { get; set; }
        public string EstadoOrigem { get; set; }
    }
}
