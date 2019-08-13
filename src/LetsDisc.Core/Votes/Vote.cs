using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LetsDisc.PostDetails;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsDisc.Votes
{
    public class Vote : Entity<int>, IHasCreationTime, ICreationAudited
    {
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        public virtual int PostId { get; set; }

        [ForeignKey("VoteTypeId")]
        public virtual VoteType VoteType { get; set; }
        public virtual int VoteTypeId { get; set; }

        public virtual long? CreatorUserId { get; set; }
        public virtual DateTime CreationTime { get; set; }

        public Vote() { }

        public Vote(int postId, int voteTypeId)
        {
            PostId = postId;
            //CreatorUserId = createdUserId;
            VoteTypeId = voteTypeId;
        }
    }
}
