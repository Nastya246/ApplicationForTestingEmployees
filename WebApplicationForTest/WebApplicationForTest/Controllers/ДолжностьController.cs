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
    public class ДолжностьController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Должность
        public async Task<ActionResult> Index()
        {
            return View(await db.Должность.ToListAsync());
        }

        // GET: Должность/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Должность должность = await db.Должность.FindAsync(id);
            if (должность == null)
            {
                return HttpNotFound();
            }
            return View(должность);
        }

        // GET: Должность/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Должность/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Название_должности,Id_должности")] Должность должность)
        {
            if (ModelState.IsValid)
            {
                db.Должность.Add(должность);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(должность);
        }

        // GET: Должность/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Должность должность = await db.Должность.FindAsync(id);
            if (должность == null)
            {
                return HttpNotFound();
            }
            return View(должность);
        }

        // POST: Должность/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Название_должности,Id_должности")] Должность должность)
        {
            if (ModelState.IsValid)
            {
                db.Entry(должность).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(должность);
        }

        // GET: Должность/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Должность должность = await db.Должность.FindAsync(id);
            if (должность == null)
            {
                return HttpNotFound();
            }
            return View(должность);
        }

        // POST: Должность/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Должность должность = await db.Должность.FindAsync(id);
            db.Должность.Remove(должность);
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
