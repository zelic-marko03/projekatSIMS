using System.Linq;
using System.Windows;
using System.Windows.Controls; // zbog ComboBoxItem
using SIMSprojekat.Models;     // zbog Korisnik
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class GuestWindow : Window
    {
        private readonly HotelService _svc = new HotelService();
        private readonly Korisnik _gost; // ulogovani gost (opciono)

        // potreban XAML/Designer-u
        public GuestWindow() : this(null) { }

        // koristi se iz MainWindow-a: new GuestWindow(user)
        public GuestWindow(Korisnik gost)
        {
            InitializeComponent();
            _gost = gost;

            if (_gost != null)
                Title = $"Gost – {_gost.Ime} {_gost.Prezime}";

            // ako želiš demo podatke:
            // _svc.EnsureSeed();

            // podesi default operator za apartmane
            if (OpBox.Items.Count > 0 && OpBox.SelectedIndex < 0) OpBox.SelectedIndex = 0;

            LoadAll();
        }

        // ako u XAML-u imaš Loaded="Window_Loaded", ostavi i ovo
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OpBox.Items.Count > 0 && OpBox.SelectedIndex < 0) OpBox.SelectedIndex = 0;
            LoadAll();
        }

        private void LoadAll()
        {
            // ako želiš da gost vidi samo odobrene hotele,
            // zameni sledeću liniju sa: HotelsGrid.ItemsSource = _svc.GetAllForGuest(SortStars.IsChecked == true);
            HotelsGrid.ItemsSource = _svc.GetAll(SortStars.IsChecked == true);
        }

        // povezati u XAML-u na SortStars: Checked/Unchecked="SortStars_Click"
        private void SortStars_Click(object sender, RoutedEventArgs e) => Apply_Click(sender, e);

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            bool sort = SortStars.IsChecked == true;

            // IME
            var nameQ = (NameBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(nameQ))
            {
                var r = _svc.SearchByName(nameQ);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // GODINA
            var yearQ = (YearBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(yearQ))
            {
                if (!int.TryParse(yearQ, out var year)) { MessageBox.Show("Unesi validnu godinu."); return; }
                var r = _svc.SearchByYear(year);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // ZVEZDICE
            var starsQ = (StarsBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(starsQ))
            {
                if (!int.TryParse(starsQ, out var stars)) { MessageBox.Show("Unesi broj zvezdica."); return; }
                var r = _svc.SearchByStars(stars);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // APARTMANI
            int? rooms = int.TryParse((RoomsBox.Text ?? "").Trim(), out var rr) ? rr : (int?)null;
            int? guests = int.TryParse((GuestsBox.Text ?? "").Trim(), out var gg) ? gg : (int?)null;
            var op = ((OpBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() == "|") ? '|' : '&';

            if (rooms != null || guests != null)
            {
                var r = _svc.SearchByApartments(rooms, guests, op);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // Ako ništa nije uneseno → prikaži sve
            LoadAll();
        }

        private void NovaRezervacija_Click(object sender, RoutedEventArgs e)
        {
            if (HotelsGrid.SelectedItem == null)
            {
                MessageBox.Show("Izaberite hotel iz liste.");
                return;
            }

            // Ovde otvori tvoj prozor za kreiranje rezervacije (hotel je selektovan):
            // var hotel = (Hotel)HotelsGrid.SelectedItem;
            // new RezervacijaWindow(hotel, _gost)?.ShowDialog();
        }
    }
}
