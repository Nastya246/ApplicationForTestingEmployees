using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using System.Threading.Tasks;
using System.Net;

using WebApplicationForTest.Models;
namespace WebApplicationForTest.Controllers
{
    public class ОтчетыController : Controller
    {
        private TestEntities db = new TestEntities();
        // GET: Отчеты
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string units, string us="", string beginCalendar="", string endCalendar="")
        {
            
            int idUnits = 1;
            ViewBag.Choose = us;
            if (units != "")
            {
                idUnits = Convert.ToInt32(units);
                var unit = await db.Подразделение.FindAsync(idUnits);
                ViewBag.Units = unit.Название_подразделения.Replace("  ", "");
                ViewBag.UnitsId = units;
            }
            if ((us == "") || (us == "5"))//если доп. параметры не выбраны
            {

                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    var user = (from u in db.Пользователи where u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0 select u);
                    var userResult = user.Include(d => d.Результат_теста);
                    var sortedUsers = from u in userResult orderby u.Фамилия select u;
                    if (sortedUsers.Count() != 0)
                    {
                        return View(sortedUsers);
                    }
                    else
                    {
                        return View();
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    List<Результат_теста> результат_ = new List<Результат_теста>();
                    if (userResult.Count() != 0)
                    {
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    int dataFlagP = 0;
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((dataFlagT == 0) || (dataFlagT > 0)) && ((dataFlagP == 0) || (dataFlagP > 0) || (d.Дата_сдачи_практики == null)))
                                    {
                                        результат_.Add(d);

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    List<Результат_теста> результат_ = new List<Результат_теста>();
                    if (userResult.Count() != 0)
                    {
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagP = 0;
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                    }

                                    if (((dataFlagT == 0) || (dataFlagT < 0)) && ((((dataFlagP == 0) || (dataFlagP < 0)) || (d.Дата_сдачи_практики == null))))
                                    {
                                        результат_.Add(d);

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    List<Результат_теста> результат_ = new List<Результат_теста>();
                    if (userResult.Count() != 0)
                    {
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagPEnd = 0;
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    int dataFlagPBegin = 0;
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagPEnd = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                        dataFlagPBegin = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if ((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && ((((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0) || (d.Дата_сдачи_практики == null)))
                                    {
                                        результат_.Add(d);

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    { return View();
                    }
                }
            }
            else if (us == "1") //пользователи, которые успешно сдали теорию и практику
            {

                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    List<Пользователи> usSuccess = new List<Пользователи>();
                    List<Результат_теста> результат = new List<Результат_теста>();
                    var user = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) select u);
                    if (user.Count() != 0)
                    {
                        var userResult = user.Include(d => d.Результат_теста);
                        var sortedUsers = from u in userResult orderby u.Фамилия select u;
                        foreach (var userSuccess in sortedUsers)
                        {
                            результат.Clear();
                            foreach (var result in userSuccess.Результат_теста)
                            {
                                if (result.Общий_результат != null)
                                {
                                    if (result.Общий_результат.Replace(" ", "") == "Сдал")
                                    {
                                        результат.Add(result);
                                    }
                                }
                            }
                            if (результат.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = userSuccess.id_user;
                                пользователи.id_должности = userSuccess.id_должности;
                                пользователи.Должность = userSuccess.Должность;
                                пользователи.Подразделение = userSuccess.Подразделение;
                                пользователи.id_подразделения = userSuccess.id_подразделения;
                                пользователи.Имя = userSuccess.Имя;
                                пользователи.Фамилия = userSuccess.Фамилия;
                                пользователи.Отчество = userSuccess.Отчество;
                                foreach (var r in результат)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usSuccess.Add(пользователи);
                            }
                        }

                        return View(usSuccess);
                    }
                    else
                    {
                        return View();
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null) && (d.Дата_сдачи_практики != null))
                                {
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    int dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));

                                    if (((dataFlagT == 0) || (dataFlagT > 0) && ((dataFlagP == 0) || (dataFlagP > 0))) && (d.Общий_результат != null))
                                    {
                                        if (d.Общий_результат.Replace(" ", "") == "Сдал")
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    { return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null) && (d.Дата_сдачи_практики != null))
                                {
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));


                                    if ((((dataFlagT == 0) || (dataFlagT < 0)) && ((dataFlagP == 0) || (dataFlagP < 0))) && (d.Общий_результат != null))
                                    {
                                        if (d.Общий_результат.Replace(" ", "") == "Сдал")
                                        {
                                            результат_.Add(d); }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();

                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null) && (d.Дата_сдачи_практики != null))
                                {
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));

                                    int dataFlagPEnd = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagPBegin = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));


