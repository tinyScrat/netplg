# Mailing Service — Specification & Implementation Plan

Current Date: 2026-02-13

## Goal
Implementing a resilient mailing service that:
- Consumes `SendEmail` messages from RabbitMQ,
- Persists incoming messages,
- Schedules and dispatches workers to send emails (with or without attachments),
- Archives sent messages as compressed MIME blobs,
- Ensures idempotency on retryable steps,
- Limits concurrency to avoid throttling by Microsoft Graph API.

---

## High-level end-to-end flow

1. Ingest
   - Consumer receives a `SendEmail` payload from a RabbitMQ queue.
   - Persist the payload to `incoming_emails`.
   - Publish a `SendEmailReceived` acknowledgment back to the publisher (including the persisted row id).
   - Entire ingest step must be idempotent (no duplicate rows or duplicate acks on repeated delivery).

2. Scheduling
   - Scheduler polls the `incoming_emails` table for unprocessed messages.
   - Scheduler locks/marks rows as "dispatched" to avoid duplicate dispatches.
   - Route messages based on presence of attachments:
     - `send-with-attachments` worker pool for messages that include attachments.
     - `send-no-attachments` worker pool for messages without attachments.
   - Scheduler enforces concurrency/rate limits per worker type (configurable).

3. Send-email workers (two kinds)
   - Worker receives a dispatched message and must echo a receipt ack back to the scheduler and persist that fact (idempotent).
   - With attachments:
     1. Create an email draft (Graph API).
     2. Create upload session(s); upload attachments; persist per-file upload status.
     3. Send the draft (Graph API).
     4. Dispatch the Graph message ID to email backup worker (queue).
   - Without attachments:
     1. Send email directly (Graph API).
     2. Dispatch the Graph message ID to email backup worker.

4. Email backup worker
   - Gets a Graph message ID.
   - Fetches the email in MIME format from Graph API.
   - Compresses (e.g., gzip) MIME content.
   - Stores compressed blob in `email_backups` with metadata.
   - Must be idempotent: repeated backups for same Graph message ID should not create duplicates.

5. Error handling & retries
   - Retry Graph API calls with exponential backoff + jitter for transient errors (429, 503).
   - Respect `Retry-After` header when present.
   - Retries must respect global concurrency/rate limits so backoff does not cause spikes.
   - Use DLQ for permanently failed items after configurable max attempts.

6. Observability & security
   - Emit metrics, structured logs, and tracing correlation IDs.
   - Encrypt backups/attachments at rest and implement retention TTL.
   - Store secrets in a secret store and rotate regularly.

---

## Suggested data model (tables)

- incoming_emails
  - id (UUID) — primary key
  - publisher_message_id (string) — unique index for idempotency
  - payload (json)
  - status (enum: received, dispatched, processing, processed, failed)
  - created_at, updated_at, attempt_count, last_error

- file_uploads
  - id, incoming_email_id, filename, size, upload_session_info (json)
  - uploaded_bytes, status

- sent_emails
  - id (UUID), incoming_email_id, graph_message_id, sent_at, status, response_metadata

- email_backups
  - id (UUID), graph_message_id (unique), incoming_email_id, compressed_blob (bytea/blob)
  - compression, size, checksum, created_at

- dlq
  - id, incoming_email_id, reason, attempts, last_error, created_at

---

## Idempotency mechanics (recommendations)

- Use `publisher_message_id` (must be provided by publisher) as dedupe key on ingest. If missing, compute a deterministic hash of canonicalized payload.
- Enforce `UNIQUE` constraint on `incoming_emails.publisher_message_id`.
- Workers persist receipt records (e.g., `worker_receipts`) before performing irreversible actions; use unique receipt keys to ignore duplicates on retries.
- Prefer idempotent Graph APIs or client-generated IDs where available, and persist Graph IDs to check before re-sending.

---

## Concurrency & rate limiting

