using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;

namespace LetsDisc.Questions
{
    // Answer Class which will be created as a Table using EntityFramework
    // We are using FullAuditedEntity because it will store all the changes happened on the entity like CreaterUser, DeletedUser, EditerUser with name and timestamp
    // IHasCreationTime is used have creation time on entity
    public class Answer : CreationAuditedEntity<int, User>, IHasCreationTime
    {
        // Maximum Length of Body, subject to change based on the feedback
        public const int MaxBodyLength = 64 * 1024;

        // The required and MaxLength Properties will eventually add Not Null and Column DataType in the Database
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
