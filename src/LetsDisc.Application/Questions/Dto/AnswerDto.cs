using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    [AutoMapFrom(typeof(Answer))]
    public class AnswerDto : CreationAuditedEntityDto
    {
        public string Body { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int QuestionId { get; set; }
        public bool IsAccepted { get; set; }
        public string CreatorUserName { get; set; }
    }
}
