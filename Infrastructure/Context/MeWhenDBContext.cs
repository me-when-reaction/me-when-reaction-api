using System;
using MeWhen.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Infrastructure.Context
{
    public class MeWhenDBContext(DbContextOptions builder) : DbContext(builder)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
