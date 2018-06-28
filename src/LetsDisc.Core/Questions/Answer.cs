using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;

namespace LetsDisc.Questions
{
    public class Answer : CreationAuditedEntity<int, User>, IHasCreationTime
    {
        public const int MaxBodyLength = 64 * 1024;

        [Required]
        [MaxLength(MaxBodyLength)]
        public string Body { get; set; }

        public int UpvoteCount { get; set; }

        public virtual Question Question { get; set; }

        public int QuestionId { get; set; }
        public bool IsAccepted { get; set; }

        public Answer()
        {
            CreationTime = DateTime.Now;
        }

        public Answer(string body) : this()
        {
            Body = body;
            UpvoteCount = 0;
        }
    }
}
