using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class Practice
    {
        public int PracticeId { get; set; }
        public int IdTheme { get; set; }
        public string HeadQuestion { get; set; }
        public string TextQuestion { get; set; }
        public string TypeAnswer { get; set; }
    }
}