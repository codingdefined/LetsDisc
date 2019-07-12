using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LetsDisc.Authorization.Users
{
    public class UserDetails : Entity<long>, IHasCreationTime
    {
        public int Reputation { get; set; }
        public string DisplayName { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual long UserId { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public string WebsiteUrl { get; set; }
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public int Views { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
