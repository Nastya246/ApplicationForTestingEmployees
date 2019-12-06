using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationForTest.Controllers
{
    public class ActionsController : Controller
    {
        // GET: Actions
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ActionUsers(string LastName, string FirstName, string Otchectvo, string Unit, string Position)
        {

           
            return View();
        }
        [HttpPost]
        public ActionResult ResultUsesrs(string LastName, string FirstName, string Otchectvo, string Unit, string Position)
        {


            return View();

        }
    }
}