using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PiTemperature.Meters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiTemperature.Hubs
{
    [HubName("tempHub")]
    public class TempHub : Hub
    {
        private readonly Temperature temperature;
        public TempHub(Temperature temperature)
        {
            this.temperature = temperature;
        }

        public void GetTempSensors()
        {
            Clients.Caller.broadcastTempSensors(temperature.TempSensors);
        }
    }
}
