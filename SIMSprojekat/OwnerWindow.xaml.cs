using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SIMSprojekat.Models;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class OwnerWindow : Window
    {
        private readonly Korisnik _vlasnik;
        private readonly OwnerService _svc = new OwnerService();

        public OwnerWindow(Korisnik vlasnik)
        {
            InitializeComponent();
            _vlasnik = vlasnik;
            Title = $"Vlasnik – {_vlasnik.Ime} {_vlasnik.Prezime}";
            InitHoteliDropDown();
            LoadRezervacije(null);     // prvi tab
            LoadHoteli(null);          // drugi tab
        }

        // === REZERVACIJE (TAB 1) ===
        private void InitHoteliDropDown()
        {
            var hoteli = _svc.HoteliVlasnika(_vlasnik.JMBG, null)
                             .OrderBy(h => h.Ime)
                             .Select(h => new { h.Sifra, Naziv = $"{h.Sifra} - {h.Ime}" })
                             .ToList();
            HotelBox.ItemsSource = hoteli;
            if (hoteli.Any()) HotelBox.SelectedIndex = 0;
        }

        private void LoadRezervacije(StatusRezervacije? filter)
        {
            var izabrani = HotelBox.SelectedItem;
            string sifraHotela = null;
            if (izabrani != null)
            {
                // hotel filter: prikazujemo rezervacije SAMO za apartmane tog hotela
                sifraHotela = (string)izabrani.GetType().GetProperty("Sifra")!.GetValue(izabrani, null);
            }

            // dohvatimo sve pa filtriramo po hotelu
            var sve = _svc.RezervacijeVlasnika(_vlasnik.JMBG, filter);
            if (!string.IsNullOrEmpty(sifraHotela))
                sve = sve.Where(r =>
                {
                    // Apartman -> HotelSifra mapping
                    var aRepo = new Repositories.ApartmanRepository();
                    var a = aRepo.GetAll().FirstOrDefault(x => x.Ime == r.ApartmanIme);
                    return a != null && a.HotelSifra == sifraHotela;
                }).ToList();

            RezGrid.ItemsSource = sve;
        }

        private void HotelBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => LoadRezervacije(GetRezFilter());

        private StatusRezervacije? GetRezFilter()
        {
            var item = RezStatusBox.SelectedItem as ComboBoxItem;
            var tag = item?.Tag?.ToString();
            return tag switch
            {
                "NaCekanju" => StatusRezervacije.NaCekanju,
                "Potvrdjena" => StatusRezervacije.Potvrdjena,
                "Odbijena" => StatusRezervacije.Odbijena,
                _ => null
            };
        }

        private void RezStatusBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => LoadRezervacije(GetRezFilter());

        private void Potvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (RezGrid.SelectedItem is not Rezervacija r) { MessageBox.Show("Izaberite rezervaciju."); return; }
            var (ok, poruka) = _svc.PotvrdiRezervaciju(r.Id);
            MessageBox.Show(poruka);
            LoadRezervacije(GetRezFilter());
        }

        private void Odbij_Click(object sender, RoutedEventArgs e)
        {
            if (RezGrid.SelectedItem is not Rezervacija r) { MessageBox.Show("Izaberite rezervaciju."); return; }
            var razlog = Microsoft.VisualBasic.Interaction.InputBox("Unesite razlog odbijanja:", "Odbijanje rezervacije", "");
            if (string.IsNullOrWhiteSpace(razlog)) return;
            var (ok, poruka) = _svc.OdbijRezervaciju(r.Id, razlog.Trim());
            MessageBox.Show(poruka);
            LoadRezervacije(GetRezFilter());
        }

        // === HOTELI (TAB 2) ===
        private void LoadHoteli(StatusHotela? f)
        {
            HoteliGrid.ItemsSource = _svc.HoteliVlasnika(_vlasnik.JMBG, f);
        }

        private StatusHotela? GetHotelFilter()
        {
            var item = HotelStatusBox.SelectedItem as ComboBoxItem;
            var tag = item?.Tag?.ToString();
            return tag switch
            {
                "NaCekanju" => StatusHotela.NaCekanju,
                "Prihvacen" => StatusHotela.Prihvacen,
                "Odbijen" => StatusHotela.Odbijen,
                _ => null
            };
        }

        private void HotelStatusBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => LoadHoteli(GetHotelFilter());

        private void PrihvatiHotel_Click(object sender, RoutedEventArgs e)
        {
            if (HoteliGrid.SelectedItem is not Hotel h) { MessageBox.Show("Izaberite hotel."); return; }
            var (ok, poruka) = _svc.PrihvatiHotel(h.Sifra);
            MessageBox.Show(poruka);
            LoadHoteli(GetHotelFilter());
            InitHoteliDropDown(); // da upadne u izbor ako je bio na čekanju
        }

        private void OdbijHotel_Click(object sender, RoutedEventArgs e)
        {
            if (HoteliGrid.SelectedItem is not Hotel h) { MessageBox.Show("Izaberite hotel."); return; }
            var razlog = Microsoft.VisualBasic.Interaction.InputBox("Razlog odbijanja hotela:", "Odbijanje hotela", "");
            if (string.IsNullOrWhiteSpace(razlog)) return;
            var (ok, poruka) = _svc.OdbijHotel(h.Sifra, razlog.Trim());
            MessageBox.Show(poruka);
            LoadHoteli(GetHotelFilter());
        }
    }
}
