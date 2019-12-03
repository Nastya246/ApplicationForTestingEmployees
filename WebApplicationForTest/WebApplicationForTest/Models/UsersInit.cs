using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace WebApplicationForTest.Models
{
    public class UsersInit : DropCreateDatabaseAlways<BDForTestingContext>
    {
        
            protected override void Seed(BDForTestingContext db)
            {
                db.Userss.Add(new Users { LastName = "петров", FirstName = "петр", Otchectvo = "Петрович" });
                db.Userss.Add(new Users { LastName = "алексеев", FirstName = "алексей", Otchectvo = "алексеевич" });
                db.Userss.Add(new Users { LastName = "белов", FirstName = "олег", Otchectvo = "олегович" });

                base.Seed(db);
            }
        
    }
}