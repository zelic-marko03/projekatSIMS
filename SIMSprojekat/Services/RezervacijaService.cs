using SIMSprojekat.Models;
using SIMSprojekat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIMSprojekat.Services
{
    public class RezervacijaService
    {
        private readonly RezervacijaRepository _rezRepo = new RezervacijaRepository();
        private readonly ApartmanRepository _aptRepo = new ApartmanRepository();

        public (bool ok, string poruka) Kreiraj(string gostEmail, string apartmanIme, DateTime od, DateTime doDatuma)
        {
            if (doDatuma < od)
                return (false, "Datum završetka mora biti posle početnog datuma.");

            var apartman = _aptRepo.GetAll().FirstOrDefault(a => a.Ime == apartmanIme);
            if (apartman == null)
                return (false, "Apartman ne postoji.");

            if (!DaLiJeSlobodan(apartmanIme, od, doDatuma))
                return (false, "Apartman je zauzet za traženi period.");

            var sve = _rezRepo.GetAll();
            sve.Add(new Rezervacija
            {
                Id = Guid.NewGuid(),
                GostEmail = gostEmail,
                ApartmanIme = apartmanIme,
                DatumOd = od,
                DatumDo = doDatuma,
                Status = StatusRezervacije.NaCekanju
            });
            _rezRepo.SaveAll(sve);

            return (true, "Rezervacija poslata vlasniku na potvrdu.");
        }

        public bool DaLiJeSlobodan(string apartmanIme, DateTime od, DateTime doDatuma)
        {
            var sve = _rezRepo.GetAll();
            return !sve.Any(r =>
                r.ApartmanIme == apartmanIme &&
                r.Status != StatusRezervacije.Odbijena &&
                !(doDatuma < r.DatumOd || od > r.DatumDo));
        }

        public List<Rezervacija> MojeRezervacije(string email, StatusRezervacije? filter = null)
        {
            var sve = _rezRepo.GetAll().Where(r => r.GostEmail == email).ToList();
            if (filter != null)
                sve = sve.Where(r => r.Status == filter).ToList();
            return sve;
        }

        public (bool ok, string poruka) Otkazi(Guid id)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");

            if (rez.Status == StatusRezervacije.Odbijena)
                return (false, "Rezervacija je već odbijena i ne može se otkazati.");

            sve.Remove(rez);
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija otkazana.");
        }

        public (bool ok, string poruka) Potvrdi(Guid id)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");

            rez.Status = StatusRezervacije.Potvrdjena;
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija potvrđena.");
        }

        public (bool ok, string poruka) Odbij(Guid id, string razlog)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");

            rez.Status = StatusRezervacije.Odbijena;
            rez.RazlogOdbijanja = razlog;
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija odbijena.");
        }
        // Alias zbog starijih poziva:
        public System.Collections.Generic.List<Rezervacija> GetByGuest(string email, StatusRezervacije? filter = null)
            => MojeRezervacije(email, filter);

        // Alias sa dijakritikom – prosleđuje na Otkazi
        public (bool ok, string poruka) Otkaži(System.Guid id) => Otkazi(id);
    }
}
