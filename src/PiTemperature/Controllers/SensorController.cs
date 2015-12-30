using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiTemperature.Meters;
using PiTemperature.Repositories;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiTemperature.Controllers
{
    [Route("api/[controller]")]
    public class SensorController : Controller
    {
        private readonly Temperature temperature;
        private ISensorRepository _sensorRepository;

        public SensorController(Temperature temperature, ISensorRepository sensorRepository)
        {
            this.temperature = temperature;
            this._sensorRepository = sensorRepository;
        }

        // GET: api/sensor
        [HttpGet]
        public IEnumerable<TempSensorBase> Get()
        {
            return temperature.TempSensors.Select(c => new TempSensorBase(c.Sensor) { Name = c.Name });
        }

        [HttpGet("{id}", Name = "GetSensor")]
        public IActionResult GetById(string id)
        {
            var item = temperature.TempSensors.Where(c => c.Sensor == id).Select(c => new TempSensorBase(c.Sensor) { Name = c.Name }).FirstOrDefault();
            if (item == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(item);
        }
        
        // PUT api/sensor/5
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]TempSensorBase sensor)
        {
            if (sensor == null || sensor.Sensor != id)
            {
                return HttpBadRequest();
            }

            var tempSensor = temperature.TempSensors.Where(c => c.Sensor == id).FirstOrDefault();
            if (tempSensor == null)
            {
                return HttpNotFound();
            }

            var tmpSensor = _sensorRepository.Get(tempSensor.Sensor);
            if (tmpSensor != null)
            {
                tmpSensor.Name = sensor.Name;
                _sensorRepository.Update(tmpSensor);
            }
            else
            {
                _sensorRepository.Create(new Models.Sensor() { Id = sensor.Sensor, Name = sensor.Name });
            }
            tempSensor.Name = sensor.Name;
            temperature.RefreshClients();
            return new NoContentResult();
        }
    }
}
