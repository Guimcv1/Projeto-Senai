using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using SCA.Core.Services;
using SCA.Core.Models;
using System;

namespace SCA.Views;

public partial class TelaLoginView : Window
{
    private JanelaPrincipal? _parent;

    public TelaLoginView()
    {
        InitializeComponent();
    }

    public TelaLoginView(JanelaPrincipal parent) : this()
    {
        _parent = parent;
    }

    private void Border_MouseDown(object sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine($"Tentativa de login: {txtUsername?.Text}");
        if (txtUsername == null || txtPassword == null)
        {
            Console.WriteLine("Erro: Controles de texto não inicializados!");
            return;
        }

        var usuario = UsuarioService.LoginUser(txtUsername.Text ?? "", txtPassword.Text ?? "");
        if (usuario != null)
        {
            Console.WriteLine("Login realizado com sucesso. Atualizando JanelaPrincipal...");
            if (_parent != null)
            {
                _parent.IsAdminMode = true;
                _parent.CurrentAdmin = usuario;
                _parent.UpdateUIRole();
                if (usuario.Id > 0)
                {
                    LogService.RegistrarLog("Login ao sistema", AcaoTipo.Usuario, usuario.Id);
                }
                this.Close();
            }
            else
            {
                JanelaPrincipal mainWindow = new JanelaPrincipal(usuario);
                mainWindow.Show();
                this.Close();
            }
        }
        else
        {
            Console.WriteLine("Usuário ou senha incorretos.");
            // Optional: show a message on the login screen itself
        }
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
