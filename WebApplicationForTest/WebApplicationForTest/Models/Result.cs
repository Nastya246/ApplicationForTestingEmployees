using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int IdUser { get; set; }
        public DateTime DataTheory { get; set; }
        public int CountQuestionTheory { get; set; }
        public int CountCorectAnswer { get; set; }
        public int GradeTheory { get; set; }
        public DateTime DataPractice { get; set; }
        public bool PracticeCounted { get; set; }
    }
}