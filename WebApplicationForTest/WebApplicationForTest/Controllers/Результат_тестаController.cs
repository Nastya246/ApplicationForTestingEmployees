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
    public class Результат_тестаController : Controller
    {
        private TestEntities db = new TestEntities();

        // GET: Результат_теста
        public async Task<ActionResult> Index()
        {
            var результат_теста = db.Результат_теста.Include(р => р.Пользователи).Include(р => р.Тесты);
            return View(await результат_теста.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form ) //обработка результатов по тесту
        {

            var k = form.Keys;
            List<string> ResultQuestion = new List<string>(k.Count); //для хранения результатов верно/неверно
            Dictionary<string, string> resultA = new Dictionary<string, string>(k.Count); //для хранения первичных результатов
            var temp  = form.ToValueProvider(); //получаем все данные
           string kD = "";
            string valD = "";
            int id_Test = 0;
            foreach (var val in k)
            {
               
                kD = val.ToString().Replace(" ","");
                if (resultA.ContainsKey(kD)) //если есть запись в словаре под таким ключом, то удаляем, по идеи ее быть не должно, но мало ли
                {
                    resultA.Remove(kD);
                }
                if (kD == "id_test") //если получили id_теста, сохраняем в отдельной переменной
                {
                    id_Test = Convert.ToInt32(temp.GetValue(kD).AttemptedValue);
                }
                else
                {
                    valD = temp.GetValue(kD).AttemptedValue.Replace(" ", ""); // если получили ответы пользователя, то сохраняем в словарь
                    string[] mystring = valD.Split(','); //обработка результатов из checkbox
                    List<string> ls = new List<string>(mystring.Count());
                    if (mystring.Count()>1)
                    {
                       for (int i=0; i< mystring.Count(); i++)
                            {
                           
                            ls.Add(mystring[i]);               
                            if (mystring[i] == "true")
                            {
                                    i++;
                            }
                        }
                        valD = "";
                        foreach(var l in ls)
                        {
                            valD = valD + l+ " ";
                        }
                    }
                    
                    resultA.Add(kD, valD);
                }
            }
            var AllQuestion = (from question in db.Вопросы where question.id_Теста == id_Test select question).ToList(); //получаем все вопросы из теста
            int nQ = 1; // для перечисления вопросов
            int Answ = 0; //число правильных ответов
            int QuestC = AllQuestion.Count();       //число вопросов тесте
            Dictionary<string, string> resultAnswerUser = new Dictionary<string, string>(QuestC); //для хранения конечных результатов пользователя
            foreach (var q1 in AllQuestion)
            {
                if (!(resultA.ContainsKey(q1.id_вопроса.ToString()))) //если нет записи в словаре под таким ключом, то пользователь не ответил на этот вопрос, фиксируем это
                {
                    resultA.Add(q1.id_вопроса.ToString(), "");
                }
                    resultAnswerUser[q1.id_вопроса.ToString()] = resultA[q1.id_вопроса.ToString()];
            }

            int key = 0;
            
            foreach (var id_q in resultAnswerUser) //проверяем правильно ли ответил пользователь
            {
            key = Convert.ToInt32(id_q.Key);
                var tempSelect= (from answ in db.Ответы where answ.id_Вопроса == key select answ).ToList(); //выбор всех ответов текущего вопроса
                foreach (var answR in tempSelect)
                    {

                    if (id_q.Value != "") //если ответ заполнен/выбран
                    {
                        Вопросы вопросы = await db.Вопросы.FindAsync(Convert.ToInt32(id_q.Key));
                        if (вопросы.Тип_ответа.Replace(" ", "") != "Несколько") //если тип вопроса выбор или соотношение
                        {
                            var TextAnsw = (from textAnsw in tempSelect where textAnsw.Текст_ответа.Replace(" ", "") == id_q.Value select textAnsw).ToList(); //получаем текст ответа
                            if (TextAnsw.Count() != 0) //если нашли текст ответа, то смотрим на его флаг
                            {
                                foreach (var aT in TextAnsw)
                                {
                                    if (aT.Флаг_правильного_ответа)
                                    {
                                        ResultQuestion.Add(nQ.ToString() + " " + "Верно");
                                        Answ++;
                                        nQ++;
                                        break;
                                    }
                                    else
                                    {
                                        ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                        nQ++;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                nQ++;
                                break;
                            }
                        }
                        else //если вопрос имеет несколько правильных ответов
                        {  
                            int CountAns = 0;  
                            int countChec = 0;
                            int idA = Convert.ToInt32(id_q.Key);
                            var TextAnsw = (from textAnsw in db.Ответы where textAnsw.id_Вопроса == idA select textAnsw).ToList();//получаем все варианты для данного вопроса
                            int countAnswerCheckboxList = TextAnsw.Count();
                            string[] mystringCheck = id_q.Value.Split(' ');
                            List<bool> checkboxl = new List<bool>(mystringCheck.Count());
                            foreach (var str in mystringCheck)
                            {
                                if (str == "true")
                                {
                                    checkboxl.Add(true);
                                }
                                if (str == "false")
                                {
                                    checkboxl.Add(false);
                                }
                            }
                            foreach (var check in TextAnsw )
                            {
                               if (check.Флаг_правильного_ответа==checkboxl[countChec])
                                {
                                    CountAns++;
                                }
                                countChec++;
                            }
                            if (CountAns >= countAnswerCheckboxList)
                            {
                                ResultQuestion.Add(nQ.ToString() + " " + "Верно");
                                Answ++;
                                nQ++;
                            }
                            else
                            {
                                ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                nQ++;
                                break;
                            }
                            break;
                        }
                    }
                    else
                    {

                        ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                        nQ++;
                        break;
                    }
                    break;
                }
            }
            
            double resulProcentTheory = ((double)Answ / (double)(QuestC)) * 100.0; //результат теста в процентах
            ViewBag.ResultProcent = resulProcentTheory; //передаем результат в % в представление
            ViewBag.ResultQuestion = ResultQuestion; // передаем в представление список , где указано , что верно, а что нет

            nQ = 0;
            var результат_вопроса = db.Результат_теста.Include(р => р.Пользователи);
            return View(await результат_вопроса.ToListAsync());
        }


        // GET: Результат_теста/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_теста результат_теста = await db.Результат_теста.FindAsync(id);
            if (результат_теста == null)
            {
                return HttpNotFound();
            }
            return View(результат_теста);
        }

        // GET: Результат_теста/Create
        public ActionResult Create()
        {
            ViewBag.id_User = new SelectList(db.Пользователи, "id_user", "Фамилия");
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста");
            return View();
        }

        // POST: Результат_теста/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_результата_теста,Дата_сдачи_теории,Оценка_за_теорию,id_Теста,id_User,Дата_сдачи_практики,Отметка_о_практике,Общий_результат")] Результат_теста результат_теста)
        {
            if (ModelState.IsValid)
            {
                db.Результат_теста.Add(результат_теста);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_User = new SelectList(db.Пользователи, "id_user", "Фамилия", результат_теста.id_User);
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", результат_теста.id_Теста);
            return View(результат_теста);
        }

        // GET: Результат_теста/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_теста результат_теста = await db.Результат_теста.FindAsync(id);
            if (результат_теста == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_User = new SelectList(db.Пользователи, "id_user", "Фамилия", результат_теста.id_User);
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", результат_теста.id_Теста);
            return View(результат_теста);
        }

        // POST: Результат_теста/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_результата_теста,Дата_сдачи_теории,Оценка_за_теорию,id_Теста,id_User,Дата_сдачи_практики,Отметка_о_практике,Общий_результат")] Результат_теста результат_теста)
        {
            if (ModelState.IsValid)
            {
                db.Entry(результат_теста).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_User = new SelectList(db.Пользователи, "id_user", "Фамилия", результат_теста.id_User);
            ViewBag.id_Теста = new SelectList(db.Тесты, "id_теста", "Название_темы_теста", результат_теста.id_Теста);
            return View(результат_теста);
        }

        // GET: Результат_теста/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Результат_теста результат_теста = await db.Результат_теста.FindAsync(id);
            if (результат_теста == null)
            {
                return HttpNotFound();
            }
            return View(результат_теста);
        }

        // POST: Результат_теста/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Результат_теста результат_теста = await db.Результат_теста.FindAsync(id);
            db.Результат_теста.Remove(результат_теста);
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
