# Value Object (VO) Coding Standard

**Applies to:** Domain layer (DDD)

**Purpose:** Provide a consistent, safe, and expressive way to model Value Objects across the codebase.

---

## 1. Definition
A **Value Object (VO)**:
- Has **no identity**
- Is **immutable**
- Is compared **by value**, not reference
- Encapsulates **domain invariants**

---

## 2. Default Choice (MANDATORY)

> **All Value Objects MUST be implemented as `sealed record`.**

No base `ValueObject` class. No marker interfaces.

---

## 3. Canonical VO Template

```csharp
public sealed record Description
{
    public const int MaxLength = 256;
    public string Value { get; }

    public Description(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Description is required");

        if (value.Length > MaxLength)
            throw new DomainException("Description too long");

        Value = value;
    }

    public override string ToString() => Value;
}
```

### Required characteristics
- `sealed`
- Explicit constructor
- Private set / read-only properties
- Invariants enforced in constructor

---

## 4. What NOT to Do (BANNED)

❌ Generic constraint types
```csharp
String256
NonEmptyString
```

❌ Primitive fields in domain entities
```csharp
public string Description;
```

❌ Public setters
```csharp
public string Value { get; set; }
```

❌ EF Core / JSON attributes inside domain
```csharp
[MaxLength(256)]
```

❌ Marker interfaces
```csharp
interface IValueObject {}
```

---

## 5. Categories of Value Objects

### 5.1 Text-based
Used for domain concepts like Description, Remark, Name.

```csharp
public sealed record Remark
{
    public string? Value { get; }

    public Remark(string? value)
    {
        if (value?.Length > 256)
            throw new DomainException("Remark too long");
        Value = value;
    }
}
```

---

### 5.2 Strongly Typed IDs

Allowed to use a generic base **only** for identifiers.

```csharp
public abstract record StronglyTypedId<T>(T Value)
{
    public override string ToString() => Value!.ToString()!;
}

public sealed record ProductId(Guid Value)
    : StronglyTypedId<Guid>(Value);
```

---

### 5.3 Behavior-rich VOs (Money, Range, Quantity)

```csharp
public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }
}
```

---

## 6. Guard Helpers (OPTIONAL)

Shared validation logic MAY be extracted into internal helpers.

```csharp
internal static class Guard
{
    public static string NotEmpty(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{name} is required");
        return value;
    }
}
```

**Rule:** Guards share mechanics, not meaning.

---

## 7. Domain Exceptions (MANDATORY)

Domain invariants MUST throw `DomainException`.

```csharp
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) {}
}
```

Do NOT throw:
- `ArgumentException`
- `InvalidOperationException`

---

## 8. Layer Boundaries

### API / DTO Layer
- Uses primitives
- Performs format validation

```csharp
public record CreateProductDto(string Description);
```

### Application Layer
- Converts primitives → VOs

```csharp
var description = new Description(dto.Description);
```

### Domain Layer
- Accepts ONLY Value Objects

---

## 9. Persistence & Serialization

- EF Core mappings live in Infrastructure
- VOs are mapped as owned types
- JSON conversion happens at boundaries

**Domain layer must not reference EF Core or JSON libraries.**

---

## 10. Equality Rules

- Equality is value-based (provided by `record`)
- Two VOs with the same value MUST be equal

```csharp
new Description("A") == new Description("A") // true
```

---

## 11. Decision Checklist

Before creating a VO, ask:
- Is this a **domain concept**?
- Would a domain expert recognize this name?
- Does it have **invariants**?

If YES → create a VO.

---

## 12. Summary Rules (Pin This)

✔ Use `sealed record`
✔ Enforce invariants in constructor
✔ No generic constraint types
✔ No framework dependencies
✔ Convert at boundaries

---

## 13. Why We Do NOT Use a `ValueObject` Base Class

### Historical context
Older DDD implementations (pre–C# 9) commonly used an abstract `ValueObject` base class to implement:
- structural equality
- `GetHashCode`
- `==` / `!=` operators

This pattern was necessary **before `record` existed**.

---

### Modern C# reasoning
In modern C#:

- `record` already provides **correct value-based equality**
- Equality logic is **compiler-generated and refactor-safe**
- Manual equality implementations are easy to subtly break

A base class whose sole purpose is equality is therefore **redundant**.

---

### Domain modeling reasons

A `ValueObject` base class:
- Adds **no domain behavior**
- Represents a **technical concern**, not a business concept
- Forces **inheritance coupling** across the domain model

DDD treats *Value Object* as a **modeling concept**, not a type hierarchy.

---

### Design decision

> **Value Objects in this codebase are identified by their behavior and invariants, not by inheriting from a base class.**

Each Value Object:
- Is a `sealed record`
- Enforces its own invariants
- Is compared by value automatically

---

### Exception (rare and explicit)
A base class MAY be introduced only if it provides **real shared domain behavior**, such as:
- Strongly typed identifier infrastructure
- Common arithmetic or normalization logic

Pure equality helpers are NOT sufficient justification.

---

### Summary

- ❌ No `ValueObject` base class for tagging or equality
- ✅ Use `sealed record` instead
- ✅ Prefer clarity and compiler support over inheritance

---

**End of Standard**

