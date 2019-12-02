using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class AnswerPractice
    {
        public int AnswerPracticeId { get; set; }
        public int IdPracticeQuestion { get; set; }
        public string VariantsAnswer { get; set; }
        public string CorrectVariantAnswer { get; set; }
    }
}