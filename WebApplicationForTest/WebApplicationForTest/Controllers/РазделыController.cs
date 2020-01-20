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
        public async Task<ActionResult> Index(string redactor = "")
        {
           
                ViewBag.r = "redactor";
            
            var  разделы = db.Разделы.Include(т => т.Тесты);
            return View(await разделы.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult> Index(int? id_user, string Login, string Password )
        {
            if (id_user == null)
            {
                foreach (var temp in db.Пользователи)
                {
                    if ((temp.Логин.Replace(" ", "") == Login)&&(temp.Пароль.Replace(" ", "") == Password))
                            {
                        id_user = temp.id_user;
                    }
                }
            }
            if (Login == "redactor")
            {
                ViewBag.Id_user = Login;
                ViewBag.r = "redactor";
            }
            else
            {
                ViewBag.Id_user = id_user;
            }
            var разделы = db.Разделы.Include(т => т.Тесты);
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
                разделы.Название_раздела = разделы.Название_раздела.TrimEnd(' ');
                var temp=from v in db.Разделы where v.Название_раздела.Replace(" ","").ToLower() == разделы.Название_раздела.Replace(" ","").ToLower() select v;
                if (temp.Count() == 0) //проверяем, есть ли раздел с таки же именем, если нет, то добавляем
                {
                   
                    db.Разделы.Add(разделы);
                    await db.SaveChangesAsync();
                }
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
                разделы.Название_раздела = разделы.Название_раздела.TrimEnd(' ');
                var temp = from v in db.Разделы where v.Название_раздела.Replace(" ", "").ToLower() == разделы.Название_раздела.Replace(" ", "").ToLower() select v;
                if (temp.Count() == 0) //проверяем, есть ли раздел с таки же именем, если нет, то добавляем
                {

                    db.Entry(разделы).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                   
                }
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
            Разделы разделы = await db.Разделы.FindAsync(id); //получеам раздел для удаления по Id
            var тесты =  await(from t in db.Тесты where t.Id_Раздела == id select t).ToListAsync(); // получаем тесты из этого раздела
            List<Вопросы> вопросы = new List<Вопросы>(); //для хранения вопросов из тестов раздела
            List<Ответы> ответы = new List<Ответы>(); //для хранения ответов к вопросам из раздела
            foreach (var q in тесты )
            {
                var Q= await (from q1 in db.Вопросы where q1.id_Теста == q.id_теста select q1).ToListAsync(); //получаем все вопросы из всех тестов данного раздела
                foreach (var qAdd in Q)
                { 
                    вопросы.Add(qAdd); //заносим эти вопросы в список
                }
            }
            foreach (var a in вопросы)
            {
                var A = await (from a1 in db.Ответы where a1.id_Вопроса == a.id_вопроса select a1).ToListAsync(); //получаем все ответы для вопросов  всех тестов данного раздела
                foreach (var aAdd in A)
                {
                    ответы.Add(aAdd); //заносим эти ответы в список
                }
            }
            foreach (var aD in ответы) //удаляем  ответы из раздела
            {
                db.Ответы.Remove(aD);
            }
            foreach (var qD in вопросы) //удаляем  вопросы из раздела
            {
                db.Вопросы.Remove(qD);
            }
            foreach (var tD in тесты) //удаляем удаляем темы из раздела
            {
                db.Тесты.Remove(tD);
            }
            db.Разделы.Remove(разделы); //удаляем раздел
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
