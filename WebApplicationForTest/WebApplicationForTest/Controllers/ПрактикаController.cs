using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest.Models;
using System.Threading.Tasks;
namespace WebApplicationForTest.Controllers
{
    public class ПрактикаController : Controller
    {
        private TestEntities db = new TestEntities();
        // GET: Практика
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(int razdel, Тесты item)
        {
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
            Тесты тема = db.Тесты.Find(userТестId);
            var users =  (from u in db.Результат_теста where u.id_Теста == тема.id_теста select u); //выбор тех, кто уже сдавал теорию
            //добавить viewbag
            List<Пользователи> пользователи = new List<Пользователи>();
            foreach (var temp in users)
            {
                пользователи.Add(db.Пользователи.Find(temp.id_User));
            }
            
            return View(пользователи);
        }
        [HttpPost]
        public ActionResult UserSearch(string name)
        {
            List<string> lSelectP = new List<string>(3);
            lSelectP.Add("Сдано");
            lSelectP.Add("Не сдано");
            lSelectP.Add("Не сдавалось");
            SelectList отметка = new SelectList(lSelectP); 
            ViewBag.Отметка = отметка;
            var alluser = db.Пользователи.Where(a => a.Фамилия.Contains(name)).ToList();
            if (alluser.Count <= 0)
            {
                return HttpNotFound();
            }
            return PartialView(alluser);
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
