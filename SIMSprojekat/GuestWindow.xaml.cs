using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SIMSprojekat.Models;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class GuestWindow : Window
    {
        private readonly HotelService _svc = new HotelService();
        private readonly Korisnik _gost; // ulogovani gost (opciono)

        // Za designer
        public GuestWindow() : this(null) { }

        // Pozivaj iz MainWindow-a: new GuestWindow(user)
        public GuestWindow(Korisnik gost)
        {
            InitializeComponent();
            _gost = gost;

            if (_gost != null)
                Title = $"Gost – {_gost.Ime} {_gost.Prezime}";

            // Default operator za pretragu po apartmanima
            if (OpBox.Items.Count > 0 && OpBox.SelectedIndex < 0) OpBox.SelectedIndex = 0;

            LoadAll();
        }

        private void LoadAll()
        {
            HotelsGrid.ItemsSource = _svc.GetAll(SortStars.IsChecked == true);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            bool sort = SortStars.IsChecked == true;

            // 1) IME (case-insensitive + partial)
            var nameQ = (NameBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(nameQ))
            {
                var r = _svc.SearchByName(nameQ);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // 2) GODINA
            var yearQ = (YearBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(yearQ))
            {
                if (!int.TryParse(yearQ, out var year)) { MessageBox.Show("Unesi validnu godinu."); return; }
                var r = _svc.SearchByYear(year);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // 3) ZVEZDICE
            var starsQ = (StarsBox.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(starsQ))
            {
                if (!int.TryParse(starsQ, out var stars)) { MessageBox.Show("Unesi broj zvezdica."); return; }
                var r = _svc.SearchByStars(stars);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // 4) APARTMANI (sobe/gosti uz & ili |)
            int? rooms = int.TryParse((RoomsBox.Text ?? "").Trim(), out var rr) ? rr : (int?)null;
            int? guests = int.TryParse((GuestsBox.Text ?? "").Trim(), out var gg) ? gg : (int?)null;
            var op = ((OpBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "|") ? '|' : '&';

            if (rooms != null || guests != null)
            {
                var r = _svc.SearchByApartments(rooms, guests, op);
                HotelsGrid.ItemsSource = sort ? r.OrderByDescending(h => h.BrojZvezdica).ToList() : r;
                return;
            }

            // Ako ništa nije uneto → prikaži sve
            LoadAll();
        }

        private void NovaRezervacija_Click(object sender, RoutedEventArgs e)
        {
            if (_gost == null) { MessageBox.Show("Niste prijavljeni kao gost."); return; }

            if (HotelsGrid.SelectedItem is not Hotel h)
            {
                MessageBox.Show("Izaberite hotel iz liste.");
                return;
            }

            // >>> TVOJ prozor trenutno traži (Hotel, Korisnik)
            var rw = new RezervacijaWindow(h, _gost);
            rw.Owner = this;
            rw.ShowDialog();
        }

        private void OpenMyReservations_Click(object sender, RoutedEventArgs e)
        {
            if (_gost == null) { MessageBox.Show("Niste prijavljeni kao gost."); return; }

            // Obrati pažnju na TAČAN naziv klase ispod:
            var w = new GuestReservationsWindow(_gost);
            w.Owner = this;
            w.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
