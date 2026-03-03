namespace Lpc.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public const string MigrationHistoryTableName = "__MigrationHistory";
    public const string SchemaName = "Lpc";
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);

        // builder.ApplyConfiguration(new OutboxMessageEntityConfig());
    }
}
