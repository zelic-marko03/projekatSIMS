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
    public class KorisnikRepository
    {
        private readonly string _filePath = "korisnici.json";

        public List<Korisnik> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<Korisnik>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Korisnik>>(json) ?? new List<Korisnik>();
        }

        public void SaveAll(List<Korisnik> korisnici)
        {
            string json = JsonSerializer.Serialize(korisnici, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
