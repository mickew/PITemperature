using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiTemperature.Models;

namespace PiTemperature.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ApplicationDbContext _context;

        public SensorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Sensor sensor)
        {
            _context.Sensors.Add(sensor);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var entity = _context.Sensors.First(t => t.Id == id);
        }

        public Sensor Get(string id)
        {
            return _context.Sensors.FirstOrDefault(t => t.Id == id);
        }

        public List<Sensor> GetAll()
        {
            return _context.Sensors.ToList();
        }

        public void Update(Sensor sensor)
        {
            _context.Sensors.Update(sensor);
            _context.SaveChanges();
        }
    }
}
