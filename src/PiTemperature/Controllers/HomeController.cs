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
    [Route("[controller]"), Route("/")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        [AllowAnonymous]
        [Route("[action]"), Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Index2()
        {
            ViewBag.HideNav = true;
            return View("Index");
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Help()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
