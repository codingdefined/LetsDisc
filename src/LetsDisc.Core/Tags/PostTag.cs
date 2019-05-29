using Abp.Domain.Entities;
using LetsDisc.PostDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetsDisc.Tags
{
    public class PostTag : Entity<int>
    {
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        public virtual int PostId { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
        public virtual int TagId { get; set; }
    }
}
