using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiTemperature.Meters
{
    public class ScaleColor
    {
        public string FirstColor { get; set; }
        public int FirstDivider { get; set; }
        public string SecondColor { get; set; }
        public int SecondDivider { get; set; }
        public string ThirdColor { get; set; }
        public int ThirdDivider { get; set; }
        public string LastColor { get; set; }
        public ScaleColor()
        {
            FirstColor = "SkyBlue";
            FirstDivider = 20;
            SecondColor = "Khaki";
            SecondDivider = 40;
            ThirdColor = "PaleGreen";
            ThirdDivider = 60;
            LastColor = "LightSalmon";
        }
    }
    public class TempSensorBase
    {
        public string Sensor { get; private set; }
        public string Name { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int TicksInterval { get; set; }
        public ScaleColor ScaleColor { get; set; }
        public TempSensorBase(string sensor)
        {
            Sensor = sensor;
            Name = sensor;
            MinValue = -30;
            MaxValue = 70;
            TicksInterval = 10;
            ScaleColor = new ScaleColor();
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
