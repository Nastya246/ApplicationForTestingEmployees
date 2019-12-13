using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest;
using System.Data.Entity;
using System.Linq;
using WebApplicationForTest.Models;
using System.Threading.Tasks;
namespace WebApplicationForTest.Controllers
{
    public class HomeController : Controller
    {
        private TestEntities db = new TestEntities();
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
      

        [HttpPost]
        public ActionResult UserAdd(string LastName, string FirstName, string Otchectvo, string Unit, string Position)
        {
            bool exist = false; //флаг существования сотрудника в бд
            var userUnit = db.Пользователи.Include(v => v.Подразделение.Должность);
            /*  foreach (var user in userUnit)
              {
                  if ((user.Имя == FirstName) && (user.Фамилия == LastName) && (user.Отчество == Otchectvo) && (user.Подразделение.Название_подразделения == Unit))
                  {
                      foreach (var u in userUnit.)
                          {
                          if ((u.Название_должности == Position))
                          {
                              exist = true; //сотрудник на такой должности есть
                          }
                      }
                  }
              }*/

            var us = from v in db.Пользователи.Include((v => v.Подразделение.Должность)) where (v.Имя==FirstName && v.Фамилия==LastName && v.Отчество==Otchectvo && v.Подразделение.Название_подразделения == Unit ) select v;
            if (us != null) //пользователь с таким именем в этом подразделении  есть
            {
                exist = true;
            }
            else
            {
               // Пользователи user = new Пользователи();
            }
          //  var разделы = db.Разделы.Include(т => т.Темы);
          //  return View(await разделы.ToListAsync());
            return View("~/Views/Разделы/Index.cshtml");
        }
        
    }
}