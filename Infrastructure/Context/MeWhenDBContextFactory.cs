using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeWhen.Infrastructure.Context
{
    public class MeWhenDBContextFactory : IDesignTimeDbContextFactory<MeWhenDBContext>
    {
        public MeWhenDBContext CreateDbContext(string[] args)
        {
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .Build();

            return new MeWhenDBContext(new DbContextOptionsBuilder<MeWhenDBContext>()
                .UseNpgsql(conf.GetConnectionString("Default")).Options);
        }
    }
}