- Scheduler maintains worker pools with configurable max concurrent workers per pool.
- Implement a token-bucket or leaky-bucket rate limiter to protect Graph API calls.
- Consider a central outbound API call queue that enforces global Graph rate limits (shared across workers).

---

## RabbitMQ delivery & acknowledgements

- Use manual ACK from the ingest consumer:
  - ACK only after persisting to `incoming_emails` and publishing `SendEmailReceived`.
  - If consumer crashes before ACK, message redelivers; idempotency prevents double persistence.
- `SendEmailReceived` should include `incoming_email_id` and `publisher_message_id`.

---

## Send-email worker flow details

- Worker receipt ACK:
  - After receiving a dispatch, worker persists a receipt record and sends a receipt event to the scheduler. This step must be idempotent.

- With attachments:
  1. Create draft with Graph API (persist draft id).
  2. Create upload session(s) for each file and upload with retries; persist per-file progress.
  3. After uploads complete, send the draft.
  4. Persist `sent_emails` row and enqueue backup task with `graph_message_id`.

- Without attachments:
  1. Send email directly via Graph API.
  2. Persist `sent_emails` and enqueue backup task.

---

## Email backup worker flow

1. Consume `graph_message_id`.
2. Fetch MIME from Graph API (retry on transient errors).
3. Compress MIME (gzip recommended).
4. Store compressed blob in `email_backups` with metadata (and encryption).
5. Ensure unique constraint on `graph_message_id` so duplicates are idempotently ignored.

---

## Error handling & DLQ

- Configure consistent retry policy with exponential backoff + jitter.
- Honor `Retry-After` header for Graph 429/503 responses.
- After max attempts, place item into DLQ and alert/notify.
- Track attempt_count and last_error for visibility.

---

## Observability & monitoring

- Metrics to expose:
  - RabbitMQ queue depth, incoming_emails pending count
  - Scheduler dispatch rate and concurrency usage
  - Worker processing latency and success/failure rates
  - Graph API 429/503 counts and retry rates
  - Upload progress metrics and backup success rate

- Logging:
  - Structured logs with: publisher_message_id, incoming_email_id, graph_message_id, correlation_id, trace_id
- Tracing:
  - Correlate full flow: publish → ingest → scheduler → worker → Graph → backup

---

## Security & retention

- Encrypt backups at rest.
- Implement backup TTL/retention and deletion processes.
- Store Graph credentials and other secrets in a secret store (e.g., HashiCorp Vault, cloud KMS).
- Follow least-privilege for Graph API credentials and rotate periodically.

---

## Example message & event payloads

- SendEmail (publisher)
```json
{
  "publisher_message_id": "string",
  "from": "sender@example.com",
  "to": ["recipient@example.com"],
  "cc": ["cc@example.com"],
  "subject": "Subject text",
  "body": { "contentType": "html", "content": "<p>...</p>" },
  "attachments": [
    { "name": "file.pdf", "size": 12345, "contentType": "application/pdf", "urlOrId": "..." }
  ],
  "metadata": { "tenant": "..." }
}
```

- SendEmailReceived (ack)
```json
{
  "publisher_message_id": "string",
  "incoming_email_id": "uuid",
  "received_at": "2026-02-13T12:34:56Z",
  "status": "received"
}
```

- Worker dispatch
```json
{
  "incoming_email_id": "uuid",
  "publisher_message_id": "string",
  "route": "attachment"|"no-attachment",
  "attempt": 1
}
```

---

## Acceptance Criteria (Combined)

- The service ingests `SendEmail` from RabbitMQ, persists it, and publishes `SendEmailReceived` — duplicate deliveries do not create duplicate rows nor duplicate acks.
- Scheduler routes messages to correct worker pool by attachments and enforces concurrency/rate limits to avoid Graph throttling.
- Workers send emails (with/without attachments), persist sent metadata, and enqueue backups. Worker ack/persist steps are idempotent.
- Backup worker fetches MIME, compresses, and persists one backup per Graph message id.
- Retries, backoff, DLQ, metrics, logging, encryption, and retention are implemented.
- Integration tests simulate 429/503 and verify retries obey `Retry-After` and global rate limits.

