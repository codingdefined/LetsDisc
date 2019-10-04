using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace LetsDisc.EntityFrameworkCore
{
    public static class LetsDiscDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<LetsDiscDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<LetsDiscDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
