using SIMSprojekat.Models;
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
using System.Windows.Shapes;

namespace SIMSprojekat
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly Korisnik _admin;
        public AdminWindow(Korisnik admin)
        {
            InitializeComponent();
            _admin = admin;
            Title = $"Admin panel - {_admin.Ime} {_admin.Prezime}";
        }
      
        private void OpenRegisterOwner_Click(object sender, RoutedEventArgs e)
        => new RegisterOwnerWindow { Owner = this }.ShowDialog();

        private void OpenAddHotel_Click(object sender, RoutedEventArgs e)
            => new AddHotelWindow { Owner = this }.ShowDialog();
    }
}
