using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    [AutoMapFrom(typeof(Question))]
    public class QuestionDto : CreationAuditedEntityDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int ViewCount { get; set; }
        public string CreatorUserName { get; set; }
    }
}
