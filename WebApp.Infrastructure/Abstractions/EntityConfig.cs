namespace WebApp.Infrastructure.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;

#nullable enable

/// <summary>
/// Base class for configuring Entities with Value Objects
/// </summary>
public abstract class EntityConfig<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    /// <summary>
    /// Configure an embedded Value Object (OwnsOne)
    /// </summary>
    protected void ConfigureOwned<TEntityVO>(
        EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TEntityVO?>> voExpression,
        Action<OwnedNavigationBuilder<TEntity, TEntityVO>>? configureAction = null)
        where TEntityVO : class
    {
        var ownedBuilder = builder.OwnsOne(voExpression);
        configureAction?.Invoke(ownedBuilder);
    }

    /// <summary>
    /// Configure a collection Value Object (OwnsMany) with shadow Id
    /// </summary>
    protected void ConfigureOwnedCollection<TEntityVO>(
        EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IEnumerable<TEntityVO>?>> collectionExpression,
        Action<OwnedNavigationBuilder<TEntity, TEntityVO>>? configureAction = null)
        where TEntityVO : class
    {
        var ownedCollection = builder.OwnsMany(collectionExpression);

        // Shadow primary key for EF Core
        ownedCollection.Property<int>("Id");
        ownedCollection.HasKey("Id");

        configureAction?.Invoke(ownedCollection);
    }
}
