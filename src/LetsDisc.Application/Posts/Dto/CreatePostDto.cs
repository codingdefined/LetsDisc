using Abp.AutoMapper;
using LetsDisc.PostDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    [AutoMapTo(typeof(Post))]
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public int PostTypeId { get; set; }

        public int ParentId { get; set; }

        public string Tags { get; set; }
    }
}
