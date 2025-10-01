using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Models
{
    public enum StatusHotela { NaCekanju, Prihvacen, Odbijen }

    public class Hotel
    {
        public string Sifra { get; set; }
        public string Ime { get; set; }
        public int GodinaIzgradnje { get; set; }
        public int BrojZvezdica { get; set; }

        public string VlasnikJMBG { get; set; }   // već bi trebalo da postoji
        public StatusHotela Status { get; set; } = StatusHotela.NaCekanju;
        public string RazlogOdbijanja { get; set; }
    }
}
