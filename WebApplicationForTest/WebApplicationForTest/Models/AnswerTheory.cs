using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class AnswerTheory
    {
        public int AnswerTheoryId { get; set; }
        public int IdTheoryQuestion { get; set; }
        public string VariantsAnswer { get; set; }
        public string CorrectVariantAnswer { get; set; }
    }
}