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
        public async Task<ActionResult> Index(string otchet="")
        {
            if (otchet!="")
            {
                ViewBag.User = "otchet";
            }
          
    
            var подразделения = await db.Подразделение.Include(p => p.ДолжностьПодразделение).ToListAsync();
            ViewBag.Positions = await db.Должность.ToListAsync();
            return View(подразделения);
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
                var unit = await (from u in db.Подразделение where u.Название_подразделения.Replace(" ", "").ToLower() == подразделение.Название_подразделения.Replace(" ", "").ToLower() select u).ToListAsync();
                if (unit.Count()==0)
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
           // var должностьПодразделение = await (from d in db.ДолжностьПодразделение where d.id_подразделения == id select d).ToListAsync();
            if (подразделение == null)
            {
                return HttpNotFound();
            }
            //   ViewBag.Position = db.Должность.ToList();
            ViewBag.Position = db.Должность.ToList();
            ViewBag.PositionUnits = await (from c in db.ДолжностьПодразделение where  c.id_подразделения==подразделение.Id_подразделения   select c).ToListAsync();
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
         
            var unit = await (from u in db.Подразделение where u.Название_подразделения.Replace(" ", "").ToLower() == подразделение.Название_подразделения.Replace(" ", "").ToLower() select u).ToListAsync();
            if (unit.Count() == 0)
            {
                Newподразделение.Название_подразделения = подразделение.Название_подразделения;
            }
                Newподразделение.ДолжностьПодразделение.Clear();
            db.Entry(Newподразделение).State = EntityState.Modified;
            await db.SaveChangesAsync();
            if (selectedPosition != null)
                {
                    foreach (var c in db.Должность.Where(co => selectedPosition.Contains(co.Id_должности)))
                    {
                    ДолжностьПодразделение должностьПодразделение = new ДолжностьПодразделение();
                    должностьПодразделение.id_подразделения = подразделение.Id_подразделения;
                    должностьПодразделение.id_должности=c.Id_должности;
                    db.ДолжностьПодразделение.Add(должностьПодразделение);
                  
                }
                }
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
            подразделение.ДолжностьПодразделение.Clear();
         //   var relations=await (from r in db.)
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
