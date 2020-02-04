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
        public async Task<ActionResult> Index(FormCollection form) //обработка результатов по тесту
        {

            var k = form.Keys;
            List<string> ResultQuestion = new List<string>(k.Count); //для хранения результатов верно/неверно
            List<string> QuestionAnswer = new List<string>(k.Count); //для хранения вопросов и ответов
            Dictionary<string, string> resultA = new Dictionary<string, string>(k.Count); //для хранения первичных результатов
            var temp = form.ToValueProvider(); //получаем все данные
            string kD = ""; // для ключа словаря
            string valD = ""; //для значения словаря
            int id_Test = 0; // для id теста
            int flagS = 0; // для количества вопросов в соотношений
            int tempIdQ = 0; //для получения id вопроса типа "соотношение"
            string tempStrQ = "";
            int id_User = 0;
            string data = "";
            foreach (var val in k)
            {

                kD = val.ToString().Replace(" ", "");
                if (resultA.ContainsKey(kD)) //если есть запись в словаре под таким ключом, то удаляем, по идеи ее быть не должно, но мало ли
                {
                    resultA.Remove(kD);
                }
                if (kD == "id_test") //если получили id_теста, сохраняем в отдельной переменной
                {
                    id_Test = Convert.ToInt32(temp.GetValue(kD).AttemptedValue);
                }
                else if (kD == "id_user") //если получили id_пользователя, сохраняем в отдельной переменной
                {
                    id_User = Convert.ToInt32(temp.GetValue(kD).AttemptedValue);
                    ViewBag.Id_user = id_User;
                }
                else if (kD == "Data") //если получили дату прохождения теста, сохраняем в отдельной переменной
                {
                    data = temp.GetValue(kD).AttemptedValue;
                    ViewBag.Data = data;
                }
              
                else
                {
                    valD = temp.GetValue(kD).AttemptedValue.Replace(" ", ""); // если получили ответы пользователя, то сохраняем в словарь
                    string[] mystringSootn = kD.Split('-');

                    if (mystringSootn.Count() == 2) // пришел ответ типа соотношение
                    {
                        kD = "О " + mystringSootn[0]; // помечаем ответы из соотношения буквой "О" перед ключом 

                    }

                    string[] mystring = valD.Split(','); //обработка результатов из checkbox
                    List<string> ls = new List<string>(mystring.Count());
                    if (mystring.Count() > 1) //значит пришел результат со множетсвенным выбором, обрабатываем его
                    {
                        for (int i = 0; i < mystring.Count(); i++)
                        {

                            ls.Add(mystring[i]);
                            if (mystring[i] == "true")
                            {
                                i++; //если пользователь отметил checkbox, то придет два значения - true и false, false пропускаем
                            }
                        }
                        valD = "";
                        foreach (var l in ls)
                        {
                            valD = valD + l + " ";
                        }
                    }
                   
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
                            QuestionAnswer.Add("Вопрос: " + textQ +"\n"+ "Правильный ответ: " + textA +"\n");
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
                    QuestionAnswer.Add("Вопрос: " + textQ + "\n" + "Правильный ответ: " + textA + "\n");
                }
                else if (qList.Тип_ответа.Replace(" ", "") == "Соотношение")
                {
                   
                    textA = "";
                    var tempSelectA = (from answ in db.Ответы where answ.id_Вопроса == qList.id_вопроса select answ).ToList();
                    foreach (var tempL in tempSelectA)
                    {
                        if (tempL.Флаг_подвопроса == true) //получаем подвопросы и ответы к ним
                        {
                            textA = textA + tempL.Текст_ответа.Replace("  ", "") + "-" + tempL.Правильный_ответ.Replace("  ", "") + ",";
                        }
                        
                    }

                    QuestionAnswer.Add("Вопрос: " + textQ + "\n" + "Правильный ответ: " + textA + "\n");
                }
                else if (qList.Тип_ответа.Replace(" ", "") == "Разрыв")
                {
                    var tempSelectR = (from answ in db.Ответы where answ.id_Вопроса == qList.id_вопроса && answ.Флаг_правильного_ответа==true select answ).ToList();
                  
                    QuestionAnswer.Add("Вопрос: " + textQ + "\n" + "Правильный ответ: " + tempSelectR[0].Текст_ответа.Replace("  ", "") + "\n");
                }
            }
            foreach (var q1 in AllQuestion) //проверка на пустые поля в ответах
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
                            Ответы ответы = await db.Ответы.FindAsync(id_Ans);
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
                            string[] tempStrForSoothosh =new string[2]; //для хранения id_ответа и выбора пользователя
                            string AnswUser = "";
                            string strResult = "";
                            int strC = 1;
                          //  strResult = string.Join(",", result);
                            foreach (var str in result)
                            {
                                tempStrForSoothosh = str.Split(' ');
                                if (tempStrForSoothosh[1] != "")
                                {
                                    
                                   
                                    foreach (var valueSootnosh in tempSelect) //ищем значение под выбранным вариантом
                                    {
                                       Ответы ответ = await db.Ответы.FindAsync(Convert.ToInt32(tempStrForSoothosh[1]));
                                        if ((valueSootnosh.Флаг_подвопроса == false) && (valueSootnosh.Текст_ответа.Replace(" ","") == (ответ.Текст_ответа.Replace(" ","") )))
                                        {
                                           
                                            if ((tempStrForSoothosh[0] != "") && ((tempStrForSoothosh[1] != "")))
                                            {
                                                AnswUser = ответ.Текст_ответа.Replace("  ", ""); //записываем вариант ответа
                                                Ответы ответы = await db.Ответы.FindAsync(Convert.ToInt32(tempStrForSoothosh[0]));
                                                if (AnswUser.Replace(" ", "") == ответы.Правильный_ответ.Replace(" ", "")) //сраниваем вариант ответа подвопроса и того значения, что под буквой
                                                {
                                                    flagCorrect++;
                                                }
                                                strResult = strResult + strC.ToString()+"." + ответы.Текст_ответа.Replace("  ","")+"-" + (ответ.Текст_ответа.Replace(" ", "")) + " ";
                                            }
                                        }
                                    }
                                   
                                }
                                else
                                {
                                    Ответы ответы = await db.Ответы.FindAsync(Convert.ToInt32(tempStrForSoothosh[0]));
                                    strResult = strResult + strC.ToString()+". " + ответы.Текст_ответа.Replace("  ", "")+"-" + "  ";
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
            
            double resulProcentTheory = Math.Round((((double)Answ / (double)(QuestC))* 100.0), 2); //результат теста в процентах
            ViewBag.ResultProcent = Convert.ToInt32(resulProcentTheory); //передаем результат в % в представление
            ViewBag.ResultQuestion = ResultQuestion; // передаем в представление список , где указано , что верно, а что нет
            ViewBag.QuestionAnswer = QuestionAnswer;
            nQ = 0;
           
            var usTestResult = await (from u in db.Результат_теста where u.id_User == id_User && u.id_Теста == id_Test select u).ToListAsync(); //Проверяем проходил ли пользователь этот тест
           
            if (usTestResult.Count > 0) //если проходил, то обновляем инфу
            {
                Результат_теста результат_Теста = db.Результат_теста.Find(usTestResult.First().id_результата_теста);
                результат_Теста.id_User = id_User;
                результат_Теста.id_Теста = id_Test;
                результат_Теста.Дата_сдачи_теории = Convert.ToDateTime(data);
                результат_Теста.Оценка_за_теорию = Convert.ToInt32(resulProcentTheory);
                db.Entry(результат_Теста).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            else //если нет, то добавляем новую запись в бд
            {
                Результат_теста результат_Теста = new Результат_теста();
                результат_Теста.id_User = id_User;
                результат_Теста.id_Теста = id_Test;
                результат_Теста.Дата_сдачи_теории = Convert.ToDateTime(data);
                результат_Теста.Оценка_за_теорию = Convert.ToInt32(resulProcentTheory);
                db.Результат_теста.Add(результат_Теста);
                await db.SaveChangesAsync();
            }
            var результат_вопроса = db.Результат_теста.Include(р => р.Пользователи);
            return View(await результат_вопроса.ToListAsync());
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
