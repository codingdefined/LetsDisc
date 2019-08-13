using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetsDisc.PostDetails
{
    public class Comment : Entity<long>, ICreationAudited
    {
        public virtual DateTime CreationTime { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Posts { get; set; }
        public virtual int PostId { get; set; }

        public string Text { get; set; }
        public int Score { get; set; }

        public virtual long? CreatorUserId { get; set; }
        public string UserDisplayName { get; set; }

    }
}
