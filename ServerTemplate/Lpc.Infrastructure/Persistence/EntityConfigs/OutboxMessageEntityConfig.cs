namespace Lpc.Infrastructure.Persistence.EntityConfigs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lpc.Infrastructure.Persistence;

internal sealed class OutboxMessageEntityConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(nameof(OutboxMessage));

        builder.HasKey(m => m.Id);
    }
}
