using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace LetsDisc.Tags
{
    public class Tag : Entity<int>
    {
        public const int MaxNameLength = 15;
        public const int MaxInfoLength = 64 * 1024;

        [Required]
        [MaxLength(MaxNameLength)]
        public string TagName { get; set; }

        [MaxLength(MaxInfoLength)]
        public string Info { get; set; }

        public int Count { get; set; }
    }
}