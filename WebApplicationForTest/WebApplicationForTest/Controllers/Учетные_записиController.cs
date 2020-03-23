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
    public class Учетные_записиController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Учетные_записи
        public async Task<ActionResult> Index()
        {
            return View(await db.Учетные_записи.ToListAsync());
        }

       

        // GET: Учетные_записи/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Учетные_записи/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Имя_учетной_записи,Пароль")] Учетные_записи учетные_записи)
        {
            if (ModelState.IsValid)
            {
                db.Учетные_записи.Add(учетные_записи);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(учетные_записи);
        }

        // GET: Учетные_записи/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Учетные_записи учетные_записи = await db.Учетные_записи.FindAsync(id);
           
            if (учетные_записи == null)
            {
                return HttpNotFound();
            }
            учетные_записи.Пароль = учетные_записи.Пароль.TrimEnd();
            return View(учетные_записи);
        }

        // POST: Учетные_записи/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Имя_учетной_записи,Пароль")] Учетные_записи учетные_записи)
        {
            if (ModelState.IsValid)
            {

                db.Entry(учетные_записи).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(учетные_записи);
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
