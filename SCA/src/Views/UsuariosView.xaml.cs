using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SCA.Back.Services;

namespace SCA.Views
{
    public partial class UsuariosView : UserControl
    {
        private MainWindow _parent;
        private int _editingId = -1;

        public UsuariosView(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var usuarios = UsuarioService.ListarUser();
                var listUI = new List<UsuarioUI>();

                foreach (var u in usuarios)
                {
                    listUI.Add(new UsuarioUI
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Login = u.Login,
                        Perfil = u.IsAdmin ? "Administrador" : "Usuário Comum",
                        StatusText = u.IsAtivo ? "Ativo" : "Inativo",
                        BadgeColor = u.IsAtivo ? "#16A34A" : "#94A3B8"
                    });
                }

                dgUsuarios.ItemsSource = listUI;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}");
            }
        }

        private void NovoUsuario_Click(object sender, RoutedEventArgs e)
        {
            _editingId = -1;
            txtDialogTitle.Text = "Novo Utilizador";
            txtNome.Text = "";
            txtLogin.Text = "";
            txtSenha.Password = "";
            chkIsAdmin.IsChecked = false;
            chkIsAtivo.IsChecked = true;
            
            MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "UsuarioDialog");
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UsuarioUI userUI)
            {
                var user = UsuarioService.BuscarPorIdUser(userUI.Id);
                if (user != null)
                {
                    _editingId = user.Id;
                    txtDialogTitle.Text = "Editar Utilizador";
                    txtNome.Text = user.Nome;
                    txtLogin.Text = user.Login;
                    txtSenha.Password = ""; // Empty on edit unless they want to change
                    chkIsAdmin.IsChecked = user.IsAdmin;
                    chkIsAtivo.IsChecked = user.IsAtivo;

                    MaterialDesignThemes.Wpf.DialogHost.Show(MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog") == null ? null : MaterialDesignThemes.Wpf.DialogHost.GetDialogSession("RootDialog").DialogContent, "UsuarioDialog");
                }
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string login = txtLogin.Text.Trim();
            string senha = txtSenha.Password;
            bool isAdmin = chkIsAdmin.IsChecked ?? false;
            bool isAtivo = chkIsAtivo.IsChecked ?? true;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Nome e Login são obrigatórios.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success;
            if (_editingId == -1)
            {
                // New
                if (string.IsNullOrEmpty(senha))
                {
                    MessageBox.Show("A senha é obrigatória para um novo usuário.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                success = UsuarioService.CriarUser(nome, login, senha, isAdmin, isAtivo);
            }
            else
            {
                // Edit
                string novaSenha = string.IsNullOrEmpty(senha) ? null : senha;
                success = UsuarioService.AtualizarUser(_editingId, nome, login, novaSenha, isAdmin, isAtivo);
            }

            if (success)
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                LoadData();
            }
            else
            {
                MessageBox.Show("Ocorreu um erro ao salvar o usuário. Pode ser que o login já exista.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class UsuarioUI
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Perfil { get; set; }
        public string StatusText { get; set; }
        public string BadgeColor { get; set; }
    }
}
