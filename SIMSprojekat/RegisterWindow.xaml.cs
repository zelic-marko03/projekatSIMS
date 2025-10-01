using System.Text.RegularExpressions;
using System.Windows;
using SIMSprojekat.Models;
using SIMSprojekat.Services;

namespace SIMSprojekat
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _auth = new AuthService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ImeBox.Text) ||
                string.IsNullOrWhiteSpace(PrezimeBox.Text) ||
                string.IsNullOrWhiteSpace(JmbgBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(TelefonBox.Text) ||
                string.IsNullOrWhiteSpace(PassBox.Password) ||
                string.IsNullOrWhiteSpace(PassConfirmBox.Password))
            {
                MessageBox.Show("Popunite sva polja.");
                return;
            }

            if (PassBox.Password != PassConfirmBox.Password)
            {
                MessageBox.Show("Lozinke se ne poklapaju.");
                return;
            }

            var korisnik = new Korisnik
            {
                Ime = ImeBox.Text.Trim(),
                Prezime = PrezimeBox.Text.Trim(),
                JMBG = JmbgBox.Text.Trim(),
                Email = EmailBox.Text.Trim().ToLowerInvariant(),
                Telefon = TelefonBox.Text.Trim(),
                Lozinka = PassBox.Password.Trim()
            };

            var (ok, poruka) = _auth.RegisterGuest(korisnik);
            MessageBox.Show(poruka);
            if (ok) Close();
        }
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) { Register_Click(this, new RoutedEventArgs()); e.Handled = true; }
            if (e.Key == System.Windows.Input.Key.Escape) { Close(); e.Handled = true; }
            base.OnPreviewKeyDown(e);
        }

        // 2) Fokus na prvo polje kada se otvori
        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            ImeBox.Focus();
        }

    }
}
