using System.Linq;
using SIMSprojekat.Models;
using SIMSprojekat.Repositories;

namespace SIMSprojekat.Services
{
    public class AdminService
    {
        private readonly KorisnikRepository _korRepo = new KorisnikRepository();
        private readonly HotelRepository _hotelRepo = new HotelRepository();

        // 1) Registracija VLASNIKA
        public (bool ok, string poruka) RegisterOwner(Korisnik novi)
        {
            var all = _korRepo.GetAll();

            novi.Email = (novi.Email ?? "").Trim().ToLowerInvariant();
            novi.JMBG = (novi.JMBG ?? "").Trim();
            novi.Lozinka = (novi.Lozinka ?? "").Trim();

            if (all.Any(k => k.Email.Equals(novi.Email, System.StringComparison.OrdinalIgnoreCase)))
                return (false, "Email već postoji.");
            if (all.Any(k => k.JMBG == novi.JMBG))
                return (false, "JMBG već postoji.");

            novi.Tip = TipKorisnika.Vlasnik;
            all.Add(novi);
            _korRepo.SaveAll(all);
            return (true, "Vlasnik uspešno registrovan.");
        }

        // 2) Unos HOTELA (na čekanju, povezan sa vlasnikom po JMBG)
        public (bool ok, string poruka) AddHotel(Hotel novi)
        {
            var vlasnik = _korRepo.GetAll()
                .FirstOrDefault(k => k.JMBG == novi.VlasnikJMBG && k.Tip == TipKorisnika.Vlasnik);
            if (vlasnik == null)
                return (false, "Ne postoji vlasnik sa tim JMBG.");

            var hoteli = _hotelRepo.GetAll();
            if (hoteli.Any(h => h.Sifra == novi.Sifra))
                return (false, "Hotel sa ovom šifrom već postoji.");

            novi.Status = StatusHotela.NaCekanju;  // ključ po specifikaciji
            hoteli.Add(novi);
            _hotelRepo.SaveAll(hoteli);
            return (true, "Hotel dodat (čeka odobrenje vlasnika).");
        }
    }
}
