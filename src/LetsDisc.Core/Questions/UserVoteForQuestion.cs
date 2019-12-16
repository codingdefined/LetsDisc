using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Questions
{
    // This Class Holds the User Information who has voted for any question
    public class UserVoteForQuestion : Entity<int>
    {
        [Required]
        public virtual Question Question { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int QuestionId { get; set; }
        public long UserId { get; set; }
        public bool IsUpvoted { get; set; }
        public bool IsDownvoted { get; set; }

        public UserVoteForQuestion()
        {
        }

        public UserVoteForQuestion(int questionid, long userid, bool isUpvoted, bool isDownvoted)
        {
            QuestionId = questionid;
            UserId = userid;
            IsUpvoted = isUpvoted;
            IsDownvoted = isDownvoted;
        }
    }
}
