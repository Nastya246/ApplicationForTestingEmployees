using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebApplicationForTest.Models
    //для динамической проверки полей при вводе и редактировании вопросов
{
    public class ВопросОтветРедактор
    {
        public Вопросы вопросыРедактор { get; set;  }
        [Required(ErrorMessage = "Введите верный ответ")]
        public string Правильный_ответ { get; set; }      
        [Required(ErrorMessage = "Введите варианты ответов")]
        public string Варианты_ответов { get; set; }
        [Required(ErrorMessage = "Введите понятия")]
        public string Понятия { get; set; }
    }
}