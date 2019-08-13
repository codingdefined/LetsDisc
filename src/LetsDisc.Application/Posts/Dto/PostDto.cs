using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using LetsDisc.PostDetails;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    [AutoMap(typeof(Post))]
    public class PostDto : FullAuditedEntityDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int PostTypeId { get; set; }
        public int AcceptedAnswerId { get; set; }
        public int ParentId { get; set; }
        public int Score { get; set; }
        public int ViewCount { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string Tags { get; set; }
        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public string CreatorUserName { get; set; }
    }
}