                                    if (((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && (((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0)) && (d.Общий_результат != null))
                                    {

                                        if (d.Общий_результат.Replace(" ", "") == "Сдал")
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

            }
            else if (us == "2") //пользователи, которые успешно сдали теорию 
            {

                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    List<Пользователи> usSuccess = new List<Пользователи>();
                    List<Результат_теста> результат = new List<Результат_теста>();
                    var user = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) select u);
                    if (user.Count() != 0)
                    {
                        var userResult = user.Include(d => d.Результат_теста);
                        var sortedUsers = from u in userResult orderby u.Фамилия select u;
                        foreach (var userSuccess in sortedUsers)
                        {
                            результат.Clear();
                            foreach (var result in userSuccess.Результат_теста)
                            {
                                if (result.Оценка_за_теорию != null)
                                {
                                    if (result.Оценка_за_теорию >= 60)
                                    {
                                        результат.Add(result);
                                    }
                                }
                            }
                            if (результат.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = userSuccess.id_user;
                                пользователи.id_должности = userSuccess.id_должности;
                                пользователи.Должность = userSuccess.Должность;
                                пользователи.Подразделение = userSuccess.Подразделение;
                                пользователи.id_подразделения = userSuccess.id_подразделения;
                                пользователи.Имя = userSuccess.Имя;
                                пользователи.Фамилия = userSuccess.Фамилия;
                                пользователи.Отчество = userSuccess.Отчество;
                                foreach (var r in результат)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usSuccess.Add(пользователи);
                            }
                        }

                        return View(usSuccess);
                    }
                    else
                    {
                        return View();
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((dataFlagT == 0) || (dataFlagT > 0) && ((dataFlagP == 0) || (dataFlagP > 0))) && (d.Оценка_за_теорию != null))
                                    {
                                        if (d.Оценка_за_теорию >= 60)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null))
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                    }

