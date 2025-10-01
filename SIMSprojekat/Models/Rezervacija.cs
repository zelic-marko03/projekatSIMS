using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Models
{
    public enum StatusRezervacije { NaCekanju, Potvrdjena, Odbijena }

    public class Rezervacija
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // jedinstven ID
        public string GostEmail { get; set; }           // email gosta
        public string ApartmanIme { get; set; }         // ime apartmana
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public StatusRezervacije Status { get; set; }
        public string? RazlogOdbijanja { get; set; }    // ako vlasnik odbije
    }
}
