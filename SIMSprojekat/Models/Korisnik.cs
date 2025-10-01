using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Models
{
    public enum TipKorisnika { Administrator, Gost, Vlasnik }
    public class Korisnik
    {
        public string JMBG { get; set; }   // jedinstveno
        public string Email { get; set; }  // jedinstveno
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Telefon { get; set; }
        public TipKorisnika Tip { get; set; }
    }
}
