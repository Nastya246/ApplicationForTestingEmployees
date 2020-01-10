using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationForTest.Models;

namespace WebApplicationForTest.Controllers
{
    public class ТестыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Тесты
        public async Task<ActionResult> Index(string razdel="", string redactor="")
        {
            int id_Section = 0;
            if (redactor == "redactor") //если зашли с учетки редактора, то нужно меню для него
            {
                ViewBag.Id_user = "redactor";
            }
            if (razdel != "")
            {

                id_Section = Convert.ToInt32(razdel);
                var тесты = (db.Тесты.Include(в => в.Разделы).Include(в => в.Вопросы).Where(в => в.Id_Раздела == id_Section));
                ViewBag.IdРаздела = id_Section; //для передачи в представление id раздела
                Разделы раздел = await db.Разделы.FindAsync(id_Section); //находим раздел в бд по id
                ViewBag.НазваниеРаздела = раздел.Название_раздела.Replace("  ", ""); //получаем название раздела
                return View(await тесты.ToListAsync());
            }
            else
            {
               
                var тесты = db.Тесты.Include(т => т.Разделы);
                ViewBag.IdРаздела = 1;
                return View(await тесты.ToListAsync());
            }
  
        }

        [HttpPost] // доступные Темы в разделе
        public async Task<ActionResult> Index( string razdel, int? id_user, string redactor, Тесты тесты)
        {
            int id_Section = 0;

            if (razdel != null)
            {

                id_Section = Convert.ToInt32(razdel);
            }
            else
            {
                id_Section = тесты.Id_Раздела;
            }
            ViewBag.IdРаздела = id_Section;
            if (redactor == "redactor") //если зашли с учетки редактора, то нужно меню для него
            {
                ViewBag.Id_user = "redactor";
            }
            foreach (var e in db.Разделы) //получаем имя раздела по его id
            {
                if (e.id_Раздела==id_Section)
                {
                 
                    ViewBag.НазваниеРаздела = e.Название_раздела; //передаем имя раздела в представление
                   
                }
            }
           
                var тесты1 = (db.Тесты.Include(в => в.Разделы).Include(в => в.Вопросы).Where(в=> в.Id_Раздела==id_Section)); //  список тестов по соответствующей темы
           
            return View(await тесты1.ToListAsync());
        }
        
           
        // GET: Тесты/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Тесты тесты = await db.Тесты.FindAsync(id);
            if (тесты == null)
            {
                return HttpNotFound();
            }
            return View(тесты);
        }

        // GET: Тесты/Create
        public ActionResult Create(int? id)
        {
           // var section = (from t in db.Разделы select t).ToList();
          //  SelectList разделы = new SelectList(section, "id_Раздела", "Название_раздела");
          
          Разделы раздел =  db.Разделы.Find(id);
            ViewBag.Раздел = раздел.id_Раздела;
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста");
            return View();
        }

        // POST: Тесты/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_теста,Название_темы_теста,Количество_вопросов,Id_Раздела")] Тесты тесты, int Id_Раздела)
        {
            тесты.Id_Раздела = Id_Раздела;
            if (ModelState.IsValid)
            {
              
                db.Тесты.Add(тесты);
                db.SaveChanges();
                return RedirectToAction("Index", new { razdel=тесты.Id_Раздела.ToString(), redactor ="redactor"});
            }

            ViewBag.id_Темы = new SelectList(db.Разделы, "id_Раздела", "Название_раздела", тесты.Id_Раздела);
            return View(тесты);
        }

        // GET: Тесты/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Тесты тесты = await db.Тесты.FindAsync(id);
            if (тесты == null)
            {
                return HttpNotFound();
            }
           var section = (from t in db.Разделы where t.id_Раздела == тесты.Id_Раздела select t).ToList();
            SelectList разделы = new SelectList(section, "id_Раздела", "Название_раздела"); //пока что не нужно (для выпадающего списка разделов) 
            ViewBag.Разделы = разделы;
          
            ViewBag.id_Темы = new SelectList(db.Разделы, "id_Раздела", "Название_раздела", тесты.Id_Раздела);
            return View(тесты);
        }

        // POST: Тесты/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_теста,Название_темы_теста,Количество_вопросов,Id_Раздела")] Тесты тесты)
        {
            if (ModelState.IsValid)
            {
                db.Entry(тесты).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { razdel = тесты.Id_Раздела.ToString(), redactor = "redactor" });
            }
            ViewBag.id_Темы = new SelectList(db.Разделы, "id_Раздела", "Название_раздела", тесты.Id_Раздела);
            return View(тесты);
        }

        // GET: Тесты/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Тесты тесты = await db.Тесты.FindAsync(id);
            if (тесты == null)
            {
                return HttpNotFound();
            }
            return View(тесты);
        }

        // POST: Тесты/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Тесты тесты = await db.Тесты.FindAsync(id);
            db.Тесты.Remove(тесты);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { razdel = тесты.Id_Раздела.ToString(), redactor = "redactor" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
