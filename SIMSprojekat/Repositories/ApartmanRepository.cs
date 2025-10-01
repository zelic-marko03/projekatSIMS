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
    public class ApartmanRepository
    {
        private readonly string _filePath = "apartmani.json";

        public List<Apartman> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<Apartman>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Apartman>>(json) ?? new List<Apartman>();
        }

        public void SaveAll(List<Apartman> apartmani)
        {
            string json = JsonSerializer.Serialize(apartmani, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
