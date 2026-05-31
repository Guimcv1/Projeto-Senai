using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using SCA.Core.Services;
using System;

namespace SCA.Views;

public partial class TelaLoginView : Window
{
    private JanelaPrincipal? _parent;

    private void RegistrarAcao(string acao, int? usuarioId = null)
    {
        int id = usuarioId ?? _parent?.CurrentAdmin?.Id ?? 0;
        if (id > 0)
        {
            SCA.Core.Services.LogService.RegistrarLog(acao, "Usuario", id);
        }
    }

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
            RegistrarAcao($"Login realizado por {usuario.Login}", usuario.Id);
            Console.WriteLine("Login realizado com sucesso. Atualizando JanelaPrincipal...");
            if (_parent != null)
            {
                _parent.IsAdminMode = true;
                _parent.CurrentAdmin = usuario;
                _parent.UpdateUIRole();
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
            var usuarioTentativa = UsuarioService.ListarUser().FirstOrDefault(u => u.Login == (txtUsername.Text ?? ""));
            if (usuarioTentativa != null)
            {
                RegistrarAcao($"Tentativa de login com falha para {usuarioTentativa.Login}", usuarioTentativa.Id);
            }
            // Optional: show a message on the login screen itself
        }
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
        RegistrarAcao("Fechou tela de login");
        this.Close();
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        RegistrarAcao("Voltou na tela de login");
        this.Close();
    }
}
