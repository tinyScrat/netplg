# EF Core Mapping – Strongly Typed `EntityId<T>` Records

This document describes the **team-standard way** to map strongly typed entity IDs implemented as `record` types (e.g. `OrderId`, `CustomerId`) to EF Core.

The pattern assumes:

```csharp
public abstract record EntityId<TDerived> : IEntityIdentity
    where TDerived : EntityId<TDerived>
{
    protected internal Guid Value { get; }

    protected abstract string KeyPrefix { get; }

    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
            throw new EmptyValueException($"{GetType().Name} cannot be empty");

        Value = value;
    }

    public string Key => string.Create(
        CultureInfo.InvariantCulture,
        $"{KeyPrefix}_{Value:N}");

    public override string ToString() => Key;
}
```

---

## Core Principles

1. **Persist the primitive, not the wrapper**  
   EF Core stores the underlying `Guid`, not the `EntityId` itself.

2. **The domain model owns identity semantics**  
   EF Core is responsible only for persistence, never for validation or invariants.

3. **No shadow IDs, no string keys**  
   IDs are `Guid`-backed in the database for performance and indexing.

4. **Explicit mapping is required**  
   EF Core does not natively understand strongly typed IDs.

---

## Recommended Mapping Strategy (ValueConverter)

### Example Domain ID

```csharp
public sealed record OrderId : EntityId<OrderId>
{
    protected override string KeyPrefix => "ORD";

    public OrderId(Guid value) : base(value) { }

    public static OrderId New() => new(Guid.NewGuid());
}
```

---

## Entity Example

```csharp
public class Order
{
    public OrderId Id { get; private set; }

    private Order() { } // EF Core

    public Order(OrderId id)
    {
        Id = id;
    }
}
```

---

## EF Core Configuration

### ValueConverter

```csharp
public sealed class OrderIdConverter
    : ValueConverter<OrderId, Guid>
{
    public OrderIdConverter()
        : base(
            id => id.Value,
            value => new OrderId(value))
    {
    }
}
```

---

### EntityTypeConfiguration

```csharp
public sealed class OrderEntityTypeConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(new OrderIdConverter())
            .ValueGeneratedNever()
            .IsRequired();
    }
}
```

---

## Database Result

```sql
CREATE TABLE Orders (
    Id uniqueidentifier NOT NULL PRIMARY KEY
)
```

✔ Database stores `uniqueidentifier`  
✔ Domain uses `OrderId`  
✔ No string parsing or prefixes at persistence level

---

## Applying the Pattern at Scale

### Generic Converter Base (Optional)

```csharp
public abstract class EntityIdConverter<TId>
    : ValueConverter<TId, Guid>
    where TId : EntityId<TId>
{
    protected EntityIdConverter(Func<Guid, TId> factory)
        : base(id => id.Value, value => factory(value))
    {
    }
}
```

```csharp
public sealed class CustomerIdConverter
    : EntityIdConverter<CustomerId>
{
    public CustomerIdConverter()
        : base(value => new CustomerId(value)) { }
}
```

---

## Why We Do NOT Use Owned Types

EF Core owned entities are **not recommended** for entity IDs because:

- They add unnecessary joins or column nesting
- They complicate key mapping
- They obscure database schema
- They do not add domain value

IDs are identifiers, not conceptual aggregates.

---

## JSON Serialization (FYI)

By default, `System.Text.Json` will fail to deserialize `OrderId`.

Recommended approach:

```csharp
public sealed class OrderIdJsonConverter
    : JsonConverter<OrderId>
{
    public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(Guid.Parse(reader.GetString()!));

    public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
```

---

## Anti-Patterns (Do NOT Do This)

❌ Persist `Key` (string) to database  
❌ Use `string` as primary key  
❌ Reconstruct IDs via reflection  
❌ Put EF attributes in domain models

---

## Summary

| Concern | Decision |
|------|---------|
| Domain ID type | `record EntityId<T>` |
| DB storage | `Guid` |
| EF mapping | `ValueConverter` |
| Prefix usage | Domain / API only |
| Validation | Constructor |

This pattern provides:
- Strong typing in domain
- Clean persistence model
- Excellent performance
- Minimal EF Core friction

---

**This document is authoritative.**

All new entities must follow this mapping pattern unless explicitly approved by the architecture group.

