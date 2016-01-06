using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PiTemperature.Models
{
    public class ApplicationDbContextOptions
    {
        public string DefaultUsername { get; set; }
        public string DefaultPassword { get; set; }
    }

    public class ApplicationDbMigrationHistory
    {
        public int Id { get; set; }
        public int MigrationId { get; set; }
        [MaxLength(512)]
        public string Note { get; set; }
    }

}
