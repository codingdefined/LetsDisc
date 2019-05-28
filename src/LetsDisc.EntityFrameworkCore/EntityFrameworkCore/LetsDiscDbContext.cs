using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LetsDisc.Authorization.Roles;
using LetsDisc.Authorization.Users;
using LetsDisc.MultiTenancy;
using LetsDisc.Questions;
using LetsDisc.Tags;
using LetsDisc.Posts;

namespace LetsDisc.EntityFrameworkCore
{
    public class LetsDiscDbContext : AbpZeroDbContext<Tenant, Role, User, LetsDiscDbContext>
    {
        /* Define a DbSet for each entity of the application 
         Adding Question, Answer and Tags */

        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<UserVoteForQuestion> UserVoteForQuestion { get; set; }
        public virtual DbSet<PostTypes> PostTypes { get; set; }

        public LetsDiscDbContext(DbContextOptions<LetsDiscDbContext> options)
            : base(options)
        {
        }
    }
}
