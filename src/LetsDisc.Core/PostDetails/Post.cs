using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsDisc.PostDetails
{
    // Posts Class which will be created as a Table using EntityFramework
    // We are using FullAuditedEntity because it will store all the changes happened on the entity like CreaterUser, DeletedUser, EditedUser with name and timestamp
    // IHasCreationTime is used have creation time on entity
    public class Post : FullAuditedEntity<int, User>
    {
        // Maximum Length of Title and Body, subject to change based on the feedback
        public const int MaxTitleLength = 255;

        public const int MaxBodyLength = 64 * 1024;

        [ForeignKey("PostTypeId")]
        public virtual PostType PostTypes { get; set; }
        public virtual int PostTypeId { get; set; }

        public int AcceptedAnswerId { get; set; }
        public int ParentId { get; set; }

        [Required]
        public int Score { get; set; }
        public int ViewCount { get; set; }

        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(MaxBodyLength)]
        public string Body { get; set; }

        public DateTime LastActivityDate { get; set; }
        [MaxLength(MaxTitleLength)]
        public string Tags { get; set; }

        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }

        public Post()
        {
            CreationTime = DateTime.Now;
            LastActivityDate = DateTime.Now;
            Score = 0;
        }
    }
}
