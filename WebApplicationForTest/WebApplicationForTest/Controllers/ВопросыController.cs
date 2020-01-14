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
using System.Data.Entity.Validation;
namespace WebApplicationForTest.Controllers
{
    public class ВопросыController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Вопросы
        public async Task<ActionResult> Index(string redactor="", string topic="")
        {
            if (redactor != "")
            {
                ViewBag.User = "redactor";
            }
           
            int id_Topic = 0;
            if (topic != "")
            {
                id_Topic = Convert.ToInt32(topic);
                Тесты тема = db.Тесты.Find(id_Topic);
                ViewBag.НазваниеТеста = тема.Название_темы_теста.Replace("  ","");
                ViewBag.IdТеста = id_Topic;
                var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в => в.Ответы);
                вопросы = (from v in db.Вопросы where v.id_Теста == id_Topic select v).Include(v => v.Ответы);
                int q = 1;
                int count = 0;

                foreach (var temp in вопросы)
                {

                    temp.Текст_вопроса = q.ToString() + " " + temp.Текст_вопроса;
                    if (temp.Тип_ответа.Replace(" ", "") == "Разрыв")
                    {
                        int selectedIndex = 1;
                        SelectList ans = new SelectList(from a in db.Ответы where a.id_Вопроса == temp.id_вопроса && a.Флаг_правильного_ответа == false select a, "id_ответа", "Текст_ответа", selectedIndex); //выпадающий список для вопросов с разрывами
                        ViewData[count.ToString()] = ans;

                        count++;
                        //  temp.Текст_вопроса = temp.Текст_вопроса.Replace("...", "@Html.DropDownList(\"Answ\", ViewBag.Ans as SelectList, new {id=\"ans\" }) ");

                    }
                    q++;

                }

                return View(await вопросы.ToListAsync());

            }
            else
            {
                int q = 1;
                var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в => в.Ответы);
                foreach (var temp in вопросы)
                {
                    temp.Текст_вопроса = q.ToString() + " " + temp.Текст_вопроса;

                    q++;

                }
                return View(await вопросы.ToListAsync());
            }
        }
        [HttpPost] // доступные вопросы по выбранному тесту
        public async Task<ActionResult> Index(Тесты item, string redactor)
        {

            string nameTest = item.Название_темы_теста;
            ViewBag.НазваниеТеста = nameTest; //передаем название теста в представление
           
            int userТестId = 0;
            foreach (var t in db.Тесты) //определяем id выбранного теста
            {
                if (t.Название_темы_теста == nameTest)
                {
                    userТестId = t.id_теста;
                }
            }
            ViewBag.IdТеста = userТестId;
            var вопросы = db.Вопросы.Include(в => в.Тесты).Include(в => в.Ответы);
            вопросы = (from v in db.Вопросы where v.id_Теста == userТестId select v).Include(v => v.Ответы); //выбираем вопросы для кокретного теста по id теста

            if (redactor == "redactor")
            {
                ViewBag.User = "redactor";

            }
           
            int q = 1;
                int count = 0;

                foreach (var temp in вопросы)
                {

                    temp.Текст_вопроса = q.ToString() + " " + temp.Текст_вопроса;
                    if (temp.Тип_ответа.Replace(" ", "") == "Разрыв")
                    {
                        int selectedIndex = 1;
                        SelectList ans = new SelectList(from a in db.Ответы where a.id_Вопроса == temp.id_вопроса && a.Флаг_правильного_ответа == false select a, "id_ответа", "Текст_ответа", selectedIndex); //выпадающий список для вопросов с разрывами
                        ViewData[count.ToString()] = ans;

                        count++;
                        //  temp.Текст_вопроса = temp.Текст_вопроса.Replace("...", "@Html.DropDownList(\"Answ\", ViewBag.Ans as SelectList, new {id=\"ans\" }) ");

                    }
                    q++;

                }

                return View(await вопросы.ToListAsync());
            
        }
        // GET: Вопросы/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            return View(вопросы);
        }

        // GET: Вопросы/Create
        public ActionResult Create(int? id)
        {
           Тесты тест = db.Тесты.Find(id);
            ViewBag.Тест = тест.id_теста;
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста");
            int selectedIndex = 1;
            List<string> typeQ = new List<string>();
            typeQ.Add("Выбор");
            typeQ.Add("Ввод");
            typeQ.Add("Несколько");
            typeQ.Add("Разрыв");
            typeQ.Add("Соотношение");
            SelectList typeAnsw = new SelectList(typeQ, selectedIndex); //выпадающий список для вопросов с разрывами
            ViewBag.ТипВопроса = typeAnsw;
            return View();
        }

        // POST: Вопросы/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_вопроса,id_Теста,Текст_вопроса,Тип_ответа,Номер_подвопроса")] Вопросы вопросы, int id_Теста, string variant, string correct, string def)
        {
            
                вопросы.id_Теста = id_Теста;
            
            if (ModelState.IsValid)
            {
                try { 
                Ответы ответы = new Ответы();
                db.Вопросы.Add(вопросы);

                await db.SaveChangesAsync();

                if (вопросы.Тип_ответа.Replace(" ", "") == "Выбор")
                {
                    string[] stringTempList = variant.Split(',');
                    var resulttemp = stringTempList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempList = resulttemp;
                    foreach (var t in stringTempList)
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        ответы.Текст_ответа = t;
                        if (t.Replace(" ", "").ToLower() == correct.Replace(" ", "").ToLower())
                        {
                            ответы.Флаг_правильного_ответа = true;
                        }
                        else
                        {
                            ответы.Флаг_правильного_ответа = false;
                        }
                        db.Ответы.Add(ответы);
                        await db.SaveChangesAsync();
                    }
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Несколько")
                {
                    string[] stringTempListVar = variant.Split(','); //разделяем варианты ответов
                    var resulttemp = stringTempListVar.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempListVar = resulttemp;

                    string[] stringTempListCor = correct.Split(',');
                    resulttemp = stringTempListCor.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempListCor = resulttemp;
                    int flag = 0;
                    foreach (var t in stringTempListVar)
                    {
                        flag = 0;
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        ответы.Текст_ответа = t;
                        foreach (var t2 in stringTempListCor)
                        {
                            if (t.Replace(" ", "").ToLower() == t2.Replace(" ", "").ToLower())
                            {
                                flag = 1;
                                break;
                            }

                        }
                        if (flag == 1)
                        {
                            ответы.Флаг_правильного_ответа = true;
                        }
                        else
                        {
                            ответы.Флаг_правильного_ответа = false;
                        }
                        db.Ответы.Add(ответы);
                        await db.SaveChangesAsync();
                    }
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Ввод")
                {
                    ответы.id_Вопроса = вопросы.id_вопроса;
                    ответы.Текст_ответа = correct;
                    ответы.Флаг_правильного_ответа = true;
                    db.Ответы.Add(ответы);
                    await db.SaveChangesAsync();
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Соотношение")
                {

                    string[] stringTempListCor = correct.Split(',');//разделяем корректные варианты ответов
                    var resulttemp = stringTempListCor.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempListCor = resulttemp;

                    string[] stringTempListDef = def.Split(',');//разделяем понятия
                    resulttemp = stringTempListDef.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempListDef = resulttemp;
                    int countDef = 1;
                    int countCor = 0;


                    foreach (var deftemp in stringTempListDef) //вносим понятия в бд
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        ответы.Текст_ответа = countDef.ToString() + " " + deftemp;
                        ответы.Флаг_правильного_ответа = true;
                        ответы.Флаг_подвопроса = true;
                        ответы.Правильный_ответ = stringTempListCor[countCor];
                        countCor++;
                        countDef++;
                        db.Ответы.Add(ответы);
                        await db.SaveChangesAsync();
                    }

                    string[] stringTempListVar = stringTempListCor; //для вариантов ответов

                    Random rand = new Random();
                    for (int i = stringTempListVar.Length - 1; i >= 1; i--) //генерируем варианты ответов в случайном порядке
                    {
                        int j = rand.Next(i + 1);
                        string tmp = stringTempListVar[j];
                        stringTempListVar[j] = stringTempListVar[i];
                        stringTempListVar[i] = tmp;
                    }
                    char countV = 'a';
                    foreach (var vartemp in stringTempListVar) //вносим варианты ответов в бд
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        ответы.Текст_ответа = countV + " " + vartemp;
                        ответы.Флаг_правильного_ответа = true;
                        ответы.Флаг_подвопроса = false;
                            ответы.Правильный_ответ = null;
                        countV++;
                        db.Ответы.Add(ответы);
                        await db.SaveChangesAsync();
                    }
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Разрыв")
                {

                    ответы.id_Вопроса = вопросы.id_вопроса;
                    ответы.Текст_ответа = correct;
                    ответы.Флаг_правильного_ответа = true;
                    db.Ответы.Add(ответы);
                    await db.SaveChangesAsync();

                    string[] stringTempListVar = correct.Split(',');//подготавливаем для составления списка с вариантами ответов
                    var resulttemp = stringTempListVar.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempListVar = resulttemp;
                    Random rand = new Random();
                    for (int i = stringTempListVar.Length - 1; i >= 1; i--) //генерируем варианты ответов в случайном порядке
                    {
                        int j = rand.Next(i + 1);
                        string tmp = stringTempListVar[j];
                        stringTempListVar[j] = stringTempListVar[i];
                        stringTempListVar[i] = tmp;
                    }

                    foreach (var vartemp in stringTempListVar) //вносим варианты ответов в БД
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        ответы.Текст_ответа = vartemp;
                        ответы.Флаг_правильного_ответа = false;

                        db.Ответы.Add(ответы);
                        await db.SaveChangesAsync();
                    }

                }
                return RedirectToAction("Index", new { topic = вопросы.id_Теста.ToString(), redactor = "redactor" });
            }
            catch (DbEntityValidationException ex)

                {
                    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                    {
                        Response.Write("Object: " + validationError.Entry.Entity.ToString());
                        Response.Write(" ");
                             foreach (DbValidationError err in validationError.ValidationErrors)
                        {
                            Response.Write(err.ErrorMessage + " ");
                        }
                    }
                }

                }
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", вопросы.id_Теста);
                return View(вопросы);
           
        }

        // GET: Вопросы/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", вопросы.id_Теста);
            return View(вопросы);
        }

        // POST: Вопросы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_вопроса,id_Теста,Текст_вопроса,Тип_ответа,Номер_подвопроса")] Вопросы вопросы)
        {
            if (ModelState.IsValid)
            {
                db.Entry(вопросы).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", вопросы.id_Теста);
            return View(вопросы);
        }

        // GET: Вопросы/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            if (вопросы == null)
            {
                return HttpNotFound();
            }
            return View(вопросы);
        }

        // POST: Вопросы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            db.Вопросы.Remove(вопросы);
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
