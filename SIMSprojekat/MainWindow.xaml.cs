using SIMSprojekat.Models;
using SIMSprojekat.Services;
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

namespace SIMSprojekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _auth = new AuthService();

        public MainWindow()
        {
            InitializeComponent();
            _auth.EnsureDemoUsers(); // kreira admina ako ga nema (admin@sims.local / admin)
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var user = _auth.Login(EmailBox.Text, PassBox.Password);
            if (user == null)
            {
                MessageBox.Show("Pogrešan email ili lozinka.");
                return;
            }

            // Otvori odgovarajući prozor po ulozi
            Window next = user.Tip switch
            {
                TipKorisnika.Administrator => new AdminWindow(user),
                TipKorisnika.Vlasnik => new OwnerWindow(user),
                TipKorisnika.Gost => new GuestWindow(user),
                _ => null
            };
          

            if (next != null)
            {
                next.Show();
                Close();
            }

        }
        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var win = new RegisterWindow { Owner = this };
            win.ShowDialog();
        }
    }
}