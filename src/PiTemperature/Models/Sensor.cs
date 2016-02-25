using System.ComponentModel.DataAnnotations;

namespace PiTemperature.Models
{
    public class Sensor
    {
        public string Id { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [Range(-55, 125)]
        public int MinValue { get; set; }
        [Range(-55, 125)]
        public int MaxValue { get; set; }
        [Range(0, 100)]
        public int TicksInterval { get; set; }
        [MaxLength(50)]
        public string FirstColor { get; set; }
        [Range(0, 100)]
        public int FirstDivider { get; set; }
        [MaxLength(50)]
        public string SecondColor { get; set; }
        [Range(0, 100)]
        public int SecondDivider { get; set; }
        [MaxLength(50)]
        public string ThirdColor { get; set; }
        [Range(0, 100)]
        public int ThirdDivider { get; set; }
        [MaxLength(50)]
        public string LastColor { get; set; }
    }
}
