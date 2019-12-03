using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest.Models;

namespace WebApplicationForTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            return View();
        }
      
        [HttpPost]
        public ActionResult Menu(string login, string password)
        {
            ViewBag.Login = login;
            ViewBag.Password = password;
            return View();
        }
        [HttpPost]
        public ActionResult ActionUsers(string login, string password)
        {
           
            return View();
        }
    }
}