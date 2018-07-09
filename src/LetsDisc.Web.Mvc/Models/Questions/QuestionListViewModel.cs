using LetsDisc.Questions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsDisc.Web.Models.Questions
{
    public class QuestionListViewModel
    {
        public IReadOnlyList<QuestionDto> Questions { get; set; }
    }
}
