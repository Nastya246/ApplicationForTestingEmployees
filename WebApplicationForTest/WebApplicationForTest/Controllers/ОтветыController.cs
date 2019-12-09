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
    public class ОтветыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Ответы
        public async Task<ActionResult> Index()
        {
            var ответы = db.Ответы.Include(о => о.Вопросы);
            return View(await ответы.ToListAsync());
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
           
          //  var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в => в.Результат_вопроса);
           
            var вопросы = from v in db.Вопросы where v.id_Теста == userТестId select v;

            var ответы = db.Ответы.Include(о => вопросы);
           

            return View(await ответы.ToListAsync());
        }
        // GET: Ответы/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ответы ответы = await db.Ответы.FindAsync(id);
            if (ответы == null)
            {
                return HttpNotFound();
            }
            return View(ответы);
        }

        // GET: Ответы/Create
        public ActionResult Create()
        {
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса");
            return View();
        }

        // POST: Ответы/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_ответа,id_Вопроса,Текст_ответа,Флаг_правильного_ответа")] Ответы ответы)
        {
            if (ModelState.IsValid)
            {
                db.Ответы.Add(ответы);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", ответы.id_Вопроса);
            return View(ответы);
        }

        // GET: Ответы/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ответы ответы = await db.Ответы.FindAsync(id);
            if (ответы == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", ответы.id_Вопроса);
            return View(ответы);
        }

        // POST: Ответы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_ответа,id_Вопроса,Текст_ответа,Флаг_правильного_ответа")] Ответы ответы)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ответы).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", ответы.id_Вопроса);
            return View(ответы);
        }

        // GET: Ответы/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ответы ответы = await db.Ответы.FindAsync(id);
            if (ответы == null)
            {
                return HttpNotFound();
            }
            return View(ответы);
        }

        // POST: Ответы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ответы ответы = await db.Ответы.FindAsync(id);
            db.Ответы.Remove(ответы);
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
