namespace MyApp.Application.Abstractions;

public interface IIdempotencyService
{
    bool IsProcessed(string key);
    void MarkProcessed(string key);
    bool TryGetResult<T>(string key, out T result);
    void StoreResult<T>(string key, T result);
}

// Simple in-memory implementation (for demo/testing)
// public class InMemoryIdempotencyService : IIdempotencyService
// {
//     private readonly HashSet<string> _processed = new();

//     public bool IsProcessed(string key) => _processed.Contains(key);

//     public void MarkProcessed(string key)
//     {
//         lock (_processed)
//         {
//             _processed.Add(key);
//         }
//     }
// }


// typical Application layer components besides commands/queries and their handlers:

// Effects / services → orchestrate domain logic, call aggregates, enforce rules

// DTOs → input/output shapes for API

// Pipelines / middleware → validation, logging, authorization, retries, idempotency

// Process managers / sagas → coordinate multi-aggregate workflows

// Unit of Work / transaction boundaries → ensure atomicity across operations

// Domain event dispatchers → publish events after aggregates change
