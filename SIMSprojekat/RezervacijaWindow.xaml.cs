using SIMSprojekat.Models;
using SIMSprojekat.Services;
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
    /// Interaction logic for RezervacijaWindow.xaml
    /// </summary>
    public partial class RezervacijaWindow : Window
    {
        private readonly Korisnik _gost;
        private readonly RezervacijaService _svc = new RezervacijaService();

        public RezervacijaWindow(Korisnik gost)
        {
            InitializeComponent();
            _gost = gost;
        }

        private void Rezervisi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApartmanBox.Text))
            {
                MessageBox.Show("Unesite ime apartmana.");
                return;
            }

            if (DatumOdPicker.SelectedDate == null || DatumDoPicker.SelectedDate == null)
            {
                MessageBox.Show("Odaberite datume.");
                return;
            }

            var od = DatumOdPicker.SelectedDate.Value;
            var doDatuma = DatumDoPicker.SelectedDate.Value;

            var rezultat = _svc.Kreiraj(_gost.Email, ApartmanBox.Text, od, doDatuma);

            MessageBox.Show(rezultat.poruka);

            if (rezultat.ok)
                Close();
        }
    }
}
