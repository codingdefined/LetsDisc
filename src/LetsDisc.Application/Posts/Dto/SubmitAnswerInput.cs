using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    public class SubmitAnswerInput
        {
            [Range(1, int.MaxValue)]
            public int QuestionId { get; set; }

            [Required]
            public string Body { get; set; }
        }
}
