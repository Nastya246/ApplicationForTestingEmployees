using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebApplicationForTest.Models
{
    public class Users //пользователи
    { 
        public int UsersId { get; set; }
       // public string Login { get; set; }
      //  public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Otchectvo { get; set; }
      //  public int PlaceWorkID { get; set; }
        
    }
}