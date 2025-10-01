using System;
using System.Linq;
using System.Windows;
using SIMSprojekat.Models;
using SIMSprojekat.Repositories;

namespace SIMSprojekat
{
    public partial class NoviApartmanWindow : Window
    {
        public NoviApartmanWindow(string defaultHotelSifra = "")
        {
            InitializeComponent();
            HotelSifraBox.Text = defaultHotelSifra;
        }

        public bool Uspeh { get; private set; }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HotelSifraBox.Text) ||
                string.IsNullOrWhiteSpace(ImeBox.Text) ||
                !int.TryParse(SobeBox.Text, out var sobe) ||
                !int.TryParse(GostiBox.Text, out var gosti))
            {
                MessageBox.Show("Popunite sva polja (sobe/gosti kao brojevi).");
                return;
            }

            var hotelRepo = new HotelRepository();
            var aptRepo = new ApartmanRepository();

            var hotel = hotelRepo.GetAll().FirstOrDefault(h => h.Sifra == HotelSifraBox.Text.Trim());
            if (hotel == null)
            {
                MessageBox.Show("Hotel sa tom šifrom ne postoji.");
                return;
            }

            var svi = aptRepo.GetAll();
            if (svi.Any(a => a.Ime.Equals(ImeBox.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Apartman sa tim imenom već postoji.");
                return;
            }

            svi.Add(new Apartman
            {
                HotelSifra = hotel.Sifra,
                Ime = ImeBox.Text.Trim(),
                BrojSoba = sobe,
                MaxBrojGostiju = gosti
            });
            aptRepo.SaveAll(svi);

            Uspeh = true;
            Close();
        }
    }
}
