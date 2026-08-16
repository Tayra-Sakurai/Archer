// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Contexts
{
    public class CaesarContext : DbContext
    {
        public DbSet<LargeCategory> LargeCategories { get; set; }
        public DbSet<MediumCategory> MediumCategories { get; set; }
        public DbSet<SmallCategory> SmallCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public CaesarContext(DbContextOptions<CaesarContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LargeCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<MediumCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<SmallCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<Item>()
                .Property(e => e.TimeTrade)
                .HasDefaultValueSql("datetime('now') || 'Z'");
            modelBuilder.Entity<PaymentMethod>();
        }
    }
}
