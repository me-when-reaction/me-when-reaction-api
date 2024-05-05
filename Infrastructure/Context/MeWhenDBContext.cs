using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MeWhen.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Infrastructure.Context
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

            foreach(var m in t[1..])
            {
                builder.Entity(m).ToTable(m.GetCustomAttribute<TableAttribute>()!.Name);
            }
        }
    }
}