                                    if ((((dataFlagT == 0) || (dataFlagT < 0)) && ((dataFlagP == 0) || (dataFlagP < 0))) && (d.Оценка_за_теорию != null))
                                    {
                                        if (d.Оценка_за_теорию >= 60)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();

                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagPEnd = 0;
                                    int dataFlagPBegin = 0;
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagPEnd = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                        dataFlagPBegin = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && (((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0)) && (d.Оценка_за_теорию != null))
                                    {

                                        if (d.Оценка_за_теорию >= 60)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

            }
            else if (us == "3") //пользователи, которые не сдали теорию и практику
            {
                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    List<Пользователи> usSuccess = new List<Пользователи>();
                    List<Результат_теста> результат = new List<Результат_теста>();
                    var user = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) select u);
                    if (user.Count() != 0)
                    {
                        var userResult = user.Include(d => d.Результат_теста);
                        var sortedUsers = from u in userResult orderby u.Фамилия select u;
                        foreach (var userSuccess in sortedUsers)
                        {
                            результат.Clear();
                            foreach (var result in userSuccess.Результат_теста)
                            {
                                if (((result.Оценка_за_теорию == null) || (result.Оценка_за_теорию < 60)) && ((result.Отметка_о_практике == false) || (result.Отметка_о_практике == null)))
                                {

                                    результат.Add(result);

                                }
                            }
                            if (результат.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = userSuccess.id_user;
                                пользователи.id_должности = userSuccess.id_должности;
                                пользователи.Должность = userSuccess.Должность;
                                пользователи.Подразделение = userSuccess.Подразделение;
                                пользователи.id_подразделения = userSuccess.id_подразделения;
                                пользователи.Имя = userSuccess.Имя;
                                пользователи.Фамилия = userSuccess.Фамилия;
                                пользователи.Отчество = userSuccess.Отчество;
                                foreach (var r in результат)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usSuccess.Add(пользователи);
                            }
                        }

                        return View(usSuccess);
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((dataFlagT == 0) || (dataFlagT > 0) && ((dataFlagP == 0) || (dataFlagP > 0))) && (d.Оценка_за_теорию != null) && (d.Отметка_о_практике != null))
                                    {
                                        if ((d.Оценка_за_теорию < 60) && (d.Отметка_о_практике == false))
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null))
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                    }

                                    if ((((dataFlagT == 0) || (dataFlagT < 0)) && ((dataFlagP == 0) || (dataFlagP < 0))) && (d.Оценка_за_теорию != null) && (d.Отметка_о_практике != null))
                                    {
                                        if ((d.Оценка_за_теорию < 60) && (d.Отметка_о_практике == false))
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();

                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagPEnd = 0;
                                    int dataFlagPBegin = 0;
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagPEnd = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                        dataFlagPBegin = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && (((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0)) && (d.Оценка_за_теорию != null) && (d.Отметка_о_практике != null))
                                    {

                                        if ((d.Оценка_за_теорию < 60) && (d.Отметка_о_практике == false))
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }


                else
                {
                    return View();
                }
                



            }
            else if (us == "4") //пользователи, которые не сдавали практику
            {
                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    List<Пользователи> usSuccess = new List<Пользователи>();
                    List<Результат_теста> результат = new List<Результат_теста>();
                    var user = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) select u);
                    if (user.Count() != 0)
                    {
                        var userResult = user.Include(d => d.Результат_теста);
                        var sortedUsers = from u in userResult orderby u.Фамилия select u;
                        foreach (var userSuccess in sortedUsers)
                        {
                            результат.Clear();
                            foreach (var result in userSuccess.Результат_теста)
                            {
                                if (result.Отметка_о_практике == null)
                                {

                                    результат.Add(result);

                                }
                            }
                            if (результат.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = userSuccess.id_user;
                                пользователи.id_должности = userSuccess.id_должности;
                                пользователи.Должность = userSuccess.Должность;
                                пользователи.Подразделение = userSuccess.Подразделение;
                                пользователи.id_подразделения = userSuccess.id_подразделения;
                                пользователи.Имя = userSuccess.Имя;
                                пользователи.Фамилия = userSuccess.Фамилия;
                                пользователи.Отчество = userSuccess.Отчество;
                                foreach (var r in результат)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usSuccess.Add(пользователи);
                            }
                        }

                        return View(usSuccess);
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));


                                    if (((dataFlagT == 0) || (dataFlagT > 0) && ((dataFlagP == 0) || (dataFlagP > 0))) && (d.Отметка_о_практике == null))
                                    {

                                        результат_.Add(d);

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }

                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null))
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));


                                    if ((((dataFlagT == 0) || (dataFlagT < 0)) && ((dataFlagP == 0) || (dataFlagP < 0))) && (d.Отметка_о_практике == null))
                                    {

                                        результат_.Add(d);


                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();

                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagPEnd = 0;
                                    int dataFlagPBegin = 0;
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));


                                    if (((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && (((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0)) && (d.Отметка_о_практике == null))
                                    {

                                        результат_.Add(d);


                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }


                else
                {
                    return View();
                }
                

            }
            else if (us == "6") //пользователи, которые не сдали  практику
            {
                if ((beginCalendar == "") && (endCalendar == "")) //если период не указан
                {
                    List<Пользователи> usSuccess = new List<Пользователи>();
                    List<Результат_теста> результат = new List<Результат_теста>();
                    var user = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) select u);
                    if (user.Count() != 0)
                    {
                        var userResult = user.Include(d => d.Результат_теста);
                        var sortedUsers = from u in userResult orderby u.Фамилия select u;
                        foreach (var userSuccess in sortedUsers)
                        {
                            результат.Clear();
                            foreach (var result in userSuccess.Результат_теста)
                            {
                                if ((result.Отметка_о_практике == false) || (result.Отметка_о_практике == null))
                                {

                                    результат.Add(result);

                                }
                            }
                            if (результат.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = userSuccess.id_user;
                                пользователи.id_должности = userSuccess.id_должности;
                                пользователи.Должность = userSuccess.Должность;
                                пользователи.Подразделение = userSuccess.Подразделение;
                                пользователи.id_подразделения = userSuccess.id_подразделения;
                                пользователи.Имя = userSuccess.Имя;
                                пользователи.Фамилия = userSuccess.Фамилия;
                                пользователи.Отчество = userSuccess.Отчество;
                                foreach (var r in результат)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usSuccess.Add(пользователи);
                            }
                        }

                        return View(usSuccess);
                    }
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((dataFlagT == 0) || (dataFlagT > 0) && ((dataFlagP == 0) || (dataFlagP > 0))) && (d.Отметка_о_практике != null))
                                    {
                                        if (d.Отметка_о_практике == false)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }

                else if ((beginCalendar == "") && (endCalendar != "")) //если указан только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if ((d.Дата_сдачи_теории != null))
                                {
                                    int dataFlagP = 0;
                                    int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                    }

                                    if ((((dataFlagT == 0) || (dataFlagT < 0)) && ((dataFlagP == 0) || (dataFlagP < 0))) && (d.Отметка_о_практике != null))
                                    {
                                        if (d.Отметка_о_практике == false)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();

                    }
                }
                else if ((beginCalendar != "") && (endCalendar != "")) //если указан период
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    if (userResult.Count() != 0)
                    {
                        List<Результат_теста> результат_ = new List<Результат_теста>();
                        foreach (var t in userResult)
                        {
                            результат_.Clear();
                            foreach (var d in t.Результат_теста)
                            {
                                if (d.Дата_сдачи_теории != null)
                                {
                                    int dataFlagPEnd = 0;
                                    int dataFlagPBegin = 0;
                                    int dataFlagTEnd = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                    int dataFlagTBegin = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                    if (d.Дата_сдачи_практики != null)
                                    {
                                        dataFlagPEnd = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                        dataFlagPBegin = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                    }

                                    if (((((dataFlagTEnd == 0) || (dataFlagTEnd < 0)) && (dataFlagTBegin > 0)) && (((dataFlagPEnd == 0) || (dataFlagPEnd < 0)) && dataFlagPBegin > 0)) && (d.Отметка_о_практике != null))
                                    {

                                        if (d.Отметка_о_практике == false)
                                        {
                                            результат_.Add(d);
                                        }

                                    }
                                }
                            }
                            if (результат_.Count() > 0)
                            {
                                Пользователи пользователи = new Пользователи();
                                пользователи.id_user = t.id_user;
                                пользователи.id_должности = t.id_должности;
                                пользователи.Должность = t.Должность;
                                пользователи.Подразделение = t.Подразделение;
                                пользователи.id_подразделения = t.id_подразделения;
                                пользователи.Имя = t.Имя;
                                пользователи.Фамилия = t.Фамилия;
                                пользователи.Отчество = t.Отчество;
                                foreach (var r in результат_)
                                {
                                    пользователи.Результат_теста.Add(r);
                                }
                                usData.Add(пользователи);
                            }
                        }


                        return View(usData);
                    }
                    else
                    {
                        return View();
                    }
                }


                else
                {
                    return View();
                }
            }



                
            
            return View();

        }
       
        // POST: Отчеты/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
