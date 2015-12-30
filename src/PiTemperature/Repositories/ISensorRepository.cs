using PiTemperature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiTemperature.Repositories
{
    public interface ISensorRepository
    {
        void Delete(string id);
        Sensor Get(string id);
        List<Sensor> GetAll();
        void Create(Sensor sensor);
        void Update(Sensor sensor);
    }
}
