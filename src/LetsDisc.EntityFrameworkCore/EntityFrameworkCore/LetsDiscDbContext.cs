using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LetsDisc.Authorization.Roles;
using LetsDisc.Authorization.Users;
using LetsDisc.MultiTenancy;
using LetsDisc.Questions;
using LetsDisc.Tags;
using LetsDisc.PostDetails;
using LetsDisc.Votes;

namespace LetsDisc.EntityFrameworkCore
{
    public class LetsDiscDbContext : AbpZeroDbContext<Tenant, Role, User, LetsDiscDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<UserVoteForQuestion> UserVoteForQuestion { get; set; }

        //Posts
        public virtual DbSet<PostType> PostTypes { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<RelatedPost> RelatedPosts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        //Tags
        public virtual DbSet<PostTag> PostTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TagSynonym> TagSynonyms { get; set; }

        //Votes
        public virtual DbSet<VoteType> VoteTypes { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }

        public LetsDiscDbContext(DbContextOptions<LetsDiscDbContext> options)
            : base(options)
        {
        }
    }
}
