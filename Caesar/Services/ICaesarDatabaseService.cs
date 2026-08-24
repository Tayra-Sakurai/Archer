// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.Services
{
    /// <summary>
    /// The basic interface for database retrieval and modification.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="DbContext"/> inherited class to be used as the gateway.</typeparam>
    public interface ICaesarDatabaseService<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Retreives all entities in the table designated by the selector.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="selector">The selector to get the entity.</param>
        /// <returns>The task to control the asynchronous process. Finally returns the collection of <typeparamref name="TEntity"/>.</returns>
        Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>(Expression<Func<TContext, DbSet<TEntity>>> selector)
            where TEntity : class;

        /// <summary>
        /// Retrieves all entities which matches the type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>The task to return the collection of the entities.</returns>
        Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Loads the related entities asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelated">The related entity's type.</typeparam>
        /// <param name="entity">The entity to be refered.</param>
        /// <param name="expression">The selector of the related entities.</param>
        /// <returns>The <paramref name="entity"/> itself.</returns>
        Task<TEntity> LoadRelatedEntitiesAsync<TEntity, TRelated>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelated>>> expression)
            where TEntity : class
            where TRelated : class;

        /// <summary>
        /// Loads the parent entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelated">The parent entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The parent entity selector function.</param>
        /// <returns>The entity itself.</returns>
        Task<TEntity> LoadRelatedEntityAsync<TEntity, TRelated>(TEntity entity, Expression<Func<TEntity, TRelated?>> expression)
            where TEntity: class
            where TRelated : class;

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The task instance to control the asynchronous operation.</returns>
        Task UpdateEntityAsync<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The task to control the asynchronous operation.</returns>
        Task AddEntityAsync<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Removes the selected entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity to be removed.</param>
        /// <returns>The task to control the asynchronous process.</returns>
        Task RemoveEntityAsync<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Returns if any entity of the property exists.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The entity set selector.</param>
        /// <returns>Whether the type of entity exists.</returns>
        bool ExistsAnyEntity<TEntity>(Expression<Func<TContext, DbSet<TEntity>>> expression)
            where TEntity : class;

        /// <inheritdoc cref="ExistsAnyEntity{TEntity}(Expression{Func{TContext, DbSet{TEntity}}})"/>
        bool ExistsAnyEntity<TEntity>()
            where TEntity : class;
    }
}
