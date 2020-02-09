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
       
            int selectedIndex = 0;
            
            SelectList подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", selectedIndex); //для передачи в представление списка подразделений
            ViewBag.Подразделения = подразделения;

            List<Должность> ДолжностьList = db.Должность.ToList(); // для передачи в представление списка должностей
            ДолжностьList.Clear();
            foreach (Подразделение temp in db.Подразделение.Include(t => t.ДолжностьПодразделение))
            {
                if (temp.Id_подразделения == selectedIndex)
                {
                    foreach (var temp2 in temp.ДолжностьПодразделение)
                    {
                        
                        ДолжностьList.Add(temp2.Должность);
                    }
                }      
            }
            SelectList должности = new SelectList(ДолжностьList, "Id_должности", "Название_должности");
            ViewBag.Должности = должности;
           
            ViewBag.Login = Login;
            ViewBag.Password = Password;
            
            //если ввели логин и пароль, то корректно ли
            var userLogin = await (from user in db.Пользователи where user.Пароль.Replace(" ", "") == Password && user.Логин.Replace(" ", "") == Login select user).ToListAsync();
           
            if (userLogin.Count()!=0)
            {

                ViewBag.LogPas = "Success";
                ViewBag.Id_user = userLogin.First().id_user;
                ViewBag.Data = @DateTime.Now.ToString("yyyy/MM/dd");
            }
            if ((Login == "redactor") && (Password == "redactor12345"))
            {
                List<string> ls=new List<string> (await (db.Разделы.CountAsync()));
                foreach (var r in db.Разделы)
                {
                    ls.Add(r.Название_раздела.Replace("  ", ""));
                }
                ViewBag.ListSection = ls;
            }
            return View(); //открываем меню, соответ. пользователю
        }
        //метод для динамического обновления списка должностей при выборе другого подразделения
        public async Task <ActionResult> GetItems(int id) 
        {

            List<Должность> ДолжностьList1 = new List<Должность>();
            var подразделение = await (db.Подразделение.Include(t => t.ДолжностьПодразделение)).ToListAsync();
            foreach (Подразделение temp in подразделение)
            {
                if (temp.Id_подразделения == id)
                {
                    foreach (var temp2 in temp.ДолжностьПодразделение)
                    {
                        ДолжностьList1.Add(await db.Должность.FindAsync(temp2.id_должности));
                       
                    }
                }
            }
           
            return PartialView(ДолжностьList1.ToList());
        }
        // метод автодополнения слов, пока что не используется
        public ActionResult AutocompleteSearch(string term) 
        {
            var models = db.Подразделение.Where(a => a.Название_подразделения.Contains(term))
                            .Select(a => new { value = a.Название_подразделения })
                            .Distinct();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public bool LoginEx(string login) //проверяем логин на существование
        {
            foreach (var temp in db.Пользователи) 
            {
                if (temp.Логин.Replace(" ", "") == login)
                {
                    return true;
                }

            }
                return false;
        }
        //Проверка на существование пользователя в бд, если его там нет, то добавляем
        [HttpPost]
        public async Task<ActionResult> AddUser(string LastName, string FirstName, string Otchectvo, int? Units, int? Position,  string Date)
        {

            if ((LastName != null) && (FirstName != null) && (Units != null) && (Position != null)) //проверка, что введены все данные
            {
                ViewBag.CorrectData = "1";
               int flagEx = 0;
                foreach (var temp in db.Пользователи) //ищем пользователя с такими же данными
                {
                    if ((temp.Фамилия.Replace(" ", "") == LastName)&&(temp.Имя.Replace(" ", "") == FirstName) &&(temp.Отчество.Replace(" ", "") == Otchectvo) &&(temp.id_должности == Position) &&(temp.id_подразделения == Units))
                    {
                                        flagEx = 1;
                    }
                }
                if (flagEx == 0) //если нет пользователя с такими данными - вносим его в Бд
                {
                    ViewBag.Data = Date;
                    Пользователи пользователь = new Пользователи();
                    пользователь.Фамилия = LastName;
                    пользователь.Имя = FirstName;
                    пользователь.Отчество = Otchectvo;
                    пользователь.id_подразделения = (int)Units;
                    пользователь.id_должности = (int)Position;
                    string login = LastName.Substring(0, 1) + FirstName.Substring(0, 1) + Otchectvo.Substring(0, 1); //его логин

                    if (LoginEx(login)) //если логин есть
                    {
                        string loginTemp = login;
                        int loginDigital = 0;
                        while (LoginEx(login)) //подбираем логин, котрый не занят 
                        {
                            login = loginTemp;
                            login = login + loginDigital.ToString();
                            loginDigital++;
                        }

                    }
                    string passw = login + Date.Substring(5, 2) + Date.Substring(8, 2); //пароль пользователя
                    пользователь.Логин = login;
                    пользователь.Пароль = passw;
                    db.Пользователи.Add(пользователь); //добавляем пользователя в БД
                    await db.SaveChangesAsync(); // сохраняем изменения
                    ViewBag.Id_user = пользователь.id_user; // передаем данные пользоватля в представление
                    ViewBag.Data = Date;
                    ViewBag.Login = login;
                    ViewBag.Passw = passw;
                }
                else //если такой пользователь уже есть
                {
                    ViewBag.CorrectData = "2";
                }

                return View();
            }
            else
            {
                ViewBag.CorrectData = "3";
                return View();
            }
           

        }
        
    
    }
    
}