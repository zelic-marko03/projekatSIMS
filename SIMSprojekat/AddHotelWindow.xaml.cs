using System.Windows;
using SIMSprojekat.Models;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class AddHotelWindow : Window
    {
        private readonly AdminService _svc = new AdminService();
        public AddHotelWindow() { InitializeComponent(); }
        private void Cancel_Click(object s, RoutedEventArgs e) => Close();

        private void Save_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SifraBox.Text) ||
                string.IsNullOrWhiteSpace(ImeBox.Text) ||
                !int.TryParse(GodBox.Text, out var god) ||
                !int.TryParse(ZvezBox.Text, out var zvez) ||
                string.IsNullOrWhiteSpace(VlasnikBox.Text))
            { MessageBox.Show("Popunite sva polja (godina/zvezdice kao brojevi)."); return; }

            var h = new Hotel
            {
                Sifra = SifraBox.Text.Trim(),
                Ime = ImeBox.Text.Trim(),
                GodinaIzgradnje = god,
                BrojZvezdica = zvez,
                VlasnikJMBG = VlasnikBox.Text.Trim()
            };

            var (ok, poruka) = _svc.AddHotel(h);
            MessageBox.Show(poruka);
            if (ok) Close();
        }
    }
}
