using Abp.Domain.Entities.Auditing;
using LetsDisc.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace LetsDisc.Tags
{
    public class Tag : CreationAuditedEntity<int, User>, IHasCreationTime
    {
        public const int MaxNameLength = 15;
        public const int MaxInfoLength = 64 * 1024;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        [MaxLength(MaxInfoLength)]
        public string Info { get; set; }

        public Tag()
        {
            CreationTime = DateTime.Now;
        }

        public Tag(string name, string info) : this()
        {
            Name = name;
            Info = info;
        }
    }
}