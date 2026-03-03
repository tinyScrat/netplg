namespace Lpc.Infrastructure.Persistence.EntityConfigs;

using System.Linq.Expressions;
using Lpc.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal abstract class EntityTypeConfigBase<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase<TId>
    where TId : EntityId<TId>
{
    protected virtual string TableName => typeof(TEntity).Name;

    protected abstract Expression<Func<TEntity, object?>> KeyExpression { get; }

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(KeyExpression);

        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
