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
    public class ВопросыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Вопросы
        public async Task<ActionResult> Index()
        {
            var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в=>в.Тесты);
            return View(await вопросы.ToListAsync());
        }
        [HttpPost] // доступные вопросы по тесту
        public async Task<ActionResult> Index(Тесты item)
        {
            string nameTest = item.Название_теста;
            ViewBag.НазваниеТеста = nameTest;
            int userТестId = 0;
            foreach (var t in db.Тесты)
            {
                if (t.Название_теста == nameTest)
                {
                    userТестId = t.id_теста;
                }
            }
            var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в => в.Ответы).Include(в=>в.Результат_вопроса);
            вопросы = (from v in db.Вопросы where v.id_Теста == userТестId select v).Include(v=>v.Ответы);
           
            return View(await вопросы.ToListAsync());
        }
        // GET: Вопросы/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            return View(вопросы);
        }

        // GET: Вопросы/Create
        public ActionResult Create()
        {
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_теста");
            return View();
        }

        // POST: Вопросы/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_вопроса,id_Теста,Текст_вопроса,Тип_ответа")] Вопросы вопросы)
        {
            if (ModelState.IsValid)
            {
                db.Вопросы.Add(вопросы);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_теста", вопросы.id_Теста);
            return View(вопросы);
        }

        // GET: Вопросы/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_теста", вопросы.id_Теста);
            return View(вопросы);
        }

        // POST: Вопросы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_вопроса,id_Теста,Текст_вопроса,Тип_ответа")] Вопросы вопросы)
        {
            if (ModelState.IsValid)
            {
                db.Entry(вопросы).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_теста", вопросы.id_Теста);
            return View(вопросы);
        }

        // GET: Вопросы/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            return View(вопросы);
        }

        // POST: Вопросы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            db.Вопросы.Remove(вопросы);
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
