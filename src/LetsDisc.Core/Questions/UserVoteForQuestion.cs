using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Questions
{
    public class UserVoteForQuestion : Entity<int>
    {
        [Required]
        public virtual Question Question { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int QuestionId { get; set; }
        public long UserId { get; set; }
        public bool IsVoted { get; set; }

        public UserVoteForQuestion(int questionid, long userid, bool isVoted)
        {
            QuestionId = questionid;
            UserId = userid;
            IsVoted = isVoted;
        }
    }
}
