using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest.Models;
using System.Threading.Tasks;
using System.Data.Entity;
namespace WebApplicationForTest.Controllers
{
    public class ПрактикаController : Controller
    {
        private TestEntities db = new TestEntities();
        // GET: Практика
        public ActionResult Index(int topic=0, int units=-1, string nameUser="")
        {
            ViewBag.id_topic = topic;
            ViewBag.nameUser = nameUser;
            ViewBag.id_units = units;
            int selectedIndex = 0;
            SelectList подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", selectedIndex); //для передачи в представление списка подразделений
            ViewBag.Подразделения = подразделения;
            Тесты тема = db.Тесты.Find(topic);
            ViewBag.razdel = (тема.Id_Раздела).ToString();
            ViewBag.НазваниеТеста = тема.Название_темы_теста.Replace("  ", "");
            IQueryable<Результат_теста> users;
            if (units != -1) //если подразделение указано
            {
                if (nameUser != "") //если указана фамилия
                {
                    users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста && u.Пользователи.id_подразделения == units && u.Пользователи.Фамилия.Contains(nameUser) select u); //выбор тех, кто уже сдавал теорию
                }
                else //если поле для фамилии пусто
                {
                    users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста && u.Пользователи.id_подразделения == units select u); //выбор тех, кто уже сдавал теорию
                }
            }
            else // если подразделение не указано то выводим всех пользователй
            {
                if (nameUser != "") //если указана фамилия
                {
                    users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста && u.Пользователи.Фамилия.Contains(nameUser) select u); //выбор тех, кто уже сдавал теорию
                }
                else
                {
                    users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста select u); //выбор тех, кто уже сдавал теорию
                }
                }
            List<Пользователи> пользователи = new List<Пользователи>();
            foreach (var temp in users)
            {
                пользователи.Add(temp.Пользователи);
               // пользователи.Add(db.Пользователи.Find(temp.id_User));
            }
           
            var sortedUsers = from u in пользователи orderby u.Фамилия select u; //сортируем пользователей по алфавиту

            return View(sortedUsers);
        }
        [HttpPost]
        public async Task <ActionResult> Index(int razdel, Тесты item, int? Units)
        {
            Разделы разделы = await db.Разделы.FindAsync(razdel); //получаем раздел по Id
            ViewBag.razdelName = разделы.Название_раздела.Replace("  ", ""); //передаем название раздела в представлнение
            ViewBag.razdel = razdel.ToString(); //Передаем Id раздела в представление
            string nameTest = item.Название_темы_теста;
            ViewBag.НазваниеТеста = nameTest.Replace("  ", ""); //передаем название теста в представление
            int selectedIndex = 0;
            SelectList подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", selectedIndex); //для передачи в представление списка подразделений
            ViewBag.Подразделения = подразделения;
            int userТестId = 0;
            foreach (var t in db.Тесты) //определяем id выбранного теста
            {
                if (t.Название_темы_теста == nameTest)
                {
                    userТестId = t.id_теста;
                }
            }
            ViewBag.id_topic = userТестId;
            Тесты тема = await db.Тесты.FindAsync(userТестId);
            IQueryable<Результат_теста> users;
            if (Units != null)
            {
                users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста && u.Пользователи.id_подразделения ==Units select u); //выбор тех, кто уже сдавал теорию
            }
            else
            {
                users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста  select u); //выбор тех, кто уже сдавал теорию

            }
            List<Пользователи> пользователи = new List<Пользователи>();
            foreach (var temp in users)
            {
                // пользователи.Add(db.Пользователи.Find(temp.id_User));
                пользователи.Add(temp.Пользователи);
            }
            var sortedUsers = from u in пользователи orderby u.Фамилия select u; //сортируем пользователей по алфавиту
           
           /* List<string> status = new List<string>();
            status.Add("Не сдавалось");
            status.Add("Не сдано");
            status.Add("Сдано");
            SelectList statusPractice = new SelectList(status, status[0]); //для передачи в представление списка подразделений
            ViewBag.statusPractice = statusPractice;*/
            return View(sortedUsers);
        }
        [HttpPost]
       public async Task <ActionResult> EditData (FormCollection form)
        {
            string name = "";
            int unit = -1;
            int id_Test = 0;
            var k = form.Keys;
            Dictionary<string, string> resultA = new Dictionary<string, string>(k.Count); //для хранения первичных результатов
            var temp = form.ToValueProvider(); //получаем все данные
            string kD = ""; // для ключа словаря
            string valD = "";
            foreach (var val in k) //заполняем словарь значениями, которые получили
            {

                kD = val.ToString().Replace(" ", "");
               
                if (resultA.ContainsKey(kD)) //если есть запись в словаре под таким ключом, то удаляем, по идеи ее быть не должно, но мало ли
                {
                    resultA.Remove(kD);
                }
                if (kD == "topic") //если получили id_теста, сохраняем в отдельной переменной
                {
                    string[] mystringTopic = (temp.GetValue(kD).AttemptedValue).Split(',');
                    if (mystringTopic.Count() > 0)
                    {
                        id_Test = Convert.ToInt32(mystringTopic[0]);

                    }
                }
                else if (kD == "units") //если получили id_теста, сохраняем в отдельной переменной
                {
                    var tempU= temp.GetValue(kD).AttemptedValue;
                    string[] mystringUnit = (temp.GetValue(kD).AttemptedValue).Split(',');
                    var tempString = mystringUnit.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    mystringUnit = tempString;
                    if (mystringUnit.Count() > 0)
                    {
                        unit = Convert.ToInt32(mystringUnit[0]);
                        
                    }
                    //  unit = Convert.ToInt32(temp.GetValue(kD).AttemptedValue);
                }
                else if (kD== "userName")
                {
                    var tempN = temp.GetValue(kD).AttemptedValue;
                    string[] mystringName = (temp.GetValue(kD).AttemptedValue).Split(',');
                    if (mystringName.Count() > 0)
                    {
                        name = mystringName[0];
                      
                    }
                }
                else
                {
                    valD = temp.GetValue(kD).AttemptedValue.Replace("  ", "");
                    resultA.Add(kD, valD);
                }
            }
             
            if (resultA.Count()>0) //обработка результатов
            {
                foreach (var resultP in resultA)
                {
                    string tempNo = "Не сдавалось";
                    string tempNot = "Не сдано";
                    string tempYes = "Сдано";
                    string[] stringStatus = resultP.Value.Split(',');
                    string status = stringStatus[0];
                   
                    if (String.Equals(status, tempNo))
                    {
                        continue;
                    }
                    else
                    {
                        string[] stringUser = resultP.Key.Split(','); //обновляем инфу о пользователе
                        int idT = Convert.ToInt32(stringUser[1]);
                        int idU = Convert.ToInt32(stringUser[0]);
                        var usResult = await (from u in db.Результат_теста where u.id_User == idU && u.id_Теста == idT select u).FirstAsync();
                        Результат_теста результат_Теста = db.Результат_теста.Find(usResult.id_результата_теста);
                       
                        результат_Теста.Дата_сдачи_практики = Convert.ToDateTime(stringStatus[1]);
                        if (String.Equals(status,tempNot))
                        {
                            результат_Теста.Отметка_о_практике = false;
                        }
                        else if (String.Equals(status, tempYes))
                        {
                            результат_Теста.Отметка_о_практике = true;
                        }
                        if ((результат_Теста.Отметка_о_практике == true) &&(результат_Теста.Оценка_за_теорию>=60))
                        {
                            результат_Теста.Общий_результат = "Успех";
                        }
                       else
                        {
                            результат_Теста.Общий_результат = null;
                        }
                        db.Entry(результат_Теста).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index", new { topic = id_Test, units=unit, nameUser=name});
        }
        [HttpPost]
        public async Task <ActionResult> UserSearch(int idTopic, int? Units, int? units, string name = "")
        {
           
            ViewBag.id_topic = idTopic;
          
            var alluserResult = await db.Результат_теста.Where(r => r.id_Теста == idTopic).ToListAsync();
            List<Пользователи> alluser;
            ViewBag.nameUser = name;
            if (Units != null) //если выбрано подразделение
            {
                ViewBag.id_units = Units;
                if (name != "") //если введено имя
                {
                    
                    alluser = await db.Пользователи.Where(r => (r.Фамилия.Contains(name) && r.id_подразделения == Units)).ToListAsync();
                }
                else //если поле для имени пустое
                {
                    alluser = await db.Пользователи.Where(r => r.id_подразделения == Units).ToListAsync();
                }
            }
            else if ((units!=null) && (units!=-1))
            {
                
                ViewBag.id_units = units;
                if (name != "") //если введено имя
                {
                    
                    alluser = await db.Пользователи.Where(r => (r.Фамилия.Contains(name) && r.id_подразделения == units)).ToListAsync();
                }
                else //если поле для имени пустое
                {
                    alluser = await db.Пользователи.Where(r => r.id_подразделения == units).ToListAsync();
                }
            }
            else //если не выбранно подразделение
            {
                ViewBag.id_units = -1;
                if (name != "")
                {
                   
                    alluser = await db.Пользователи.Where(r => r.Фамилия.Contains(name)).ToListAsync();
                }
                else
                {
                    alluser = await db.Пользователи.ToListAsync();
                }
            }
            List<Пользователи> us = new List<Пользователи>();
            foreach (var r in alluserResult)
            {
               foreach (var u in alluser)
                {
                    if (u.id_user==r.id_User)
                    {
                        us.Add(u);
                    }
                }
            }

           
            return PartialView(us);
        }
     
       
    }
}
