using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiTemperature.Meters
{
    public class TempSensorBase
    {
        public string Sensor { get; private set; }
        public string Name { get; set; }
        public TempSensorBase(string sensor)
        {
            Sensor = sensor;
            Name = sensor;
        }
    }
    public class TempSensor : TempSensorBase
    {       
        public double Temp { get; set; }        

        public TempSensor(string sensor) : base(sensor)
        {
            Name = sensor;
            Temp = 0.0;
        }
    }

    public class TempsensorOldTemp : TempSensor
    {
        public double Oldtemp { get; set; }

        public TempsensorOldTemp(string sensor) : base(sensor)
        {
            Oldtemp = -255.0;
        }
    }
}
