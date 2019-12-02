using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationForTest.Models
{
    public class Topic
    {
        public int TopicId { get; set; }
        public int IdSection { get; set; }
        public string NameTopic { get; set; }
    }
}