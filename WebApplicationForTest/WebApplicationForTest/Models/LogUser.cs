using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class LogUser
    {
        public int LogUserId { get; set; }
        public int IdResult { get; set; }
        public string ViewTask { get; set; }
        public string TextTask { get; set; }
        public string AnswerUser { get; set; }
        public bool CorectFlag { get; set; }
    }
}