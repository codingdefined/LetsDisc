using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using LetsDisc.Tags;

namespace LetsDisc.Questions
{
    // Question Class which will be created as a Table using EntityFramework
    // We are using FullAuditedEntity because it will store all the changes happened on the entity like CreaterUser, DeletedUser, EditerUser with name and timestamp
    // IHasCreationTime is used have creation time on entity
    public class Question : FullAuditedEntity<int, User>, IHasCreationTime
    {
        // Maximum Length of Title and Body, subject to change based on the feedback
        public const int MaxTitleLength = 255;
        public const int MaxBodyLength = 64 * 1024;

        // The required and MaxLength Properties will eventually add Not Null and Column DataType in the Database

        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(MaxBodyLength)]
        public string Body { get; set; }

        public int UpvoteCount { get; set; }

        public int ViewCount { get; set; }

        // ICollection of all the Entities Which are Linked with Questions
        public ICollection<Answer> Answers { get; set; }
        public ICollection<UserVoteForQuestion> UsersVoted { get; set; }
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