---

## Subtasks (breakdown for implementation)

1. Schema + Migrations
   - Create tables: `incoming_emails`, `file_uploads`, `sent_emails`, `email_backups`, `dlq`.
   - Add unique index on `publisher_message_id`.
   - AC: DB migrations run successfully in dev and CI; schema matches model.

2. Message ingestion component
   - RabbitMQ consumer for `SendEmail` queue.
   - Persist incoming message idempotently.
   - Publish `SendEmailReceived` ack.
   - AC: Redelivery doesn't create duplicates.

3. Scheduler
   - Poll & lock `incoming_emails`, route by attachments, mark dispatched, and enqueue to worker queues.
   - Enforce concurrency limits (configurable).
   - AC: Scheduler never runs more than configured concurrent workers per type.

4. Send-email worker (no-attachment)
   - Persist receipt, send via Graph API, persist `sent_emails`, enqueue backup.
   - Implement retry/backoff & `Retry-After`.
   - AC: Single send produces `sent_emails` & backup enqueued; re-dispatch safe.

5. Send-email worker (with-attachment)
   - Persist receipt, draft creation, upload sessions, file uploads persistence, send draft, persist `sent_emails`, enqueue backup.
   - AC: Idempotent file tracking and single final send.

6. Email backup worker
   - Fetch MIME, compress, store in `email_backups`.
   - AC: One backup per Graph message id.

7. Retry/DLQ and backoff
   - Unified retry policies, DLQ handling, and alerts for DLQ items.
   - AC: Transient failures retried; permanent failures go to DLQ.

8. Rate limiter & throttling protection
   - Implement token-bucket/leaky-bucket limiter or central outbound queue.
   - AC: Outbound Graph call rate respects configured limits.

9. Observability & tracing
   - Structured logging, tracing spans and metrics (Prometheus or similar).
   - AC: Metrics and traces available for sample flows.

10. Security & retention
    - Secrets in secret store, backups encrypted, retention/TTL implemented.
    - AC: Backups encrypted and aged per policy.

11. Tests & CI
    - Unit tests for idempotency, scheduler routing and rate limiter.
    - Integration tests with fake Graph API (simulate 429/503).
    - AC: CI runs tests and validates throttling behavior.

12. Deployment & runbooks
    - Helm/docker-compose manifests, env configs, runbooks for incident response.
    - AC: Deployable to staging and runbook documented.

---

## Implementation notes & recommendations

- Use `publisher_message_id` provided by publisher; document it as required field.
- If publisher cannot provide an id, use a deterministic hash of canonicalized payload (document caveats).
- Persist receipts/locks with short TTLs to allow recovery if worker crashes mid-work.
- Prefer a central outbound request queue for Graph to simplify global throttling across multiple worker instances.
- For very large attachments, use chunked upload sessions and persist chunk progress.
- Consider horizontal scaling of scheduler with a distributed lock (DB row lock, advisory lock, or Redis lock).
- Provide a management API or dynamic config to tune concurrency/rate-limits at runtime.

---

## Next steps (recommended implementation order)

1. Implement DB schema + migrations (subtask 1).
2. Implement RabbitMQ ingestion + idempotent persist + `SendEmailReceived` ack (subtask 2).
3. Implement basic scheduler that routes by attachments and dispatches (subtask 3).
4. Implement no-attachment worker (subtask 4) and backup worker (subtask 6) to validate a minimal end-to-end flow.
5. Add rate-limiter and attachment worker (subtask 5 & 8).
6. Add observability, security, tests, CI, and deployment artifacts.

---

If you want, I can now:
- Generate DB migration SQL for the schema,
- Draft the RabbitMQ consumer/scheduler/worker interfaces and message schemas,
- Generate code skeletons (language of your choice) for scheduler and workers,
- Create integration test scenarios including fake Graph API responses (429/503).

Tell me which artifact you want first.
