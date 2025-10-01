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
    public class HotelRepository
    {
        private readonly string _filePath = "hoteli.json";

        public List<Hotel> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<Hotel>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Hotel>>(json) ?? new List<Hotel>();
        }

        public void SaveAll(List<Hotel> hoteli)
        {
            string json = JsonSerializer.Serialize(hoteli, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
