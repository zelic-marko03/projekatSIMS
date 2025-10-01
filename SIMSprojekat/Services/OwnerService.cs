using System;
using System.Collections.Generic;
using System.Linq;
using SIMSprojekat.Models;
using SIMSprojekat.Repositories;

namespace SIMSprojekat.Services
{
    public class OwnerService
    {
        private readonly RezervacijaRepository _rezRepo = new RezervacijaRepository();
        private readonly ApartmanRepository _aptRepo = new ApartmanRepository();
        private readonly HotelRepository _hotelRepo = new HotelRepository();

        // Sve rezervacije koje se odnose na hotele ovog vlasnika
        public List<Rezervacija> RezervacijeVlasnika(string vlasnikJMBG, StatusRezervacije? filter = null)
        {
            var mojiHoteli = _hotelRepo.GetAll()
                                       .Where(h => h.VlasnikJMBG == vlasnikJMBG)
                                       .Select(h => h.Sifra)
                                       .ToHashSet();

            var mojiApt = _aptRepo.GetAll()
                                  .Where(a => mojiHoteli.Contains(a.HotelSifra))
                                  .Select(a => a.Ime)
                                  .ToHashSet();

            var sve = _rezRepo.GetAll().Where(r => mojiApt.Contains(r.ApartmanIme)).ToList();
            if (filter != null) sve = sve.Where(r => r.Status == filter).ToList();
            return sve;
        }

        public (bool ok, string poruka) PotvrdiRezervaciju(Guid id)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");
            rez.Status = StatusRezervacije.Potvrdjena;
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija potvrđena.");
        }

        public (bool ok, string poruka) OdbijRezervaciju(Guid id, string razlog)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");
            rez.Status = StatusRezervacije.Odbijena;
            rez.RazlogOdbijanja = razlog;
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija odbijena.");
        }

        // Hoteli jednog vlasnika (opciono filter po statusu)
        public List<Hotel> HoteliVlasnika(string vlasnikJMBG, StatusHotela? filter = null)
        {
            var hoteli = _hotelRepo.GetAll().Where(h => h.VlasnikJMBG == vlasnikJMBG).ToList();
            if (filter != null) hoteli = hoteli.Where(h => h.Status == filter).ToList();
            return hoteli;
        }

        public (bool ok, string poruka) PrihvatiHotel(string sifra)
        {
            var svi = _hotelRepo.GetAll();
            var h = svi.FirstOrDefault(x => x.Sifra == sifra);
            if (h == null) return (false, "Hotel ne postoji.");
            h.Status = StatusHotela.Prihvacen;
            h.RazlogOdbijanja = null;
            _hotelRepo.SaveAll(svi);
            return (true, "Hotel prihvaćen.");
        }

        public (bool ok, string poruka) OdbijHotel(string sifra, string razlog)
        {
            var svi = _hotelRepo.GetAll();
            var h = svi.FirstOrDefault(x => x.Sifra == sifra);
            if (h == null) return (false, "Hotel ne postoji.");
            h.Status = StatusHotela.Odbijen;
            h.RazlogOdbijanja = razlog;
            _hotelRepo.SaveAll(svi);
            return (true, "Hotel odbijen.");
        }

    }

}
