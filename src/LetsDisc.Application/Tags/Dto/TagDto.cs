using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LetsDisc.Tags.Dto
{
    [AutoMapFrom(typeof(Tag))]
    public class TagDto
    {
        public const int MaxNameLength = 15;
        public const int MaxInfoLength = 64 * 1024;

        [Required]
        [StringLength(MaxNameLength)]
        public string TagName { get; set; }

        [StringLength(MaxInfoLength)]
        public string Info { get; set; }

        public int Count { get; set; }
    }
}
