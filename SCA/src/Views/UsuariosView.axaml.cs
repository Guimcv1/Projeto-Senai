using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class UsuariosView : UserControl
{
    private JanelaPrincipal? _parent;
    private int _editingId = -1;

    public UsuariosView()
    {
        InitializeComponent();
    }

    public UsuariosView(JanelaPrincipal parent) : this()
    {
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
            Console.WriteLine($"Erro ao carregar usuários: {ex.Message}");
        }
    }

    private void NovoUsuario_Click(object sender, RoutedEventArgs e)
    {
        _editingId = -1;
        txtDialogTitle.Text = "Novo Utilizador";
        txtNome.Text = "";
        txtLogin.Text = "";
        txtSenha.Text = "";
        chkIsAdmin.IsChecked = false;
        chkIsAtivo.IsChecked = true;
        UsuarioDialogOverlay.IsVisible = true;
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
                txtSenha.Text = ""; 
                chkIsAdmin.IsChecked = user.IsAdmin;
                chkIsAtivo.IsChecked = user.IsAtivo;
                UsuarioDialogOverlay.IsVisible = true;
            }
        }
    }

    private void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        UsuarioDialogOverlay.IsVisible = false;
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        string nome = txtNome.Text?.Trim() ?? "";
        string login = txtLogin.Text?.Trim() ?? "";
        string senha = txtSenha.Text ?? "";
        bool isAdmin = chkIsAdmin.IsChecked ?? false;
        bool isAtivo = chkIsAtivo.IsChecked ?? true;

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(login))
        {
            Console.WriteLine("Nome e Login são obrigatórios.");
            return;
        }

        bool success;
        if (_editingId == -1)
        {
            if (string.IsNullOrEmpty(senha))
            {
                Console.WriteLine("A senha é obrigatória para um novo usuário.");
                return;
            }

            success = UsuarioService.CriarUser(nome, login, senha, isAdmin, isAtivo);
        }
        else
        {
            string? novaSenha = string.IsNullOrEmpty(senha) ? null : senha;
            success = UsuarioService.AtualizarUser(_editingId, nome, login, novaSenha, isAdmin, isAtivo);
        }

        if (success)
        {
            UsuarioDialogOverlay.IsVisible = false;
            LoadData();
        }
        else
        {
            Console.WriteLine("Ocorreu um erro ao salvar o usuário.");
        }
    }

    private void Inativar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is UsuarioUI userUI)
        {
            bool novoStatus = userUI.StatusText == "Inativo";
            if (UsuarioService.InativarAtivarUser(userUI.Id, novoStatus))
            {
                LoadData();
            }
        }
    }
}

public class UsuarioUI
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Login { get; set; } = "";
    public string Perfil { get; set; } = "";
    public string StatusText { get; set; } = "";
    public string BadgeColor { get; set; } = "";
}
