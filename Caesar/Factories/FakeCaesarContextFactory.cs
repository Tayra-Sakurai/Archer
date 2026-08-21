using Caesar.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Factories
{
    public class FakeCaesarContextFactory : IDesignTimeDbContextFactory<CaesarContext>
    {
        public CaesarContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<CaesarContext> dbContextOptionsBuilder = new();

            dbContextOptionsBuilder
                .UseSqlite($"Data Source=Caesar.db");

            return new(dbContextOptionsBuilder.Options);
        }
    }
}
