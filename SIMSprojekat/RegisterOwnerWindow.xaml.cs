using System.Text.RegularExpressions;
using System.Windows;
using SIMSprojekat.Models;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class RegisterOwnerWindow : Window
    {
        private readonly AdminService _svc = new AdminService();

        public RegisterOwnerWindow()
        {
            InitializeComponent();
        }

        // mora tačno ovakav potpis
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        // mora tačno ovakav potpis
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ImeBox.Text) ||
                string.IsNullOrWhiteSpace(PrezimeBox.Text) ||
                string.IsNullOrWhiteSpace(JmbgBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(TelefonBox.Text) ||
                string.IsNullOrWhiteSpace(PassBox.Password))
            {
                MessageBox.Show("Popunite sva polja.");
                return;
            }

            if (!Regex.IsMatch(JmbgBox.Text.Trim(), @"^\d{13}$"))
            {
                MessageBox.Show("JMBG mora imati 13 cifara.");
                return;
            }

            var k = new Korisnik
            {
                Ime = ImeBox.Text.Trim(),
                Prezime = PrezimeBox.Text.Trim(),
                JMBG = JmbgBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Telefon = TelefonBox.Text.Trim(),
                Lozinka = PassBox.Password.Trim()
            };

            var (ok, poruka) = _svc.RegisterOwner(k);
            MessageBox.Show(poruka);
            if (ok) Close();
        }
    }
}