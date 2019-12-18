using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest;
using System.Data.Entity;
using WebApplicationForTest.Models;
using System.Threading.Tasks;
namespace WebApplicationForTest.Controllersd
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
            
            int selectedIndex = 1;
            SelectList подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", selectedIndex); //для передачи в представление списка подразделений
            ViewBag.Подразделения = подразделения;

            List<Должность> ДолжностьList = db.Должность.ToList(); // для передачи в представление списка должностей
            ДолжностьList.Clear();
            foreach (Подразделение temp in db.Подразделение.Include(t => t.Должность))
            {
                if (temp.Id_подразделения == selectedIndex)
                {
                    foreach (Должность temp2 in temp.Должность)
                    {
                        ДолжностьList.Add(temp2);
                    }
                }
               
            }
            SelectList должности = new SelectList(ДолжностьList, "Id_должности", "Название_должности");
            ViewBag.Должности = должности;
           
            ViewBag.Login = Login;
            ViewBag.Password = Password;
        
            return View(); //открываем меню, соответ. пользователю
           
         
        }
        public ActionResult GetItems(int id) //для динамического обновления списка должностей при выборе другого подразделения
        {
           
            List<Должность> ДолжностьList1 = db.Должность.ToList();
            ДолжностьList1.Clear();
            foreach (Подразделение temp in db.Подразделение.Include(t => t.Должность))
            {
                if (temp.Id_подразделения == id)
                {
                    foreach (Должность temp2 in temp.Должность)
                    {
                        ДолжностьList1.Add(temp2);
                       
                    }
                }
            }
           
            return PartialView(ДолжностьList1.ToList());
        }
      
        public ActionResult AutocompleteSearch(string term) // автодополнение слов, пока что не используется
        {
            var models = db.Подразделение.Where(a => a.Название_подразделения.Contains(term))
                            .Select(a => new { value = a.Название_подразделения })
                            .Distinct();

            return Json(models, JsonRequestBehavior.AllowGet);
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