﻿using System;
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
        // функция добавления ответов к вопросу
        public void ProcessingTypeAnswer(Вопросы вопросы, string func, string variant = "", string def = "", string correct = "")
        {
            try
            {
               
                if (вопросы.Тип_ответа.Replace(" ", "") == "Выбор")
                {
                    string[] stringTempList = variant.Split(',');
                    var resulttemp = stringTempList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    stringTempList = resulttemp;
                    foreach (var t in stringTempList)
                    {
                        Ответы ответы = new Ответы();
                        ответы.Текст_ответа = t;
                        if (t.Replace(" ", "").ToLower() == correct.Replace(" ", "").ToLower())
                        {
                            ответы.Флаг_правильного_ответа = true;
                        }
                        else
                        {
                            ответы.Флаг_правильного_ответа = false;
                        }
                        if (func == "create")
                        {
                            ответы.id_Вопроса = вопросы.id_вопроса;
                            db.Ответы.Add(ответы);
                            db.SaveChanges();
                    }
                        else
                        {
                            вопросы.Ответы.Add(ответы);
                          //  db.SaveChanges();
                        }
                        
                       
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
                        Ответы ответы = new Ответы();
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
                        if (func == "create")
                        {
                            ответы.id_Вопроса = вопросы.id_вопроса;
                            db.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                        else
                        {
                            вопросы.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                       
                    }
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Ввод")
                {
                    Ответы ответы = new Ответы();
                    ответы.Текст_ответа = correct.Replace("  ", "");
                    ответы.Флаг_правильного_ответа = true;
                    if (func == "create")
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        db.Ответы.Add(ответы);
                        db.SaveChanges();
                    }
                    else
                    {
                        вопросы.Ответы.Add(ответы);
                        db.SaveChanges();

                    }
                    
                   
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
                        Ответы ответы = new Ответы();
                        ответы.Текст_ответа = countDef.ToString() + " " + deftemp.Replace("  ", "");
                        ответы.Флаг_правильного_ответа = true;
                        ответы.Флаг_подвопроса = true;
                        ответы.Правильный_ответ = stringTempListCor[countCor];
                        countCor++;
                        countDef++;
                        if (func == "create")
                        {
                            ответы.id_Вопроса = вопросы.id_вопроса;
                            db.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                        else
                        {
                            вопросы.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                        
                       
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
                        Ответы ответы = new Ответы();
                        ответы.Текст_ответа = countV + " " + vartemp.Replace("  ", "");
                        ответы.Флаг_правильного_ответа = true;
                        ответы.Флаг_подвопроса = false;
                        ответы.Правильный_ответ = null;
                        countV++;
                        if (func == "create")
                        {
                            ответы.id_Вопроса = вопросы.id_вопроса;
                            db.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                        else
                        {
                            вопросы.Ответы.Add(ответы);
                            db.SaveChanges();
                        }
                    }
                }
                else if (вопросы.Тип_ответа.Replace(" ", "") == "Разрыв")
                {

                    Ответы ответы = new Ответы();
                    ответы.Текст_ответа = correct.Replace("  ", "");
                    ответы.Флаг_правильного_ответа = true;
                    if (func == "create")
                    {
                        ответы.id_Вопроса = вопросы.id_вопроса;
                        db.Ответы.Add(ответы);
                        db.SaveChanges();
                    }
                    else
                    {
                        вопросы.Ответы.Add(ответы);
                        db.SaveChanges();
                    }

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
                        Ответы ответы1 = new Ответы();
                        ответы1.Текст_ответа = vartemp.Replace("  ", "");
                        ответы1.Флаг_правильного_ответа = false;

                        if (func == "create")
                        {
                            ответы1.id_Вопроса = вопросы.id_вопроса;
                            db.Ответы.Add(ответы1);
                            db.SaveChanges();
                        }
                        else
                        {
                            вопросы.Ответы.Add(ответы1);
                            db.SaveChanges();
                        }
                    }

                }
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

        // GET: Вопросы
        public async Task<ActionResult> Index(string redactor = "", string topic = "")
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
                ViewBag.Раздел = тема.Id_Раздела;
                ViewBag.НазваниеТеста = тема.Название_темы_теста.Replace("  ", "");
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
        public async Task<ActionResult> Index(Тесты item, string redactor, string id_user)
        {
            if (id_user != "")
            {
               int id_user1 = Convert.ToInt32(id_user);
                ViewBag.Id_user = id_user1;
            }
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
            Тесты тема = db.Тесты.Find(userТестId);
            ViewBag.Раздел = тема.Id_Раздела;
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

            ViewBag.Раздел = вопросы.Тесты.Id_Раздела;
            return View(вопросы);
        }
    
        // GET: Вопросы/Create
        public ActionResult Create(int? id)
        {
            Тесты тест = db.Тесты.Find(id); //находим тему, в которой этот вопрос
            ViewBag.Тест = тест.id_теста;
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста");
            ViewBag.Раздел = тест.Id_Раздела;
            
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
                    ProcessingTypeAnswer(вопросы, "create",variant, def, correct);
                    
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
            ViewBag.Раздел = вопросы.Тесты.Id_Раздела;
            int selected = вопросы.id_Теста;
            ViewBag.id_Теста = new SelectList(db.Тесты.Where(a=>a.id_теста==вопросы.id_Теста), "id_теста", "Название_темы_теста", selected);
            ViewBag.ТипОтвета = вопросы.Тип_ответа.Replace(" ", "");
            if ((вопросы.Тип_ответа.Replace(" ", "") == "Ввод") || (вопросы.Тип_ответа.Replace(" ", "") == "Разрыв"))
            {

                var answerCorrect = from v in db.Ответы where v.id_Вопроса == вопросы.id_вопроса select v;
                foreach(var t in answerCorrect)
                {
                    if (t.Флаг_правильного_ответа)
                    {
                        ViewBag.ВерныйОтвет = t.Текст_ответа.Replace("  ", "").TrimStart(' ').TrimEnd(' ');
                        break;
                    }
                }
               

            }
            else if ((вопросы.Тип_ответа.Replace(" ", "") == "Выбор")||(вопросы.Тип_ответа.Replace(" ", "") == "Несколько"))
            {
                string variantStr = "";
                string correctStr = "";
                var answer = from v in db.Ответы where v.id_Вопроса == вопросы.id_вопроса select v;
                foreach (var variant in answer)
                {
                    variantStr = variantStr+ variant.Текст_ответа.Replace("  ", "").TrimStart(' ').TrimEnd(' ') + ", ";
                  
                    if (variant.Флаг_правильного_ответа)
                    {
                        correctStr = correctStr+ variant.Текст_ответа.Replace("  ", "").TrimStart(' ').TrimEnd(' ') + ", ";

                    }
                }

                    variantStr = variantStr.Substring(0, variantStr.Length-2);
                    correctStr =correctStr.Substring(0, correctStr.Length-2); //убираем лишнюю запятую
                
                ViewBag.Варианты = variantStr;
                ViewBag.ВерныйОтвет = correctStr;
            }
            else if (вопросы.Тип_ответа.Replace(" ", "") == "Соотношение")
            {
                string Def = "";
                string CorrectAnsw = "";
                var answerS = from v in db.Ответы where (v.id_Вопроса == вопросы.id_вопроса && v.Флаг_подвопроса==true) select v;
                foreach (var variant in answerS)
                {
                    Def = Def+ variant.Текст_ответа.Replace("  ", "").TrimStart(' ').TrimEnd(' ').Substring(1) + ", ";
                    
                    CorrectAnsw = CorrectAnsw + variant.Правильный_ответ.Replace("  ", "").TrimStart(' ').TrimEnd(' ') + ", ";
                    
                }

                Def = Def.Substring(0, Def.Length - 2);
                CorrectAnsw = CorrectAnsw.Substring(0, CorrectAnsw.Length-2); //убираем лишнюю запятую
                
                ViewBag.Понятия = Def;
                ViewBag.ВерныйОтвет = CorrectAnsw;
            }
                return View(вопросы);
        }

        // POST: Вопросы/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_вопроса,id_Теста,Текст_вопроса,Тип_ответа,Номер_подвопроса")] Вопросы вопросы, string variant, string correct, string def)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Вопросы editВопрос = await (db.Вопросы.FindAsync(вопросы.id_вопроса));
                    editВопрос.Текст_вопроса = вопросы.Текст_вопроса.Replace("  ", "");
                    editВопрос.Тип_ответа = вопросы.Тип_ответа;
                    editВопрос.id_Теста = вопросы.id_Теста;
                    var answerDelete = from a in db.Ответы where a.id_Вопроса == editВопрос.id_вопроса select a;
                    foreach (var aDelete in answerDelete)
                    {
                        db.Ответы.Remove(aDelete);
                    }
                    //     editВопрос.Ответы.Clear();         
                   
                    ProcessingTypeAnswer(editВопрос, "edit", variant, def, correct); //добавляем новые ответы
                   
                    db.Entry(editВопрос).State = EntityState.Modified;
                   
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { topic = editВопрос.id_Теста.ToString(), redactor = "redactor" });
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
            ViewBag.Раздел = вопросы.Тесты.Id_Раздела;
            return View(вопросы);
        }

        // POST: Вопросы/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Вопросы вопросы = await db.Вопросы.FindAsync(id);
            string id_Topic = вопросы.id_Теста.ToString();
            var answDelete = await (from answ in db.Ответы where answ.id_Вопроса == вопросы.id_вопроса select answ).ToListAsync(); //ответы к вопросам
            foreach (var aD in answDelete )
            {
                db.Ответы.Remove(aD); //удаляем ответы к вопросам
            }
            db.Вопросы.Remove(вопросы); //удаляем вопрос
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { topic = id_Topic, redactor = "redactor" });
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
