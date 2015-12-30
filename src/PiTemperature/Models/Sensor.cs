using System.ComponentModel.DataAnnotations;

namespace PiTemperature.Models
{
    public class Sensor
    {
        public string Id { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
