using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    [AutoMapFrom(typeof(Question))]
    public class QuestionWithAnswersDto : QuestionDto
    {
        public List<AnswerDto> Answers { get; set; }
    }
}
