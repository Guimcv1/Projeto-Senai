using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SCA.Core.Services;
using SCA.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class UsuariosView : UserControl, IReloadableView
{
    private JanelaPrincipal? _parent;
    private int _editingId = -1;
    private List<SCA.Core.Models.Usuario> _todosUsuarios = new();

    public UsuariosView()
    {
        InitializeComponent();
    }

    public UsuariosView(JanelaPrincipal parent) : this()
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
            _todosUsuarios = UsuarioService.ListarUser();
            
            if (txtSearch != null)
            {
                var nomesELogins = _todosUsuarios.Select(u => u.Nome)
                                                 .Concat(_todosUsuarios.Select(u => u.Login))
                                                 .Concat(new[] { "Ativo", "Inativo" })
                                                 .Where(s => !string.IsNullOrWhiteSpace(s))
                                                 .Distinct()
                                                 .ToList();
                txtSearch.ItemsSource = nomesELogins;
                txtSearch.ItemFilter = SearchFilter;
            }

            FilterData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar usuários: {ex.Message}");
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

    private bool SearchFilter(string searchText, object item)
    {
        if (item is string str)
        {
            return string.IsNullOrEmpty(searchText) || NormalizeString(str).Contains(NormalizeString(searchText));
        }
        return false;
    }

    private void FilterData()
    {
        if (dgUsuarios == null) return;
        string searchText = NormalizeString(txtSearch?.Text ?? "");

        var listUI = new List<UsuarioUI>();

        var filtrados = _todosUsuarios.Where(u =>
            string.IsNullOrEmpty(searchText) ||
            NormalizeString(u.Nome).Contains(searchText) ||
            NormalizeString(u.Login).Contains(searchText) ||
            NormalizeString(u.IsAtivo ? "Ativo" : "Inativo").Contains(searchText)
        );

        foreach (var u in filtrados)
            {
                listUI.Add(new UsuarioUI
                {
                    Id = u.Id,
                    Nome = u.Nome?.ToUpper() ?? "",
                    Login = u.Login,
                    Perfil = u.IsAdmin ? "Administrador" : "Usuário Comum",
                    StatusText = u.IsAtivo ? "Ativo" : "Inativo",
                    BadgeColor = u.IsAtivo ? "#16A34A" : "#94A3B8"
                });
            }

            dgUsuarios.ItemsSource = listUI;
    }

    private void Search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        FilterData();
    }

    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        if (txtSearch == null) return;

        txtSearch.Text = "";
        FilterData();
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
        string nome = txtNome.Text?.Trim().ToUpper() ?? "";
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
            int adminId = _parent?.CurrentAdmin?.Id ?? 0;
            if (adminId > 0)
            {
                string acao = _editingId == -1 ? "Criou novo usuário" : "Editou usuário";
                LogService.RegistrarLog($"{acao}: {login}", AcaoTipo.Usuario, adminId);
            }
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
                int adminId = _parent?.CurrentAdmin?.Id ?? 0;
                if (adminId > 0)
                {
                    LogService.RegistrarLog($"Alterou status do usuário ID {userUI.Id}", AcaoTipo.Usuario, adminId);
                }
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
