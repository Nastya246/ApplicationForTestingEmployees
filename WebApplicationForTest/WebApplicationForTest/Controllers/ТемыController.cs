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
    public class ТемыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Темы
        public async Task<ActionResult> Index() //выбор разделов и тем
        {
            var темы = db.Темы.Include(т => т.Тесты).Include(т=>т.Разделы);
            return View(await темы.ToListAsync());
        }

        // GET: Темы/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Темы темы = await db.Темы.FindAsync(id);
            if (темы == null)
            {
                return HttpNotFound();
            }
            return View(темы);
        }

        // GET: Темы/Create
        public ActionResult Create()
        {
            ViewBag.id_Раздела = new SelectList(db.Разделы, "id_раздела", "Название_раздела");
            return View();
        }

        // POST: Темы/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_темы,id_Раздела,Название_темы")] Темы темы)
        {
            if (ModelState.IsValid)
            {
                db.Темы.Add(темы);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_Раздела = new SelectList(db.Разделы, "id_раздела", "Название_раздела", темы.id_Раздела);
            return View(темы);
        }

        // GET: Темы/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Темы темы = await db.Темы.FindAsync(id);
            if (темы == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Раздела = new SelectList(db.Разделы, "id_раздела", "Название_раздела", темы.id_Раздела);
            return View(темы);
        }

        // POST: Темы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_темы,id_Раздела,Название_темы")] Темы темы)
        {
            if (ModelState.IsValid)
            {
                db.Entry(темы).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Раздела = new SelectList(db.Разделы, "id_раздела", "Название_раздела", темы.id_Раздела);
            return View(темы);
        }

        // GET: Темы/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Темы темы = await db.Темы.FindAsync(id);
            if (темы == null)
            {
                return HttpNotFound();
            }
            return View(темы);
        }

        // POST: Темы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Темы темы = await db.Темы.FindAsync(id);
            db.Темы.Remove(темы);
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
