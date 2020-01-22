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
        public ActionResult Index(int topic=0)
        {
            ViewBag.id_topic = topic;
            
           Тесты тема = db.Тесты.Find(topic);
            ViewBag.razdel = (тема.Id_Раздела).ToString();
            ViewBag.НазваниеТеста = тема.Название_темы_теста.Replace("  ", "");
           var users = (from u in db.Результат_теста where u.id_Теста == тема.id_теста select u); //выбор тех, кто уже сдавал теорию
            
            List<Пользователи> пользователи = new List<Пользователи>();
            foreach (var temp in users)
            {
                пользователи.Add(db.Пользователи.Find(temp.id_User));
            }
           
            var sortedUsers = from u in пользователи orderby u.Фамилия select u; //сортируем пользователей по алфавиту

            return View(sortedUsers);
        }
        [HttpPost]
        public ActionResult Index(int razdel, Тесты item)
        {
            ViewBag.razdel = razdel.ToString();
            string nameTest = item.Название_темы_теста;
            ViewBag.НазваниеТеста = nameTest; //передаем название теста в представление

            int userТестId = 0;
            foreach (var t in db.Тесты) //определяем id выбранного теста
            {
                if (t.Название_темы_теста == nameTest)
                {
                    userТестId = t.id_теста;
                }
            }
            ViewBag.id_topic = userТестId;
            Тесты тема = db.Тесты.Find(userТестId);
            var users =  (from u in db.Результат_теста where u.id_Теста == тема.id_теста select u); //выбор тех, кто уже сдавал теорию
            
            List<Пользователи> пользователи = new List<Пользователи>();
            foreach (var temp in users)
            {
                пользователи.Add(db.Пользователи.Find(temp.id_User));
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
                    if (mystringTopic.Count()>0)
                    {
                        id_Test = Convert.ToInt32(mystringTopic[0]);
                       
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
                    string[] stringStatus = resultP.Value.Split(',');
                    if (stringStatus[0].Replace(" ","")=="Несдавалось")
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
                        if (stringStatus[0].Replace(" ", "") == "Cдано")
                        {
                            результат_Теста.Отметка_о_практике = true;
                        }
                        else if ((stringStatus[0].Replace(" ", "") == "Несдано"))
                        {
                            результат_Теста.Отметка_о_практике = false;
                        }
                        if ((результат_Теста.Отметка_о_практике == true) &&(результат_Теста.Оценка_за_теорию>=60))
                        {
                            результат_Теста.Общий_результат = "Успех";
                        }
                        db.Entry(результат_Теста).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index", new { topic = id_Test});
        }
        [HttpPost]
        public ActionResult UserSearch(string name, int idTopic)
        {
            /* List<string> lSelectP = new List<string>(3);
             lSelectP.Add("Сдано");
             lSelectP.Add("Не сдано");
             lSelectP.Add("Не сдавалось");
             SelectList отметка = new SelectList(lSelectP); 
             */
            // ViewBag.Отметка = отметка;
            @ViewBag.id_topic = idTopic;
             var alluserResult = db.Результат_теста.Where(r => r.id_Теста == idTopic).ToList();
            var alluser = db.Пользователи.Where(r => r.Фамилия.Contains(name)).ToList();
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

            if (us.Count <= 0)
            {
                return HttpNotFound();
            }
            return PartialView(us);
        }
        // GET: Практика/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Практика/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Практика/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Практика/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Практика/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Практика/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Практика/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
