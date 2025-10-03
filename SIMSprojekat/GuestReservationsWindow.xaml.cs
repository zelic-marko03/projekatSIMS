using SIMSprojekat.Models;
using SIMSprojekat.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SIMSprojekat
{
    public partial class GuestReservationsWindow : Window
    {
        private readonly Korisnik _gost;
        private readonly RezervacijaService _svc = new RezervacijaService();

        // Za Designer – ne zovi InitializeComponent ovde
        public GuestReservationsWindow() : this(null) { }

        public GuestReservationsWindow(Korisnik gost)
        {
            InitializeComponent();
            _gost = gost ?? new Korisnik { Email = "" };

            if (StatusBox.Items.Count > 0) StatusBox.SelectedIndex = 0;
            LoadData(GetFilter());
        }

        private void LoadData(StatusRezervacije? filter)
        {
            var email = _gost?.Email ?? "";
            RezGrid.ItemsSource = _svc.MojeRezervacije(email, filter);
        }

        private StatusRezervacije? GetFilter()
        {
            var tag = (StatusBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            return tag switch
            {
                "NaCekanju" => StatusRezervacije.NaCekanju,
                "Potvrdjena" => StatusRezervacije.Potvrdjena,
                "Odbijena" => StatusRezervacije.Odbijena,
                _ => (StatusRezervacije?)null
            };
        }

        private void StatusBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => LoadData(GetFilter());

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            if (RezGrid.SelectedItem is not Rezervacija r)
            {
                MessageBox.Show("Izaberite rezervaciju.");
                return;
            }

            var (ok, poruka) = _svc.Otkazi(r.Id);   // ASCII naziv
            MessageBox.Show(poruka);
            LoadData(GetFilter());
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
