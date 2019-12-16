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
        public async Task<ActionResult> Menu(string Login, string Password)
        {
            SelectList units = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения");
            ViewBag.Units = units;
            SelectList positions = new SelectList(db.Должность, "Id_должности", "Название_должности");
            ViewBag.Positions = positions;
            ViewBag.Login = Login;
            ViewBag.Password = Password;
            var подразделения = db.Подразделение.Include(p => p.Должность);
            return View(await подразделения.ToListAsync());
           
          //  return View("~/Views/Home/Menu.cshtml"); //открываем меню, соответствующее пользователю
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(string LastName, string FirstName, string Otchectvo, string Id_подразделения, string Id_должности)
        {

            var UnitTemp = from v in db.Должность where v.Id_должности == Convert.ToInt32(Id_должности) select v;
            if (UnitTemp != null)
            {
                ViewBag.Flag = 1;
                Пользователи newПользователь = new Пользователи();
                newПользователь.id_подразделения = Convert.ToInt32(Id_подразделения);
                newПользователь.Имя = FirstName;
                newПользователь.Фамилия = LastName;
                newПользователь.Отчество = Otchectvo;
                
            }
            else
            {
                ViewBag.Flag = 0; //такой должности в подразделении нет
            }
          

              

              
            return View(); 
        }


    }
}