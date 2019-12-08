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

        [HttpPost] // доступные тесты по теме
        public async Task<ActionResult> Index(Темы item)
        {
            string usersTopic = item.Название_темы;
            ViewBag.НазваниеТемы = usersTopic;
            int userTopicId = 0;
            foreach (var t in db.Темы)
            {
                if (t.Название_темы==usersTopic)
                {
                    userTopicId = t.id_темы;
                }
            }
            var тесты = db.Тесты.Include(в => в.Темы).Include(в=>в.Вопросы);
            тесты = from t in db.Тесты where t.id_Темы == userTopicId select t;
           
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
