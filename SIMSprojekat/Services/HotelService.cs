using SIMSprojekat.Models;
using SIMSprojekat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Services
{
    public class HotelService
    {
        private readonly HotelRepository _hotels = new HotelRepository();
        private readonly ApartmanRepository _apts = new ApartmanRepository();

        public List<Hotel> GetAll(bool sortByStars = false)
        {
            var list = _hotels.GetAll();
            return sortByStars ? list.OrderByDescending(h => h.BrojZvezdica).ToList() : list;
        }

        public List<Hotel> SearchByName(string q) =>
            _hotels.GetAll().Where(h => (h.Ime ?? "").Contains(q ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Hotel> SearchByYear(int year) =>
            _hotels.GetAll().Where(h => h.GodinaIzgradnje == year).ToList();

        public List<Hotel> SearchByStars(int stars) =>
            _hotels.GetAll().Where(h => h.BrojZvezdica == stars).ToList();

        // Pretraga po apartmanima: sobe, gosti, kombinacija sa '&' ili '|'
        public List<Hotel> SearchByApartments(int? rooms, int? guests, char op = '&')
        {
            var hotels = _hotels.GetAll();
            var apts = _apts.GetAll();

            var roomsSet = rooms is null ? apts.Select(a => a.HotelSifra) : apts.Where(a => a.BrojSoba == rooms).Select(a => a.HotelSifra);
            var guestsSet = guests is null ? apts.Select(a => a.HotelSifra) : apts.Where(a => a.MaxBrojGostiju == guests).Select(a => a.HotelSifra);

            var ids = op == '|' ? roomsSet.Union(guestsSet) : roomsSet.Intersect(guestsSet);
            var idSet = ids.ToHashSet();
            return hotels.Where(h => idSet.Contains(h.Sifra)).ToList();
        }

        // Seed demo podataka ako je prazno (da imaš šta da vidiš)
        public void EnsureSeed()
        {
            var hotels = _hotels.GetAll();
            var apts = _apts.GetAll();
            if (hotels.Count > 0 && apts.Count > 0) return;

            hotels = new List<Hotel> {
                new() { Sifra="H1", Ime="Four Seasons", GodinaIzgradnje=1998, BrojZvezdica=5, VlasnikJMBG="111" },
                new() { Sifra="H2", Ime="Sea Breeze",   GodinaIzgradnje=2005, BrojZvezdica=4, VlasnikJMBG="222" },
                new() { Sifra="H3", Ime="Mountain Inn", GodinaIzgradnje=2015, BrojZvezdica=3, VlasnikJMBG="333" }
            };
            apts = new List<Apartman> {
                new() { Ime="FS-Deluxe", Opis="Suite", BrojSoba=2, MaxBrojGostiju=3, HotelSifra="H1" },
                new() { Ime="FS-Family", Opis="Family",BrojSoba=3, MaxBrojGostiju=5, HotelSifra="H1" },
                new() { Ime="SB-Std",    Opis="Std",   BrojSoba=1, MaxBrojGostiju=2, HotelSifra="H2" },
                new() { Ime="MI-Loft",   Opis="Loft",  BrojSoba=2, MaxBrojGostiju=4, HotelSifra="H3" },
            };
            _hotels.SaveAll(hotels);
            _apts.SaveAll(apts);
        }
    }
}
