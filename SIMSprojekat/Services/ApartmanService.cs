// Services/ApartmanService.cs
using System.Linq;
using System.Collections.Generic;
using SIMSprojekat.Models;
using SIMSprojekat.Repositories;

namespace SIMSprojekat.Services
{
    public class ApartmanService
    {
        private readonly ApartmanRepository _repo = new ApartmanRepository();

        /// <summary>
        /// Vraća sve apartmane za dati hotel.
        /// </summary>
        public List<Apartman> GetByHotel(string hotelSifra)
            => _repo.GetAll().Where(a => a.HotelSifra == hotelSifra).ToList();

        /// <summary>
        /// Dodaje apartman u hotel. Proverava duplikate imena u okviru istog hotela
        /// i validira osnovna polja. Vraća (ok, poruka).
        /// </summary>
        public (bool ok, string poruka) Add(Apartman a)
        {
            if (a == null) return (false, "Nedefinisan apartman.");
            a.Ime = (a.Ime ?? "").Trim();
            a.Opis = (a.Opis ?? "").Trim();
            a.HotelSifra = (a.HotelSifra ?? "").Trim();

            if (string.IsNullOrWhiteSpace(a.Ime) || string.IsNullOrWhiteSpace(a.HotelSifra))
                return (false, "Ime i hotel su obavezni.");

            if (a.BrojSoba <= 0 || a.MaxBrojGostiju <= 0)
                return (false, "Broj soba i broj gostiju moraju biti pozitivni brojevi.");

            var svi = _repo.GetAll();

            // Duplikat imena u okviru istog hotela?
            bool postoji = svi.Any(x =>
                x.HotelSifra == a.HotelSifra &&
                x.Ime.Equals(a.Ime, System.StringComparison.OrdinalIgnoreCase));

            if (postoji)
                return (false, "Apartman sa tim imenom već postoji u izabranom hotelu.");

            svi.Add(a);
            _repo.SaveAll(svi);
            return (true, "Apartman dodat.");
        }
    }
}
