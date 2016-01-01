using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiTemperature.Meters;
using Microsoft.AspNet.Authorization;

namespace PiTemperature.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TempController : Controller
    {
        private readonly Temperature temperature;
        public TempController(Temperature temperature)
        {
            this.temperature = temperature;
        }

        // GET: api/temp
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<TempSensor> Get()
        {
            return temperature.TempSensors.Select(c => new TempSensor(c.Sensor) { Name = c.Name, Temp = c.Temp });
        }


        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public TempSensor Get(string id)
        {
            return temperature.GetTempSensor(id);
        }
    }
}
