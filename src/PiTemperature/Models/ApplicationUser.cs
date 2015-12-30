using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace PiTemperature.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string DisplayName { get; set; }
    }
}
