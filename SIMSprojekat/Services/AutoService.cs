using SIMSprojekat.Models;
using SIMSprojekat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Services
{
    public class AuthService
    {
        private readonly KorisnikRepository _korRepo = new KorisnikRepository();

        public Korisnik? Login(string email, string lozinka)
        {
            return _korRepo.GetAll().FirstOrDefault(k =>
                k.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase) &&
                k.Lozinka == lozinka);
        }

        public (bool ok, string poruka) RegisterGuest(Korisnik novi)
        {
            var all = _korRepo.GetAll();

            // normalizacija
            novi.Email = (novi.Email ?? "").Trim().ToLowerInvariant();
            novi.JMBG = (novi.JMBG ?? "").Trim();
            novi.Lozinka = (novi.Lozinka ?? "").Trim();

            // provere jedinstvenosti prema specifikaciji
            if (all.Any(k => k.Email.Equals(novi.Email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Email već postoji.");

            if (all.Any(k => k.Lozinka == novi.Lozinka))
                return (false, "Lozinka već postoji.");

            // (opciono, ali korisno) jedinstven JMBG
            if (all.Any(k => k.JMBG == novi.JMBG))
                return (false, "JMBG već postoji.");

            novi.Tip = TipKorisnika.Gost;
            all.Add(novi);
            _korRepo.SaveAll(all);
            return (true, "Uspešna registracija.");
        }

        public void EnsureDemoUsers()
        {
            var all = _korRepo.GetAll();

            // Admin
            if (!all.Any(k => k.Tip == TipKorisnika.Administrator))
            {
                all.Add(new Korisnik
                {
                    JMBG = "0000000000000",
                    Ime = "Admin",
                    Prezime = "Admin",
                    Email = "admin@sims.com",
                    Lozinka = "admin",
                    Telefon = "000",
                    Tip = TipKorisnika.Administrator
                });
            }

            // Gost
            if (!all.Any(k => k.Email.Equals("gost@test.com", StringComparison.OrdinalIgnoreCase)))
            {
                all.Add(new Korisnik
                {
                    JMBG = "1111111111111",
                    Ime = "Gost",
                    Prezime = "Test",
                    Email = "gost@test.com",
                    Lozinka = "gost",
                    Telefon = "111",
                    Tip = TipKorisnika.Gost
                });
            }

            _korRepo.SaveAll(all);
        }
        

    }
}
