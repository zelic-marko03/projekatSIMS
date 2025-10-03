using System;
using System.Linq;
using System.Windows;
using SIMSprojekat.Models;
using SIMSprojekat.Repositories;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class RezervacijaWindow : Window
    {
        private readonly Korisnik _gost;
        private readonly Hotel _hotel;
        private readonly RezervacijaService _svc = new RezervacijaService();

        // PROSLEDI i hotel i gosta
        public RezervacijaWindow(Hotel hotel, Korisnik gost)
        {
            InitializeComponent();
            _hotel = hotel;
            _gost = gost;

            Title = $"Nova rezervacija – {_hotel.Ime}";

            // napuni listu apartmana samo iz ovog hotela
            var aRepo = new ApartmanRepository();
            var apartmani = aRepo.GetAll()
                                 .Where(a => a.HotelSifra == _hotel.Sifra)
                                 .Select(a => a.Ime)
                                 .ToList();

            AptBox.ItemsSource = apartmani;
            if (apartmani.Count > 0) AptBox.SelectedIndex = 0;

            // podrazumevani datumi
            DatumOdPicker.SelectedDate = DateTime.Today;
            DatumDoPicker.SelectedDate = DateTime.Today; // jedan dan ili postavi raspon po želji
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Rezervisi_Click(object sender, RoutedEventArgs e)
        {
            if (AptBox.SelectedItem is not string apartmanIme)
            {
                MessageBox.Show("Izaberite apartman.");
                return;
            }

            if (DatumOdPicker.SelectedDate is not DateTime dOd ||
                DatumDoPicker.SelectedDate is not DateTime dDo)
            {
                MessageBox.Show("Odaberite datume.");
                return;
            }

            if (dDo < dOd)
            {
                MessageBox.Show("Krajnji datum ne može biti pre početnog.");
                return;
            }

            // POZIV TVOG SERVISA (zadrži tvoj potpis — ovde je 'Kreiraj')
            var rezultat = _svc.Kreiraj(_gost?.Email ?? "", apartmanIme, dOd, dDo);

            MessageBox.Show(rezultat.poruka);
            if (rezultat.ok)
            {
                DialogResult = true; // signaliziraj uspeh roditeljskom prozoru
                Close();
            }
        }
    }
}
