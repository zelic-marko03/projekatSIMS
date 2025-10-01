using SIMSprojekat.Models;
using SIMSprojekat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Services
{
    public class RezervacijaService
    {
        private readonly RezervacijaRepository _rezRepo = new RezervacijaRepository();
        private readonly ApartmanRepository _aptRepo = new ApartmanRepository();

        /// <summary>
        /// Gost pravi rezervaciju (jedan dan ili više dana).
        /// </summary>
        public (bool ok, string poruka) Kreiraj(string gostEmail, string apartmanIme, DateTime od, DateTime doDatuma)
        {
            if (doDatuma < od)
                return (false, "Datum završetka mora biti posle početnog datuma.");

            var apartmani = _aptRepo.GetAll();
            var apartman = apartmani.FirstOrDefault(a => a.Ime == apartmanIme);
            if (apartman == null)
                return (false, "Apartman ne postoji.");

            if (!DaLiJeSlobodan(apartmanIme, od, doDatuma))
                return (false, "Apartman je zauzet za traženi period.");

            var sve = _rezRepo.GetAll();
            sve.Add(new Rezervacija
            {
                GostEmail = gostEmail,
                ApartmanIme = apartmanIme,
                DatumOd = od,
                DatumDo = doDatuma,
                Status = StatusRezervacije.NaCekanju
            });
            _rezRepo.SaveAll(sve);

            return (true, "Rezervacija poslata vlasniku na potvrdu.");
        }

        /// <summary>
        /// Provera da li je apartman slobodan u zadatom periodu.
        /// </summary>
        public bool DaLiJeSlobodan(string apartmanIme, DateTime od, DateTime doDatuma)
        {
            var sve = _rezRepo.GetAll();
            return !sve.Any(r =>
                r.ApartmanIme == apartmanIme &&
                r.Status != StatusRezervacije.Odbijena && // odbijene ne blokiraju
                !(doDatuma < r.DatumOd || od > r.DatumDo) // provera preklapanja
            );
        }

        /// <summary>
        /// Vrati sve rezervacije za gosta.
        /// </summary>
        public List<Rezervacija> MojeRezervacije(string email, StatusRezervacije? filter = null)
        {
            var sve = _rezRepo.GetAll().Where(r => r.GostEmail == email).ToList();
            if (filter != null)
                sve = sve.Where(r => r.Status == filter).ToList();
            return sve;
        }

        /// <summary>
        /// Gost otkazuje rezervaciju (ako je NaČekanju ili Potvrđena).
        /// </summary>
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

        /// <summary>
        /// Vlasnik potvrđuje rezervaciju.
        /// </summary>
        public (bool ok, string poruka) Potvrdi(Guid id)
        {
            var sve = _rezRepo.GetAll();
            var rez = sve.FirstOrDefault(r => r.Id == id);
            if (rez == null) return (false, "Rezervacija ne postoji.");

            rez.Status = StatusRezervacije.Potvrdjena;
            _rezRepo.SaveAll(sve);
            return (true, "Rezervacija potvrđena.");
        }

        /// <summary>
        /// Vlasnik odbija rezervaciju sa razlogom.
        /// </summary>
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
        public (bool ok, string poruka) KreirajRezervaciju(string apartmanIme, string gostEmail, DateTime datumOd, DateTime datumDo)
        {
            var sve = _rezRepo.GetAll();

            // 1) Provera zauzetosti
            var zauzete = sve.Where(r => r.ApartmanIme == apartmanIme && r.Status == StatusRezervacije.Potvrdjena);

            foreach (var rez in zauzete)
            {
                // Ako se preklapaju intervali → apartman je zauzet
                if (datumOd <= rez.DatumDo && datumDo >= rez.DatumOd)
                {
                    return (false, "Apartman je zauzet u tom periodu!");
                }
            }

            // 2) Ako je slobodno → napravi novu rezervaciju
            var nova = new Rezervacija
            {
                Id = Guid.NewGuid(),
                ApartmanIme = apartmanIme,
                GostEmail = gostEmail,
                DatumOd = datumOd,
                DatumDo = datumDo,
                Status = StatusRezervacije.NaCekanju
            };

            sve.Add(nova);
            _rezRepo.SaveAll(sve);

            return (true, "Rezervacija poslata vlasniku na potvrdu.");
        }
    }
}
