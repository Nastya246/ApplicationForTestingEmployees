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
    public class ПодразделениеController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Подразделение
        public async Task<ActionResult> Index()
        {
            var подразделения = db.Подразделение.Include(p => p.Должность);
            return View(await подразделения.ToListAsync());
        }

        // GET: Подразделение/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Подразделение подразделение = await db.Подразделение.FindAsync(id);
            if (подразделение == null)
            {
                return HttpNotFound();
            }
            return View(подразделение);
        }

        // GET: Подразделение/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Подразделение/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id_подразделения,Название_подразделения")] Подразделение подразделение)
        {
            if (ModelState.IsValid)
            {
                if ((db.Подразделение.FindAsync(подразделение.Название_подразделения)) == null)
                {
                    db.Подразделение.Add(подразделение);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else { return RedirectToAction("Index"); }
            }

            return View(подразделение);
        }

        // GET: Подразделение/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Подразделение подразделение = await db.Подразделение.FindAsync(id);
            if (подразделение == null)
            {
                return HttpNotFound();
            }
            ViewBag.Position = db.Должность.ToList();
            return View(подразделение);
        }

        // POST: Подразделение/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id_подразделения,Название_подразделения")] Подразделение подразделение, int[] selectedPosition)
        {
            
                Подразделение Newподразделение = await db.Подразделение.FindAsync(подразделение.Id_подразделения);
            if ((db.Подразделение.FindAsync(подразделение.Название_подразделения)) == null)
            {
                Newподразделение.Название_подразделения = подразделение.Название_подразделения;
            }
                Newподразделение.Должность.Clear();
                if (selectedPosition != null)
                {
                    foreach (var c in db.Должность.Where(co => selectedPosition.Contains(co.Id_должности)))
                    { Newподразделение.Должность.Add(c); }
                }
                db.Entry(Newподразделение).State = EntityState.Modified;
                await db.SaveChangesAsync();
            
            return RedirectToAction("Index");
            /*  if (ModelState.IsValid)
              {
                  db.Entry(подразделение).State = EntityState.Modified;
                  await db.SaveChangesAsync();
                  return RedirectToAction("Index");
              }*/
          //  return View(подразделение);
        }

        // GET: Подразделение/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Подразделение подразделение = await db.Подразделение.FindAsync(id);
            if (подразделение == null)
            {
                return HttpNotFound();
            }
            return View(подразделение);
        }

        // POST: Подразделение/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Подразделение подразделение = await db.Подразделение.FindAsync(id);
            db.Подразделение.Remove(подразделение);
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
