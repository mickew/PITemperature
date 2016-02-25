using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiTemperature.Meters;
using PiTemperature.Repositories;
using Microsoft.AspNet.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiTemperature.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SensorController : Controller
    {
        private readonly ILogger<SensorController> _logger;
        private readonly Temperature temperature;
        private ISensorRepository _sensorRepository;

        public SensorController(Temperature temperature, ISensorRepository sensorRepository, ILogger<SensorController> logger)
        {
            _logger = logger;
            this.temperature = temperature;
            this._sensorRepository = sensorRepository;
        }

        // GET: api/sensor
        [Authorize(Roles = "admins")]
        [HttpGet]
        public IEnumerable<TempSensorBase> Get()
        {
            return temperature.TempSensors.Select(c => new TempSensorBase(c.Sensor) { Name = c.Name, MinValue = c.MinValue, MaxValue = c.MaxValue, TicksInterval = c.TicksInterval, ScaleColor = c.ScaleColor });
        }

        [Authorize(Roles = "admins")]
        [HttpGet("{id}", Name = "GetSensor")]
        public IActionResult GetById(string id)
        {
            var item = temperature.TempSensors.Where(c => c.Sensor == id).Select(c => new TempSensorBase(c.Sensor)
            {
                Name = c.Name,
                MinValue = c.MinValue,
                MaxValue = c.MaxValue,
                TicksInterval = c.TicksInterval,
                ScaleColor = c.ScaleColor
            }).FirstOrDefault();
            if (item == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(item);
        }

        // PUT api/sensor/5
        [Authorize(Roles = "admins")]
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
                tmpSensor.MinValue = sensor.MinValue;
                tmpSensor.MaxValue = sensor.MaxValue;
                tmpSensor.TicksInterval = sensor.TicksInterval;
                tmpSensor.FirstColor = sensor.ScaleColor.FirstColor;
                tmpSensor.FirstDivider = sensor.ScaleColor.FirstDivider;
                tmpSensor.SecondColor= sensor.ScaleColor.SecondColor;
                tmpSensor.SecondDivider = sensor.ScaleColor.SecondDivider;
                tmpSensor.ThirdColor = sensor.ScaleColor.ThirdColor;
                tmpSensor.ThirdDivider = sensor.ScaleColor.ThirdDivider;
                tmpSensor.LastColor = sensor.ScaleColor.LastColor;
                _sensorRepository.Update(tmpSensor);
            }
            else
            {
                _sensorRepository.Create(new Models.Sensor()
                {
                    Id = sensor.Sensor,
                    Name = sensor.Name,
                    MinValue = sensor.MinValue,
                    MaxValue = sensor.MaxValue,
                    TicksInterval = sensor.TicksInterval,
                    FirstColor = sensor.ScaleColor.FirstColor,
                    FirstDivider = sensor.ScaleColor.FirstDivider,
                    SecondColor = sensor.ScaleColor.SecondColor,
                    SecondDivider = sensor.ScaleColor.SecondDivider,
                    ThirdColor = sensor.ScaleColor.ThirdColor,
                    ThirdDivider = sensor.ScaleColor.ThirdDivider,
                    LastColor = sensor.ScaleColor.LastColor
                });
            }
            tempSensor.Name = sensor.Name;
            tempSensor.MinValue = sensor.MinValue;
            tempSensor.MaxValue = sensor.MaxValue;
            tempSensor.TicksInterval = sensor.TicksInterval;
            tempSensor.ScaleColor = sensor.ScaleColor;
            temperature.RefreshClients();
            return new NoContentResult();
        }

        // PUT api/sensor/5
        [Authorize(Roles = "admins")]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id, [FromBody]TempSensorBase sensor)
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
                _sensorRepository.Delete(tmpSensor.Id);
            }
            var i = temperature.TempSensors.IndexOf(tempSensor) + 1;
            sensor.Name = string.Format("TempSensor {0}", i);
            TempSensor newTempSensor = new TempSensor(sensor.Name);
            tempSensor.Name = newTempSensor.Name;
            tempSensor.MinValue = newTempSensor.MinValue;
            tempSensor.MaxValue = newTempSensor.MaxValue;
            tempSensor.TicksInterval = newTempSensor.TicksInterval;
            tempSensor.ScaleColor= newTempSensor.ScaleColor;
            temperature.RefreshClients();
            return new ObjectResult(sensor);
        }

        // GET: api/sensor
        [Authorize(Roles = "admins")]
        [HttpGet("Refresh")]
        public IEnumerable<TempSensorBase> Refresh()
        {
            temperature.RescanTempSensors();
            return Get();
        }

        // GET: api/sensor
        [Authorize(Roles = "admins")]
        [HttpGet("Reboot")]
        public IActionResult Reboot()
        {
            var p = System.Diagnostics.Process.Start("sudo", "reboot");
            //var p = System.Diagnostics.Process.Start("Notepad");
            _logger.LogInformation("Reboot....");
            return new NoContentResult();
        }

        // GET: api/sensor
        [Authorize(Roles = "admins")]
        [HttpGet("Shutdown")]
        public IActionResult Shutdown()
        {
            var p = System.Diagnostics.Process.Start("sudo", "shutdown -h now");
            //var p = System.Diagnostics.Process.Start("Notepad");
            _logger.LogInformation("Shutdown....");
            return new NoContentResult();
        }

    }
}
