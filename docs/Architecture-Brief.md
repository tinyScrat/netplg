# Architecture Brief

## 1. Purpose

This document defines the **current architectural direction** of the system. It is intended to:
- Lock in shared understanding
- Reduce design churn
- Distinguish **decisions** from **open explorations**

This is a *living document*, but changes should be deliberate.

---

## 2. System Goals

The system prioritizes:
- Correctness of business processes
- Explicit modeling of domain behavior
- Strong consistency on writes
- Clear separation of concerns
- Long-term maintainability over short-term velocity

This is **not** a CRUD-first system.

---

## 3. Core Architectural Style

### 3.1 Domain-Driven Design (DDD)

- The domain model is the core of the system.
- Aggregates are responsible for:
  - enforcing invariants
  - managing state transitions
  - emitting domain events
- Not all entities are aggregates.
- Domain events exist only at aggregate boundaries.

---

### 3.2 Command-Oriented Write Model

All state changes occur through **commands**.

- Commands represent *intent*.
- Each command is handled by a single execution flow.
- Write logic is not embedded in controllers or infrastructure.

The write side is modeled as:

```
Command → Effect / Handler → Aggregate → Domain Events → Persistence
```

---

## 4. Effect / Handler Model

### 4.1 Effects

Effects are responsible for **executing commands**.

- Effects consume commands
- Effects do not return domain objects
- Effects may:
  - load aggregates
  - invoke domain logic
  - coordinate persistence

Generic variance is intentional:

```csharp
public interface IEffect<in TCommand>
{
    IObservable<Unit> Handle(TCommand command);
}
```

This allows substitution of generalized handlers for specific commands.

---

### 4.2 Reactive Execution

- Command handling is asynchronous and reactive.
- `IObservable` is used for:
  - cancellation
  - retries
  - unified error handling
  - composition

Reactive pipelines are treated as **execution infrastructure**, not domain logic.

---

## 5. Layered Architecture

### 5.1 API Layer

Responsibilities:
- Transport concerns (HTTP)
- DTO shape validation
- FluentValidation for user-defined rules
- Consistent error responses (400, 401, etc.)

API layer **does not**:
- contain domain logic
- access EF Core directly

---

### 5.2 Application Layer

Responsibilities:
- Command and query orchestration
- Effect composition
- Transaction boundaries (conceptually)

Application layer:
- defines interfaces
- does not depend on EF Core
- may depend on abstractions (repositories, query services)

---

### 5.3 Infrastructure Layer

Responsibilities:
- Persistence (EF Core, Dapper)
- Outbox pattern
- Query projections / read models

Infrastructure implements application abstractions.

---

## 6. Query Model

- Queries are separated from commands (CQRS-style)
- Queries:
  - do not load aggregates
  - may use Dapper or read-optimized schemas
- Query handlers live outside the domain model

Strong consistency is **not required** for queries.

---

## 7. Consistency & Concurrency

- Writes use optimistic concurrency
- Idempotency is explicitly designed for create operations
- Each command execution is treated as a single logical transaction

---

## 8. Domain Events & Integration

- Aggregates emit domain events
- Events are persisted atomically with state changes
- Outbox pattern is used for external publication

Events are facts, not commands.

---

## 9. Actor Model (Exploratory)

The system is actively evaluating:

- Traditional aggregate + Unit of Work
vs
- Aggregate-as-Actor execution model

Open questions:
- Should aggregate message handlers equal command handlers?
- Where should persistence and event dispatch occur?
- Which state changes *must* go through the aggregate?

This area is **not yet locked**.

---

## 10. Non-Goals

The system explicitly avoids:
- Anemic domain models
- Repository-per-entity patterns
- EF Core leaking into application logic
- Implicit business rules hidden in infrastructure

---

## 11. Decision Status

| Area | Status |
|---|---|
| DDD Core | Locked |
| Command-only writes | Locked |
| Reactive execution | Locked |
| CQRS read model | Locked |
| Actor model | Exploratory |

---

## 12. Guiding Principle

> Make business processes explicit, observable, and hard to misuse — even if that costs convenience.

---

## 13. Aggregate Invariants

Aggregates are responsible for enforcing **business invariants**. An invariant is any rule that must *always* hold true immediately after a command completes.

Examples:
- An item cannot be approved unless it is in a submitted state
- A released change cannot be modified
- A transition cannot skip required intermediate states

Rules:
- Invariants live **inside aggregates**
- Invariants are checked synchronously during command handling
- Violations result in command failure, not partial updates

---

## 14. What Must Go Through an Aggregate

The following operations **must** be executed via an aggregate:

- State transitions (e.g. Draft → Submitted → Approved → Released)
- Any logic that depends on current aggregate state
- Any operation that emits domain events
- Any rule that coordinates multiple fields to stay consistent

The following operations **may bypass aggregates**:

- Read-only queries
- Projections / denormalized read models
- Purely technical updates that do not affect business meaning

---

## 15. Command Lifecycle

Each command follows a well-defined lifecycle:

```
Receive → Validate → Execute → Persist → Publish → Complete
```

### 15.1 Receive
- Command is received via API or internal message
- Transport concerns are handled

### 15.2 Validate
- DTO-level validation occurs in the API layer
- Semantic validation may occur during execution

### 15.3 Execute
- Effect / handler loads required aggregate(s)
- Aggregate methods are invoked
- Domain events are produced

### 15.4 Persist
- Aggregate state and events are persisted atomically
- Optimistic concurrency is enforced

### 15.5 Publish
- Domain events are written to the outbox
- External publication is asynchronous

### 15.6 Complete
- Command completes successfully or fails atomically

---

## 16. Command Failure Semantics

- Commands fail fast on invariant violations
- Partial state updates are not allowed
- Failures do not emit domain events

Failures are considered part of normal control flow, not exceptional system errors.

---

## 17. Transaction Boundary

A single command execution defines a **logical transaction boundary**:

- All aggregate mutations
- All domain events
- All version updates

must succeed or fail together.

---

## 18. Consistency Guarantees

The system guarantees:
- Strong consistency within an aggregate
- Eventual consistency across aggregates and read models

Cross-aggregate coordination must occur via:
- domain events
- process managers / sagas (if introduced)

