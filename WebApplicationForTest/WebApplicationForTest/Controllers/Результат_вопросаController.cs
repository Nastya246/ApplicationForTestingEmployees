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
    public class Результат_вопросаController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Результат_вопроса
        public async Task<ActionResult> Index()
        {
            var результат_вопроса = db.Результат_вопроса.Include(р => р.Вопросы);
            return View(await результат_вопроса.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult> Index( FormCollection form) //обработка результатов по тесту
        {
          
            var k = form.Keys;
            List<string> ResultQuestion = new List<string>(k.Count); //для хранения результатов верно/неверно
           
           
            int nQ = 1; // для перечисления вопросов
            int Answ = 0; //число правильных ответов
            Dictionary<int, string> resultAnswer = new Dictionary<int, string>(k.Count); //для значений, которые отметил пользователь
            foreach (var r in k)
            {
               var t= form.GetValue(r.ToString());
                resultAnswer.Add(Convert.ToInt32(r), t.AttemptedValue); //заполняем словарь значениями, которые соответствуют вопросу и ответу
               // result += t.AttemptedValue +" ";
            }
            foreach (var id_q in resultAnswer) //проверяем правильно ли ответил пользователь
            {
                foreach (var id_o in db.Ответы)
                {
                    if (id_q.Key == id_o.id_Вопроса) 
                    {
                        if (id_q.Value==id_o.Текст_ответа)
                        {
                            if (id_o.Флаг_правильного_ответа == true)
                            {
                                ResultQuestion.Add(nQ.ToString()+" "+"Верно");
                                Answ++;
                            }
                            else
                            {
                                ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                            }
                           
                            nQ++;
                        }
                    }
                }            
                
            }
            double resulProcentTheory = ((double)Answ / (double)(k.Count))*100.0; //результат теста в процентах
            ViewBag.ResultProcent = resulProcentTheory; //передаем результат в % в представление
            ViewBag.ResultQuestion = ResultQuestion; // передаем в представление список , где указано , что верно, а что нет
            
                nQ = 0;
            var результат_вопроса = db.Результат_вопроса.Include(р => р.Вопросы);
            return View(await результат_вопроса.ToListAsync());
        }
        // GET: Результат_вопроса/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_вопроса результат_вопроса = await db.Результат_вопроса.FindAsync(id);
            if (результат_вопроса == null)
            {
                return HttpNotFound();
            }
            return View(результат_вопроса);
        }

        // GET: Результат_вопроса/Create
        public ActionResult Create()
        {
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса");
            return View();
        }

        // POST: Результат_вопроса/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_результата,id_Вопроса,Номер_ответа,Номер_вопроса,Правильность_ответа")] Результат_вопроса результат_вопроса)
        {
            if (ModelState.IsValid)
            {
                db.Результат_вопроса.Add(результат_вопроса);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", результат_вопроса.id_Вопроса);
            return View(результат_вопроса);
        }

        // GET: Результат_вопроса/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_вопроса результат_вопроса = await db.Результат_вопроса.FindAsync(id);
            if (результат_вопроса == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", результат_вопроса.id_Вопроса);
            return View(результат_вопроса);
        }

        // POST: Результат_вопроса/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_результата,id_Вопроса,Номер_ответа,Номер_вопроса,Правильность_ответа")] Результат_вопроса результат_вопроса)
        {
            if (ModelState.IsValid)
            {
                db.Entry(результат_вопроса).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Вопроса = new SelectList(db.Вопросы, "id_вопроса", "Текст_вопроса", результат_вопроса.id_Вопроса);
            return View(результат_вопроса);
        }

        // GET: Результат_вопроса/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_вопроса результат_вопроса = await db.Результат_вопроса.FindAsync(id);
            if (результат_вопроса == null)
            {
                return HttpNotFound();
            }
            return View(результат_вопроса);
        }

        // POST: Результат_вопроса/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Результат_вопроса результат_вопроса = await db.Результат_вопроса.FindAsync(id);
            db.Результат_вопроса.Remove(результат_вопроса);
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
