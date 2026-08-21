// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.Services
{
    public class WindowsCaesarDatabaseService : ICaesarDatabaseService<CaesarContext>
    {
        private readonly IDbContextFactory<CaesarContext> factory;

        public WindowsCaesarDatabaseService(IDbContextFactory<CaesarContext> factory)
        {
            this.factory = factory;
        }

        public async Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>(Expression<Func<CaesarContext, DbSet<TEntity>>> expression)
            where TEntity : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();

            return await expression.Compile()(context).ToListAsync();
        }

        public async Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>()
            where TEntity : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();

            return new HashSet<TEntity>(context.Set<TEntity>());
        }

        public async Task<TEntity> LoadRelatedEntitiesAsync<TEntity, TRelated>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelated>>> expression)
            where TEntity : class
            where TRelated : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();

            EntityEntry<TEntity> entityEntry = context.Attach(entity);

            await entityEntry
                .Collection(expression)
                .LoadAsync();

            return entity;
        }

        public async Task<TEntity> LoadRelatedEntityAsync<TEntity, TRelated>(TEntity entity, Expression<Func<TEntity, TRelated?>> expression)
            where TEntity : class
            where TRelated : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();

            EntityEntry<TEntity> entityEntry = context.Attach(entity);
            await entityEntry
                .Reference(expression)
                .LoadAsync();

            return entity;
        }

        public async Task UpdateEntityAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();

            context.Update(entity);

            await context.SaveChangesAsync();
        }

        public async Task AddEntityAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();
            context.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveEntityAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using CaesarContext context = await factory.CreateDbContextAsync();
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
