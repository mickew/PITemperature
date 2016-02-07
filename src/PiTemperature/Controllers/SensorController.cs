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
            return temperature.TempSensors.Select(c => new TempSensorBase(c.Sensor) { Name = c.Name });
        }

        [Authorize(Roles = "admins")]
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
            tempSensor.Name = sensor.Name;
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
            p.WaitForExit(1000);
            if (p.HasExited && p.ExitCode != 0)
            {
                _logger.LogWarning(string.Format("Reboot not working Exit = {0}", p.ExitCode));
            }
            else
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
            p.WaitForExit(1000);
            if (p.HasExited && p.ExitCode != 0)
            {
                _logger.LogWarning(string.Format("Shutdown not working Exit = {0}", p.ExitCode));
            }
            else
                _logger.LogInformation("Shutdown....");
            return new NoContentResult();
        }

    }
}
