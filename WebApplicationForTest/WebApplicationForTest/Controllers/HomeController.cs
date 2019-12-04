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
           
            return View(); //стартовая стр авторизации
        }
        [HttpPost]
        public ActionResult Index(string Error)
        {

            return View(); //возврат на дом. стр. если ошибка данных
        }
        [HttpPost]
        public ActionResult Menu(string Login, string Password)
        {
            ViewBag.Login = Login;
            ViewBag.Password = Password;
            return View("~/Views/Home/Menu.cshtml"); //открываем меню, соответствующее пользователю
        }
     //   [HttpPost]
     /*   public ActionResult ActionUsers(string LastName, string FirstName, string Otchectvo, string Unit, string Position)
        {
            
           
           return View();
        }
        */
    }
}