using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class Test
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int IdResult { get; set; }
        public int CountTaskTheory { get; set; }
        public int CountTaskPractice { get; set; }
    }
}