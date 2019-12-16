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
    public class РазделыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Разделы
        public async Task<ActionResult> Index()
        {

            var  разделы = db.Разделы.Include(т => т.Темы);
            return View(await разделы.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult> Index(string LastName, string FirstName,string Otchectvo, string Id_подразделения, string Id_должности)
        {
            
            var разделы = db.Разделы.Include(т => т.Темы);
            return View(await разделы.ToListAsync());
        }
        // GET: Разделы/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Разделы разделы = await db.Разделы.FindAsync(id);
            if (разделы == null)
            {
                return HttpNotFound();
            }
            return View(разделы);
        }

        // GET: Разделы/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Разделы/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_раздела,Название_раздела")] Разделы разделы)
        {
            if (ModelState.IsValid)
            {
                db.Разделы.Add(разделы);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(разделы);
        }

        // GET: Разделы/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Разделы разделы = await db.Разделы.FindAsync(id);
            if (разделы == null)
            {
                return HttpNotFound();
            }
            return View(разделы);
        }

        // POST: Разделы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_раздела,Название_раздела")] Разделы разделы)
        {
            if (ModelState.IsValid)
            {
                db.Entry(разделы).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(разделы);
        }

        // GET: Разделы/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Разделы разделы = await db.Разделы.FindAsync(id);
            if (разделы == null)
            {
                return HttpNotFound();
            }
            return View(разделы);
        }

        // POST: Разделы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Разделы разделы = await db.Разделы.FindAsync(id);
            db.Разделы.Remove(разделы);
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
