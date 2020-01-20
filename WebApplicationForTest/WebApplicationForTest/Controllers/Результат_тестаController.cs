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
            List<string> QuestionAnswer = new List<string>(k.Count); //для хранения вопросов и ответов
            Dictionary<string, string> resultA = new Dictionary<string, string>(k.Count); //для хранения первичных результатов
            var temp  = form.ToValueProvider(); //получаем все данные
           string kD = ""; // для ключа словаря
            string valD = ""; //для значения словаря
            int id_Test = 0; // для id теста
            int flagS = 0; // для количества вопросов в соотношений
            int tempIdQ = 0; //для получения id вопроса типа "соотношение"
            string tempStrQ = "";
            int id_User = 0;
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
                if (kD == "id_user") //если получили id_пользователя, сохраняем в отдельной переменной
                {
                    id_User = Convert.ToInt32(temp.GetValue(kD).AttemptedValue);
                    ViewBag.Id_user = id_User;
                }
                /*  else if (kD == "id_Q") //если получили id_вопроса типа "соотношение", сохраняем в отдельной переменной
                      {
                      tempStrQ= temp.GetValue(kD).AttemptedValue.Replace(" ", "");
                      string[] mystringTemp = tempStrQ.Split(',');
                        tempIdQ = Convert.ToInt32(mystringTemp[0]);
                      flagS = (from que in db.Ответы where que.id_Вопроса == tempIdQ select que).Count();
                      flagS = flagS / 2; // количество подвопросов в соотношении
                  }
                  */
                else
                {
                    valD = temp.GetValue(kD).AttemptedValue.Replace(" ", ""); // если получили ответы пользователя, то сохраняем в словарь
                    string[] mystringSootn = kD.Split('-');

                    if (mystringSootn.Count()==2) // пришел ответ типа соотношение
                    {
                        kD = "О " + mystringSootn[0]; // помечаем ответы из соотношения буквой "О" перед ключом 
                      
                    }

                    string[] mystring = valD.Split(','); //обработка результатов из checkbox
                    List<string> ls = new List<string>(mystring.Count());
                    if (mystring.Count()>1) //значит пришел результат со множетсвенным выбором, обрабатываем его
                    {
                       for (int i=0; i< mystring.Count(); i++)
                            {
                           
                            ls.Add(mystring[i]);               
                            if (mystring[i] == "true")
                            {
                                    i++; //если пользователь отметил checkbox, то придет два значения - true и false, false пропускаем
                            }
                        }
                        valD = "";
                        foreach(var l in ls)
                        {
                            valD = valD + l+ " ";
                        }
                    }
                 /*   if (flagS>0)
                    {
                        kD = "О "+kD; // помечаем ответы из соотношения буквой "О" перед ключом 
                        flagS--;
                    } */
                    kD = kD.Replace("/", "");
                    resultA.Add(kD, valD);
                    
                }
            }
            string textQ = "";
            string textA = "";
            var AllQuestion = (from question in db.Вопросы where question.id_Теста == id_Test select question).ToList(); //получаем все вопросы из теста
           
            int nQ = 1; // для перечисления вопросов
            int Answ = 0; //число правильных ответов
            int QuestC = AllQuestion.Count();       //число вопросов тесте
            
            Dictionary<string, string> resultAnswerUser = new Dictionary<string, string>(QuestC); //для хранения конечных результатов пользователя
            string tempAnsw = "";
            foreach (var qList in AllQuestion) //получаем вопросы и верные к ним ответы
            {
                textQ = qList.Текст_вопроса.Replace("  ", "");

                if ((qList.Тип_ответа.Replace(" ", "") == "Выбор") || (qList.Тип_ответа.Replace(" ", "") == "Ввод"))
                {
                    textA = "";
                    foreach (var aList in qList.Ответы)
                    {

                        if (aList.Флаг_правильного_ответа == true)
                        {
                            textA = aList.Текст_ответа.Replace("  ", "");
                            QuestionAnswer.Add("Вопрос: " + textQ + " Правильный ответ: " + textA);
                            break;
                        }
                    }
                }
                else if (qList.Тип_ответа.Replace(" ", "") == "Несколько")
                {
                    textA = "";
                    foreach (var aList in qList.Ответы)
                    {
                        if (aList.Флаг_правильного_ответа == true)
                        {
                            textA = textA + aList.Текст_ответа.Replace("  ", "") + " ";


                        }
                    }
                    QuestionAnswer.Add("Вопрос: " + textQ + " Правильный ответ: " + textA + " ");
                }
                else if (qList.Тип_ответа.Replace(" ", "") == "Соотношение")
                {
                    List<Ответы> TempListA = new List<Ответы>();
                    List<string> TempListA2 = new List<string>();
                    textA = "";
                    var tempSelectA = (from answ in db.Ответы where answ.id_Вопроса == qList.id_вопроса select answ).ToList();
                    foreach (var tempL in tempSelectA)
                    {
                        if (tempL.Флаг_подвопроса == true)
                        {
                            TempListA.Add(tempL); //получили подвопросы
                        }
                        if (tempL.Флаг_подвопроса == false)
                        {
                            TempListA2.Add(tempL.Текст_ответа); //получили варианты ответов
                        }
                    }
                    foreach (var listA in TempListA)
                    {
                        foreach (var listA2 in TempListA2) //смотрим, что под буквой
                        {
                            string[] stringTempList = listA2.Split(' ');
                            var resulttemp = stringTempList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            stringTempList = resulttemp;
                            if ((listA.Правильный_ответ.Replace(" ", "") == (stringTempList[1]).Replace(" ", ""))) // если вариант под буквой совпал с правильным вариантом ответа подвопроса, то записываем эту букву
                            {
                                textA = textA + " " + listA.Текст_ответа[0] + "-" + stringTempList[0] + " ";
                            }
                        }
                    }

                    QuestionAnswer.Add("Вопрос: " + textQ + " Правильный ответ: " + textA);
                }
                else if (qList.Тип_ответа.Replace(" ", "") == "Разрыв")
                {
                    var tempSelectR = (from answ in db.Ответы where answ.id_Вопроса == qList.id_вопроса && answ.Флаг_правильного_ответа==true select answ).ToList();
                    QuestionAnswer.Add("Вопрос: " + textQ + " Правильный ответ: " + tempSelectR[0].Текст_ответа.Replace("  ",""));
                }
            }
            foreach (var q1 in AllQuestion)
            {
              
                if ((!(resultA.ContainsKey(q1.id_вопроса.ToString()))) && (q1.Тип_ответа.Replace(" ", "") != "Соотношение")) //если нет записи в словаре под таким ключом, то пользователь не ответил на этот вопрос, фиксируем это
                {
                    resultA.Add(q1.id_вопроса.ToString(), "");
                    resultAnswerUser[q1.id_вопроса.ToString()] = resultA[q1.id_вопроса.ToString()];
                }
                else if (q1.Тип_ответа.Replace(" ", "") == "Соотношение") //ищем вопрос на соотношение в словаре
                {
                    tempAnsw = "";
                    foreach (var sootnosh in resultA)
                    {
                      
                        if ((sootnosh.Key)[0] == 'О')
                        {
                            int id_Ans = Convert.ToInt32(sootnosh.Key.Replace("О ", ""));
                            Ответы ответы = db.Ответы.Find(id_Ans);
                            if (ответы.id_Вопроса == q1.id_вопроса) //такой вопрос на соотношение в словаре есть
                            {
                                tempAnsw = tempAnsw+sootnosh.Key.Replace("О ", "") + " " + sootnosh.Value + ",";
                            }
                        }
                    }
                    if (resultA.ContainsKey(q1.id_вопроса.ToString())) //если есть запись в словаре под таким ключом, то удаляем, по идеи ее быть не должно, но мало ли
                    {
                        resultA.Remove(q1.id_вопроса.ToString());
                    }
                    resultA.Add(q1.id_вопроса.ToString(), tempAnsw);
                    resultAnswerUser[q1.id_вопроса.ToString()] = resultA[q1.id_вопроса.ToString()];
                }
                else
                {
                    resultAnswerUser[q1.id_вопроса.ToString()] = resultA[q1.id_вопроса.ToString()];
                }
            }

            int key = 0;
            int iTemp = 0;
            foreach (var id_q in resultAnswerUser) //проверяем правильно ли ответил пользователь
            {
                key = Convert.ToInt32(id_q.Key);
                var tempSelect= (from answ in db.Ответы where answ.id_Вопроса == key select answ).ToList(); //выбор всех ответов текущего вопроса
                foreach (var answR in tempSelect)
                    {

                    if (id_q.Value != "") //если ответ заполнен/выбран
                    {
                        Вопросы вопросы = await db.Вопросы.FindAsync(Convert.ToInt32(id_q.Key));
                        if ((вопросы.Тип_ответа.Replace(" ", "") == "Выбор")||(вопросы.Тип_ответа.Replace(" ", "") == "Ввод")) //если тип вопроса выбор или ввод
                        {
                            var TextAnsw = (from textAnsw in tempSelect where textAnsw.Текст_ответа.Replace(" ", "") == id_q.Value select textAnsw).ToList(); //получаем текст ответа
                            if (TextAnsw.Count() != 0) //если нашли текст ответа, то смотрим на его флаг
                            {
                                foreach (var aT in TextAnsw)
                                {
                                    if (aT.Флаг_правильного_ответа)
                                    {
                                        // ResultQuestion.Add(nQ.ToString() + " " + "Верно");
                                        QuestionAnswer[iTemp] = nQ.ToString() + " "+ QuestionAnswer[iTemp] + "\nВаш ответ: " + id_q.Value + " - " + "ВЕРНО";
                                        QuestionAnswer[iTemp].Replace("\n", "<br />");
                                        Answ++;
                                        nQ++;            
                                        iTemp++;
                                        break;
                                    }
                                    else
                                    {
                                        QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + id_q.Value + " - " + "НЕВЕРНО";
                                        QuestionAnswer[iTemp].Replace("\n", "<br />");
                                        iTemp++;
                                        //  ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                        nQ++;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                
                                    QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + id_q.Value + " - " + "НЕВЕРНО";
                                iTemp++;
                                // ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                nQ++;
                                break;
                            }
                        }
                        else if (вопросы.Тип_ответа.Replace(" ", "") == "Несколько") //если вопрос имеет несколько правильных ответов
                        {  
                            int CountAns = 0;  
                            int countChec = 0;
                            int idA = Convert.ToInt32(id_q.Key);
                            var TextAnsw = (from textAnsw in db.Ответы where textAnsw.id_Вопроса == idA select textAnsw).ToList();//получаем все варианты для данного вопроса
                            int countAnswerCheckboxList = TextAnsw.Count();
                            string[] mystringCheck = id_q.Value.Split(' ');
                            List<bool> checkboxl = new List<bool>(mystringCheck.Count());
                            string VariatUser = "";
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
                               if (checkboxl[countChec])
                                {
                                    VariatUser = VariatUser + check.Текст_ответа.Replace("  ", "") + " ";
                                }
                                countChec++;
                            }
                            if (CountAns >= countAnswerCheckboxList)
                            {
                              //  ResultQuestion.Add(nQ.ToString() + " " + "Верно");
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + VariatUser + " - " + "ВЕРНО";
                                iTemp++;
                                Answ++;
                                nQ++;
                            }
                            else
                            {
                              //  ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + VariatUser + " - " + "НЕВЕРНО";
                                iTemp++;
                                nQ++;
                                break;
                            }
                            break;
                        }

                        else if (вопросы.Тип_ответа.Replace(" ", "") == "Соотношение")
                        {
                            string VariatUserS = "";
                            string[] mystringTemp = id_q.Value.Split(',');
                            var result = mystringTemp.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(); //убираем нулевые и пустые элементы
                            int flagCorrect = 0;
                            string[] tempStrForSoothosh =new string[2]; //для хранения id_ответа и буквы, которую ввел пользователь
                            string AnswUser = "";
                            string strResult = "";
                            int strC = 1;
                          //  strResult = string.Join(",", result);
                            foreach (var str in result)
                            {
                                tempStrForSoothosh = str.Split(' ');
                                if (tempStrForSoothosh[1] != "")
                                {
                                    var tempStr = tempStrForSoothosh[1].ToCharArray();
                                    strResult = strResult+ strC.ToString() + "-" + tempStrForSoothosh[1] + " ";
                                    foreach (var valueSootnosh in tempSelect) //ищем значение под выбранной буквой
                                    {

                                        if ((valueSootnosh.Флаг_подвопроса == false) && (valueSootnosh.Текст_ответа[0] == tempStr[0]))
                                        {
                                           
                                            if ((tempStrForSoothosh[0] != "") && ((tempStrForSoothosh[1] != "")))
                                            {
                                                AnswUser = valueSootnosh.Текст_ответа.Replace(tempStrForSoothosh[1] + " ", ""); //записываем вариант под буквой 
                                                Ответы ответы = await db.Ответы.FindAsync(Convert.ToInt32(tempStrForSoothosh[0]));
                                                if (AnswUser.Replace(" ", "") == ответы.Правильный_ответ.Replace(" ", "")) //сраниваем вариант ответа подвопроса и того значения, что под буквой
                                                {
                                                    flagCorrect++;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    strResult = strResult + strC.ToString() + "-" + "  ";
                                }
                                strC++;
                            }

                            if (flagCorrect==result.Count())
                            {
                                
                               // ResultQuestion.Add(nQ.ToString() + " " + "Верно");
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + strResult + " - " + "ВЕРНО";
                                iTemp++;
                                Answ++;
                                nQ++;

                            }
                            else
                            {
                                // ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + strResult + " - " + "НЕВЕРНО";
                                iTemp++;
                                nQ++;
                                break;
                            }
                            break;
                        }
                        else if (вопросы.Тип_ответа.Replace(" ", "") == "Разрыв")
                        {
                            string VariatUserR = "";
                            string[] mystringTempR= id_q.Value.Split(' '); // ответы пользователя в виде id ответов
                            var resultR = mystringTempR.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(); //убираем нулевые и пустые элементы

                            var AnswRstring = (from textAnsw in tempSelect where textAnsw.id_Вопроса == Convert.ToInt32(id_q.Key) && textAnsw.Флаг_правильного_ответа == true select textAnsw).ToList();//правильные ответы в виде текста
                            string[] stringAnswTempR = AnswRstring[0].Текст_ответа.Split(',');
                            var AnswR = stringAnswTempR.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            int flagA = 0;
                            string strR = "";
                            List<string> templsR = new List<string>(AnswR.Count());
                            var TextQ = QuestionAnswer[iTemp].Replace("  ", "").Split(new string[] { "..." }, StringSplitOptions.RemoveEmptyEntries);
                            if (AnswR.Count() == resultR.Count()) // если пользователь вставил ответы во все разрывы
                            {
                                for (int i = 0; i < AnswR.Count(); i++)
                                {
                                    var answ = db.Ответы.Find(Convert.ToInt32(mystringTempR[i])); //получаем по id ответа его текст

                                    strR = strR  + " " + answ.Текст_ответа.Replace("  ","") + ", ";
                                    if (answ.Текст_ответа.Replace(" ", "") == AnswR[i].Replace(" ", ""))
                                    {
                                        flagA++;
                                    }

                                }
                            }
                        
                            if (flagA>= AnswR.Count())
                            {
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp]+" " + "\nВаш ответ: " + strR + " - " + "ВЕРНО";
                                iTemp++;
                                Answ++;
                                nQ++;
                                break;
                            }
                            else
                            {
                                QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + " " + "\nВаш ответ: " + strR + " - " + "НЕВЕРНО";
                                iTemp++;
                                nQ++;
                                break;
                            }
                        }
                    }
                    else
                    {

                        //  ResultQuestion.Add(nQ.ToString() + " " + "Неверно");
                        QuestionAnswer[iTemp] = nQ.ToString() + " " + QuestionAnswer[iTemp] + "\nВаш ответ: " + " " + " - " + "НЕВЕРНО";
                        iTemp++;
                        nQ++;
                        break;
                    }
                    break;
                }
            }
            
            double resulProcentTheory = Math.Round((((double)Answ / (double)(QuestC))* 100.0), 3); //результат теста в процентах
            ViewBag.ResultProcent = resulProcentTheory; //передаем результат в % в представление
            ViewBag.ResultQuestion = ResultQuestion; // передаем в представление список , где указано , что верно, а что нет
            ViewBag.QuestionAnswer = QuestionAnswer;
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
