using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using SCA.Core.Models;
using System;
using System.Linq;

namespace SCA.Views;

public partial class MainWindow : Window
{
    public bool IsAdminMode { get; set; } = false;
    public Usuario? CurrentAdmin { get; set; } = null;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(Usuario adminUser) : this()
    {
        if (adminUser != null && adminUser.IsAdmin)
        {
            IsAdminMode = true;
            CurrentAdmin = adminUser;
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        UpdateUIRole();
        LoadView(new DashboardView(this));
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        ResetMenuButtons();
        if (sender is Button btn)
        {
            btn.Background = new SolidColorBrush(Color.FromArgb(38, 255, 255, 255));
            btn.BorderThickness = new Thickness(4, 0, 0, 0);
            btn.BorderBrush = Brush.Parse("#EA580C");
            
            if (btn == btnVisaoGeral) LoadView(new DashboardView(this));
            else if (btn == btnAprovacoes) LoadView(new AprovacoesView(this));
            else if (btn == btnLocais) LoadView(new LocaisView(this));
            else if (btn == btnItens) LoadView(new ItensView(this));
            else if (btn == btnUsuarios) LoadView(new UsuariosView(this));
            else if (btn == btnRelatorios) LoadView(new RelatoriosView(this));
        }
    }

    private void ResetMenuButtons()
    {
        var buttons = new[] { btnVisaoGeral, btnAprovacoes, btnLocais, btnItens, btnUsuarios, btnRelatorios };
        foreach (var b in buttons)
        {
            if (b != null)
            {
                b.Background = Brushes.Transparent;
                b.BorderThickness = new Thickness(0);
            }
        }
    }

    public void LoadView(UserControl view)
    {
        if (MainContent != null)
        {
            MainContent.Content = view;
        }
    }

    private void BtnAuth_Click(object sender, RoutedEventArgs e)
    {
        if (IsAdminMode)
        {
            IsAdminMode = false;
            CurrentAdmin = null;
            UpdateUIRole();
            
            ResetMenuButtons();
            if (btnVisaoGeral != null)
            {
                btnVisaoGeral.Background = new SolidColorBrush(Color.FromArgb(38, 255, 255, 255));
                btnVisaoGeral.BorderThickness = new Thickness(4, 0, 0, 0);
                btnVisaoGeral.BorderBrush = Brush.Parse("#EA580C");
            }
            LoadView(new DashboardView(this));
        }
        else
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }
    }

    public void UpdateUIRole()
    {
        if (btnAprovacoes == null) return;

        btnAprovacoes.IsVisible = IsAdminMode;
        btnLocais.IsVisible = IsAdminMode;
        btnItens.IsVisible = IsAdminMode;
        btnUsuarios.IsVisible = IsAdminMode;
        btnRelatorios.IsVisible = IsAdminMode;
        
        if (UserBadge != null) UserBadge.IsVisible = IsAdminMode;

        if (IsAdminMode)
        {
            if (txtUserNameDisplay != null) txtUserNameDisplay.Text = $"Admin ({CurrentAdmin?.Nome})";
            if (txtAuth != null) txtAuth.Text = "Sair do Sistema";
            if (btnAuth != null)
            {
                btnAuth.Foreground = Brushes.White;
                btnAuth.Background = Brush.Parse("#dc2626");
                btnAuth.BorderThickness = new Thickness(0);
            }
            if (iconAuth != null)
            {
                iconAuth.Kind = Material.Icons.MaterialIconKind.Logout;
                iconAuth.Foreground = Brushes.White;
            }
        }
        else
        {
            if (txtAuth != null) txtAuth.Text = "Acesso Administrativo";
            if (btnAuth != null)
            {
                btnAuth.Foreground = Brush.Parse("#002776");
                btnAuth.Background = Brushes.Transparent;
                btnAuth.BorderThickness = new Thickness(2);
            }
            if (iconAuth != null)
            {
                iconAuth.Kind = Material.Icons.MaterialIconKind.AccountLock;
                iconAuth.Foreground = Brush.Parse("#002776");
            }
        }
    }
    public void ShowMessage(string msg, bool isError = true)
    {
        if (txtNotification == null || NotificationBar == null) return;

        txtNotification.Text = msg;
        NotificationBar.Background = isError ? Brush.Parse("#DC2626") : Brush.Parse("#16A34A");
        if (iconNotification != null) iconNotification.Kind = isError ? Material.Icons.MaterialIconKind.AlertCircle : Material.Icons.MaterialIconKind.CheckCircle;
        
        NotificationBar.IsVisible = true;

        // Auto-hide after 5 seconds
        var timer = new System.Timers.Timer(5000);
        timer.Elapsed += (s, e) => {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                NotificationBar.IsVisible = false;
            });
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    private void CloseNotification_Click(object sender, RoutedEventArgs e)
    {
        if (NotificationBar != null) NotificationBar.IsVisible = false;
    }
}
