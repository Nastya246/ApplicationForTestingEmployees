using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest;
using System.Data.Entity;
using WebApplicationForTest.Models;
using System.Threading.Tasks;
///сделать контроль регистра
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
            ///////////////////////////////////
         //   Dictionary<string, string> loginPass = new Dictionary<string, string>(db.Пользователи.Count()); //отправляем все логины и пароли в представление

         /*   foreach (var i in db.Пользователи)
            {
                loginPass.Add(i.Логин.Replace(" ", ""), i.Пароль.Replace(" ", ""));
            }
            ViewBag.LoginPassword = loginPass;
            */
            //////////////////////////////
            int selectedIndex = 0;
            
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
            int flagLog = 0;
            int flagPas = 0;
            foreach (var temp in db.Пользователи) //если ввели логи и пароль, то корректно ли
            {
                if (temp.Логин.Replace(" ", "") == Login)
                {
                    flagLog = 1;
                    if (temp.Пароль.Replace(" ", "") == Password)
                    {
                        flagPas = 1;
                    }
                    }
            }
            if ((flagLog==1)&&(flagPas==1))
                    {
                ViewBag.LogPas = "Success";
            }

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
                    string passw = login + Date.Substring(0, 2) + Date.Substring(3, 2); //пароль пользователя
                    пользователь.Логин = login;
                    пользователь.Пароль = passw;
                    db.Пользователи.Add(пользователь); //добавляем пользователя в БД
                    await db.SaveChangesAsync(); // сохраняем изменения
                    ViewBag.Id_user = пользователь.id_user; // передаем данные пользоватля в представление
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