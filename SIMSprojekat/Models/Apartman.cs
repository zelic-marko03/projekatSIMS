using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSprojekat.Models
{
    public class Apartman
    {
        public string Ime { get; set; }          // jedinstveno u okviru hotela
        public string Opis { get; set; }
        public int BrojSoba { get; set; }
        public int MaxBrojGostiju { get; set; }
        public string HotelSifra { get; set; }   // povezuje apartman sa hotelom
    }
}
