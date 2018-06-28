using LetsDisc.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    public class CreateQuestionInput
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}
