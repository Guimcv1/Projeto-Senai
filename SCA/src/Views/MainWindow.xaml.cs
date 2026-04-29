using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SCA
{
    public partial class MainWindow : Window
    {
        public bool IsAdminMode { get; set; } = false;
        public SCA.Back.Data.Usuario CurrentAdmin { get; set; } = null;

        public MainWindow(SCA.Back.Data.Usuario adminUser = null)
        {
            InitializeComponent();
            
            if (adminUser != null && adminUser.Pefil == "Admin")
            {
                IsAdminMode = true;
                CurrentAdmin = adminUser;
            }

            UpdateUIRole();
            LoadView(new Views.DashboardView(this));
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset background for all buttons
            ResetMenuButtons();
            var btn = sender as Button;
            if (btn != null)
            {
                btn.Background = new SolidColorBrush(Color.FromArgb(38, 255, 255, 255)); // rgba(255,255,255,0.15)
                btn.BorderThickness = new Thickness(4, 0, 0, 0);
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA580C"));
                
                if (btn == btnVisaoGeral) LoadView(new Views.DashboardView(this));
                else if (btn == btnAprovacoes) LoadView(new Views.AprovacoesView(this));
                else if (btn == btnLocais) LoadView(new Views.LocaisView(this));
                else if (btn == btnItens) LoadView(new Views.ItensView(this));
                else if (btn == btnUsuarios) LoadView(new Views.UsuariosView(this));
                else if (btn == btnRelatorios) LoadView(new Views.RelatoriosView(this));
            }
        }

        private void ResetMenuButtons()
        {
            var buttons = new[] { btnVisaoGeral, btnAprovacoes, btnLocais, btnItens, btnUsuarios, btnRelatorios };
            foreach (var b in buttons)
            {
                b.Background = Brushes.Transparent;
                b.BorderThickness = new Thickness(0);
            }
        }

        public void LoadView(UserControl view)
        {
            MainContent.Content = view;
        }

        private void BtnAuth_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdminMode)
            {
                IsAdminMode = false;
                CurrentAdmin = null;
                UpdateUIRole();
                
                ResetMenuButtons();
                btnVisaoGeral.Background = new SolidColorBrush(Color.FromArgb(38, 255, 255, 255));
                btnVisaoGeral.BorderThickness = new Thickness(4, 0, 0, 0);
                btnVisaoGeral.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA580C"));
                LoadView(new Views.DashboardView(this));
            }
            else
            {
                // Open Admin Login Dialog (LoginPage actually acts as a login window for both Admin and User)
                // For simplicity, we open LoginPage
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
                this.Close();
            }
        }

        public void UpdateUIRole()
        {
            var adminVisibility = IsAdminMode ? Visibility.Visible : Visibility.Collapsed;
            
            btnAprovacoes.Visibility = adminVisibility;
            btnLocais.Visibility = adminVisibility;
            btnItens.Visibility = adminVisibility;
            btnUsuarios.Visibility = adminVisibility;
            btnRelatorios.Visibility = adminVisibility;
            
            UserBadge.Visibility = adminVisibility;

            if (IsAdminMode)
            {
                txtUserNameDisplay.Text = $"Admin ({CurrentAdmin?.Nome})";
                txtAuth.Text = "Sair do Sistema";
                btnAuth.Foreground = Brushes.White;
                btnAuth.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc2626"));
                btnAuth.BorderThickness = new Thickness(0);
                iconAuth.Kind = MaterialDesignThemes.Wpf.PackIconKind.Logout;
                iconAuth.Foreground = Brushes.White;
            }
            else
            {
                txtAuth.Text = "Acesso Administrativo";
                btnAuth.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#002776"));
                btnAuth.Background = Brushes.Transparent;
                btnAuth.BorderThickness = new Thickness(2);
                iconAuth.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountLock;
                iconAuth.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#002776"));
            }
        }
    }
}