using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiTemperature.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        [AllowAnonymous]
        [Route("[action]"), Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admins")]
        [Route("[action]")]
        public IActionResult AdminSensors()
        {
            return View();
        }

        [Authorize(Roles = "admins")]
        [Route("[action]")]
        public IActionResult Shutdown()
        {
            var p = System.Diagnostics.Process.Start("shutdown", "h now");
            p.WaitForExit(2000);
            return View("Index");
        }

    }
}
