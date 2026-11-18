---
trigger: always_on
---

# .NET Performance Rules

- Prefer correct, maintainable code over speculative optimization.
- Optimize only real bottlenecks, expensive queries, high-traffic paths, or known scalability issues.
- Reuse existing performance patterns before introducing new infrastructure.

## Async I/O

- Use async APIs for DB, HTTP, files, and Redis.
- Never use `.Result`, `.Wait()`, `Thread.Sleep()`, or synchronous I/O in request paths.
- Propagate CancellationToken.
- Never use unsafe fire-and-forget work for request-critical operations.

## EF Core

For read-only queries:
- Use AsNoTracking().
- Project only required fields.
- Prefer DTO projection over loading entities and mapping afterward.
- Avoid unnecessary Include(), navigation graphs, and tracking.

Avoid:
- N+1 queries
- Query-per-item loops
- Repeated database calls
- Loading large datasets into memory
- Client-side pagination/filtering

Prefer:
- Projection
- Single/batched queries
- Efficient joins
- Database-side filtering/sorting/pagination

## Pagination

- Never return unbounded collections from high-volume data.
- Enforce default and maximum page sizes.
- Use stable ordering.
- Paginate in the database.
- Support appropriate indexes for frequent pagination/filter/sort patterns.

## Memory

Avoid:
- Huge collections
- Large in-memory transformations
- Unnecessary `ToList()`
- Duplicate object graphs
- Loading unused columns, binary data, or large text

Prefer streaming/pagination when appropriate.

## Redis / Caching

- Use Redis only when caching has a real benefit.
- Define consistent cache keys, scope, TTL, and invalidation.
- Include relevant user/tenant/resource/version scope.
- Avoid unbounded cache entries.
- Invalidate or update affected cache entries after successful writes.
- When cache is not required for correctness, Redis failures should degrade gracefully.

## SignalR

- Keep payloads small, DTO-based, and relevant.
- Do not broadcast unnecessarily.
- Use server-verified groups.
- Never send full persistence entities unless explicitly required.

## Serialization

- Return explicit DTOs.
- Avoid large graphs, unnecessary properties, and metadata.
- Prevent oversized responses and circular reference problems.

## External HTTP

- Use the project's IHttpClientFactory pattern.
- Reuse HttpClient.
- Propagate CancellationToken.
- Configure appropriate timeouts.
- Follow existing resilience/retry conventions.
- Do not add custom retry systems without a real need.

## Background Work

- Do not use unsafe fire-and-forget tasks inside request handlers.
- Use the project's established background-processing mechanism for work that must survive the request.
- Ensure failure handling, observability, cancellation, and graceful shutdown.

## Indexes

Before adding an index:
- Understand the query.
- Check existing indexes.
- Verify filtering/sorting usage.
- Consider write/storage overhead.

Do not index every column.

## Logging

Use structured logs without hurting performance.

Avoid:
- Huge objects
- Full request/response bodies unless necessary
- Excessive logs inside loops
- Sensitive data

## Verification

For performance-sensitive changes:
- Inspect generated SQL.
- Check query count.
- Verify pagination.
- Check payload size and memory behavior.
- Verify cache behavior.
- Run relevant tests/benchmarks when available.

Do not claim a performance improvement without evidence.