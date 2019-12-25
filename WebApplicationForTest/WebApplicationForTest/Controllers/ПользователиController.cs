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
    public class ПользователиController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Пользователи
        public async Task<ActionResult> Index()
        {
            var пользователи = db.Пользователи.Include(п => п.Должность).Include(п => п.Подразделение);
            return View(await пользователи.ToListAsync());
        }

        // GET: Пользователи/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Пользователи пользователи = await db.Пользователи.FindAsync(id);
            if (пользователи == null)
            {
                return HttpNotFound();
            }
            return View(пользователи);
        }

        // GET: Пользователи/Create
        public ActionResult Create()
        {
            ViewBag.id_должности = new SelectList(db.Должность, "Id_должности", "Название_должности");
            ViewBag.id_подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения");
            return View();
        }

        // POST: Пользователи/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_user,Фамилия,Имя,Отчество,Логин,Пароль,id_подразделения,id_должности")] Пользователи пользователи)
        {
            if (ModelState.IsValid)
            {
                db.Пользователи.Add(пользователи);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_должности = new SelectList(db.Должность, "Id_должности", "Название_должности", пользователи.id_должности);
            ViewBag.id_подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", пользователи.id_подразделения);
            return View(пользователи);
        }

        // GET: Пользователи/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Пользователи пользователи = await db.Пользователи.FindAsync(id);
            if (пользователи == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_должности = new SelectList(db.Должность, "Id_должности", "Название_должности", пользователи.id_должности);
            ViewBag.id_подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", пользователи.id_подразделения);
            return View(пользователи);
        }

        // POST: Пользователи/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_user,Фамилия,Имя,Отчество,Логин,Пароль,id_подразделения,id_должности")] Пользователи пользователи)
        {
            if (ModelState.IsValid)
            {
                db.Entry(пользователи).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_должности = new SelectList(db.Должность, "Id_должности", "Название_должности", пользователи.id_должности);
            ViewBag.id_подразделения = new SelectList(db.Подразделение, "Id_подразделения", "Название_подразделения", пользователи.id_подразделения);
            return View(пользователи);
        }

        // GET: Пользователи/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Пользователи пользователи = await db.Пользователи.FindAsync(id);
            if (пользователи == null)
            {
                return HttpNotFound();
            }
            return View(пользователи);
        }

        // POST: Пользователи/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Пользователи пользователи = await db.Пользователи.FindAsync(id);
            db.Пользователи.Remove(пользователи);
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
