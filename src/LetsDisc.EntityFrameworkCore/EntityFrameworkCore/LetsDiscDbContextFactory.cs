using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using LetsDisc.Configuration;
using LetsDisc.Web;

namespace LetsDisc.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class LetsDiscDbContextFactory : IDesignTimeDbContextFactory<LetsDiscDbContext>
    {
        public LetsDiscDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LetsDiscDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            LetsDiscDbContextConfigurer.Configure(builder, configuration.GetConnectionString(LetsDiscConsts.ConnectionStringName));

            return new LetsDiscDbContext(builder.Options);
        }
    }
}
