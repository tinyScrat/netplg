namespace Lpc.Infrastructure.Persistence.EntityConfigs.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class ViewEntityConfig<TView> : IEntityTypeConfiguration<TView> where TView : class
{
    public virtual void Configure(EntityTypeBuilder<TView> builder)
    {
        builder.ToView(null);
        builder.HasNoKey();
    }
}
