using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using LetsDisc.Tags;

namespace LetsDisc.Questions
{
    public class Question : FullAuditedEntity<int, User>, IHasCreationTime
    {
        public const int MaxTitleLength = 255;
        public const int MaxBodyLength = 64 * 1024;

        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(MaxBodyLength)]
        public string Body { get; set; }

        public int UpvoteCount { get; set; }

        public int ViewCount { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public Question()
        {
            CreationTime = DateTime.Now;
        }

        public Question(string title, string body) : this()
        {
            Title = title;
            Body = body;
            UpvoteCount = 0;
            ViewCount = 0;
        }
    }
}
