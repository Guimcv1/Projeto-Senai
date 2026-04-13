using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CarregarAmbientes();
        }

        private void CarregarAmbientes()
        {
            try
            {
                var salas = SCA.Back.Services.SalasService.ListarSala();
                var ambientes = new System.Collections.Generic.List<Keys_manager___Tester.AmbienteTemp>();

                string filtro = "";
                if (ComboFiltro != null && ComboFiltro.SelectedItem is ComboBoxItem item)
                {
                    filtro = item.Content.ToString();
                }

                foreach (var sala in salas)
                {
                  
                    string cor = sala.isAtivo ? "#16a34a" : "#94a3b8";
                    string statusTxt = sala.isAtivo ? "Disponíveis" : "Manutenção";

                    // Filtro
                    if (filtro == "Disponíveis" && statusTxt != "Disponíveis") continue;
                    if (filtro == "Em Uso" && statusTxt != "Em Uso") continue;
                    if (filtro == "Manutenção" && statusTxt != "Manutenção") continue;

                    ambientes.Add(new Keys_manager___Tester.AmbienteTemp
                    {
                        Id = sala.Id,
                        Nome = sala.Descricao,
                        CorStatus = cor
                    });
                }

                if (icAmbientes != null)
                {
                    icAmbientes.ItemsSource = ambientes;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao carregar ambientes: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }

        private void ComboFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregarAmbientes();
        }

    }
}