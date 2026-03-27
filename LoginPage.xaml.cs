using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projeto_Senai
{
    public partial class LoginPage : page
    {
        // string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Para testar a navegação IMEDIATAMENTE sem banco de dados, 
            // comentei a validação do SQL. Quando for ligar o banco, descomente!

            /*
            if (VerifyUser(txtUsername.Text, txtPassword.Password))
            {
                NavigationService.Navigate(new Dashboard());
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorretos", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            */

            // Navegação direta para teste (Copiar e colar e já ver funcionando):
          
        }

        /* private bool VerifyUser(string username, string password)
        {
             // SEU CÓDIGO SQL ORIGINAL VEM AQUI
        }
        */

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}