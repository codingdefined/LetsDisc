using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LetsDisc.Authorization.Roles;
using LetsDisc.Authorization.Users;
using LetsDisc.MultiTenancy;

namespace LetsDisc.EntityFrameworkCore
{
    public class LetsDiscDbContext : AbpZeroDbContext<Tenant, Role, User, LetsDiscDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public LetsDiscDbContext(DbContextOptions<LetsDiscDbContext> options)
            : base(options)
        {
        }
    }
}
