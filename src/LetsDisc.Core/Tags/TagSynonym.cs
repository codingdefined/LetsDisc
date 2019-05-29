using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetsDisc.Tags 
{
    public class TagSynonym : Entity<long>,ICreationAudited
    {
        public const int MaxNameLength = 15;

        [Required]
        [MaxLength(MaxNameLength)]
        public string SourceTagName { get; set; }

        [MaxLength(MaxNameLength)]
        public string TargetTagName { get; set; }

        public virtual long? CreatorUserId { get; set; }
        public virtual DateTime CreationTime { get; set; }

        [ForeignKey("ApprovedByUserId")]
        public virtual User ApprovedByUser { get; set; }
        public virtual long ApprovedByUserId { get; set; }

        public DateTime ApprovalDate { get; set; }
    }
}
