using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    public class GetQuestionInput : EntityDto
    {
        public bool IncrementViewCount { get; set; }
    }
}