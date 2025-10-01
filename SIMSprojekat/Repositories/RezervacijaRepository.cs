using SIMSprojekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SIMSprojekat.Repositories
{
    public class RezervacijaRepository
    {
        private readonly string _filePath = "rezervacije.json";

        public List<Rezervacija> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<Rezervacija>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Rezervacija>>(json) ?? new List<Rezervacija>();
        }

        public void SaveAll(List<Rezervacija> rezervacije)
        {
            string json = JsonSerializer.Serialize(rezervacije, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
