using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MeWhenAPI.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Infrastructure.Context
{
    public class MeWhenDBContext(DbContextOptions builder) : DbContext(builder)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var modType = typeof(BaseModel);
            var t = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(modType.IsAssignableFrom)
                .ToList();

            foreach (var m in t[1..])
            {
                builder.Entity(m).ToTable(m.GetCustomAttribute<TableAttribute>()!.Name);
            }

            // Custom
            builder.Entity<TagModel>()
                .HasIndex(x => x.Name)
                .IncludeProperties(x => new { x.ID, x.AgeRating, x.Alias });
            builder.Entity<TagModel>()
                .Property(x => x.Alias)
                .HasDefaultValue(new List<string>());
        }

        public async Task Transaction(Func<MeWhenDBContext, Task> process)
        {
            await Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                var transaction = await Database.BeginTransactionAsync();
                try
                {
                    await process(this);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
