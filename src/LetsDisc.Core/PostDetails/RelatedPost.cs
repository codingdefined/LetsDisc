using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetsDisc.PostDetails
{
    public class RelatedPost : Entity<long>, IHasCreationTime
    {
        public string Name { get; set; }

        public virtual DateTime CreationTime { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Posts { get; set; }
        public virtual int PostId { get; set; }

        [ForeignKey("SimilarPostId")]
        public virtual Post SimilarPosts { get; set; }
        public virtual int? SimilarPostId { get; set; }

        public int LinkTypeId { get; set; }
    }
}
