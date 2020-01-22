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
            if (units != "")
            {
                idUnits = Convert.ToInt32(units);
                var unit = await db.Подразделение.FindAsync(idUnits);
                ViewBag.Units = unit.Название_подразделения.Replace("  ", "");
                ViewBag.UnitsId = units;
            }
            if (us == "")
            {
                if ((beginCalendar == "") && (endCalendar == ""))
                {
                    var user = (from u in db.Пользователи where u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0 select u);
                    var userResult = user.Include(d => d.Результат_теста);
                    var sortedUsers = from u in userResult orderby u.Фамилия select u;
                    return View(sortedUsers);
                }
                else if ((beginCalendar != "") && (endCalendar == "")) //если указано только начало периода
                {
                    List<Пользователи> usData = new List<Пользователи>();
                   
                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    List<Результат_теста> результат_ = new List<Результат_теста>();
                    foreach (var t in userResult)
                    {
                        результат_.Clear();
                        foreach (var d in t.Результат_теста)
                        {
                            if (d.Дата_сдачи_теории != null)
                            {
                                int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(beginCalendar));
                                int dataFlagP = 0;
                                if (d.Дата_сдачи_теории != null)
                                {
                                     dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(beginCalendar));
                                }

                                if ((dataFlagT == 0) || (dataFlagT > 0)&&(((dataFlagP == 0) || (dataFlagP > 0))|| (d.Дата_сдачи_теории==null)))
                                {
                                    результат_.Add(d);
                                   
                                }
                            }
                            }
                        if (результат_.Count()>0)
                        {
                            Пользователи пользователи = new Пользователи();
                            пользователи.id_user = t.id_user;
                            пользователи.id_должности = t.id_должности;
                            пользователи.Должность = t.Должность;
                            пользователи.Подразделение= t.Подразделение;
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

                else if ((beginCalendar == "") && (endCalendar != "")) //если указано только конец периода
                {
                    List<Пользователи> usData = new List<Пользователи>();

                    var userResult = (from u in db.Пользователи where (u.Подразделение.Id_подразделения == idUnits && u.Результат_теста.Count != 0) orderby u.Фамилия select u);
                    List<Результат_теста> результат_ = new List<Результат_теста>();
                    foreach (var t in userResult)
                    {
                        результат_.Clear();
                        foreach (var d in t.Результат_теста)
                        {
                            if (d.Дата_сдачи_теории != null)
                            {
                                int dataFlagT = Convert.ToDateTime(d.Дата_сдачи_теории).CompareTo(Convert.ToDateTime(endCalendar));
                                int dataFlagP = 0;
                                if (d.Дата_сдачи_теории != null)
                                {
                                    dataFlagP = Convert.ToDateTime(d.Дата_сдачи_практики).CompareTo(Convert.ToDateTime(endCalendar));
                                }

                                if ((dataFlagT == 0) || (dataFlagT < 0) && (((dataFlagP == 0) || (dataFlagP < 0)) || (d.Дата_сдачи_теории == null)))
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
            }
            return View();

        }
        // GET: Отчеты/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Отчеты/Create
        public ActionResult Create()
        {
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

        // GET: Отчеты/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Отчеты/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Отчеты/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Отчеты/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
