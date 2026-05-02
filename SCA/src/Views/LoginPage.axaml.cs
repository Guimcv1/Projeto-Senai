using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using SCA.Core.Services;
using System;

namespace SCA.Views;

public partial class LoginPage : Window
{
    public LoginPage()
    {
        InitializeComponent();
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
            Console.WriteLine("Login realizado com sucesso. Abrindo MainWindow...");
            MainWindow mainWindow = new MainWindow(usuario);
            mainWindow.Show();
            this.Close();
        }
        else
        {
            Console.WriteLine("Usuário ou senha incorretos.");
        }
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}
