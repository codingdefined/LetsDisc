using Abp.Domain.Entities;
using LetsDisc.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Questions
{
    class QuestionTags : Entity<int>
    {
        [Required]
        public virtual Question Question { get; set; }

        [Required]
        public virtual Tag Tag { get; set; }

        public int QuestionId { get; set; }
        public int TagId { get; set; }

        public QuestionTags()
        {

        }

        public QuestionTags(int questionid, int tagid)
        {
            QuestionId = questionid;
            TagId = tagid;
        }
    }
}
