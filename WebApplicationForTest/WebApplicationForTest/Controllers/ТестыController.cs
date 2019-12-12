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
        public async Task<ActionResult> Index()
        {
            var тесты = db.Тесты.Include(т => т.Темы);
            return View(await тесты.ToListAsync());
        }

        [HttpPost] // доступные вопросы по тесту
        public async Task<ActionResult> Index( Темы itemO)
        {
            int temp=0;
             int  idTopic = itemO.id_темы;
            foreach(var e in db.Темы) //получаем имя темы по ее id
            {
                if (e.id_темы==idTopic)
                {
                 
                    ViewBag.НазваниеТемы = e.Название_темы; //передаем имя темы в представление
                   temp  = e.id_Раздела;
                }
            }
            foreach (var e in db.Разделы) //получаем имя раздела по его id
            {
                if (e.id_раздела == temp)
                {

                    ViewBag.НазваниеРаздела = e.Название_раздела; //передаем имя раздела в представление

                }
            }
            
                var тесты = (db.Тесты.Include(в => в.Темы).Include(в => в.Вопросы).Where(в=> в.id_Темы==idTopic)); //  список тестов по соответствующей темы
           
            return View(await тесты.ToListAsync());
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
        public ActionResult Create()
        {
            ViewBag.id_Темы = new SelectList(db.Темы, "id_темы", "Название_темы");
            return View();
        }

        // POST: Тесты/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_теста,Название_теста,Количество_вопросов,id_Темы")] Тесты тесты)
        {
            if (ModelState.IsValid)
            {
                db.Тесты.Add(тесты);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_Темы = new SelectList(db.Темы, "id_темы", "Название_темы", тесты.id_Темы);
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
            ViewBag.id_Темы = new SelectList(db.Темы, "id_темы", "Название_темы", тесты.id_Темы);
            return View(тесты);
        }

        // POST: Тесты/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_теста,Название_теста,Количество_вопросов,id_Темы")] Тесты тесты)
        {
            if (ModelState.IsValid)
            {
                db.Entry(тесты).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Темы = new SelectList(db.Темы, "id_темы", "Название_темы", тесты.id_Темы);
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
            return RedirectToAction("Index");
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
