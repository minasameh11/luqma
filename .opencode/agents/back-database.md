---
trigger: always_on
---

# .NET Database Rules

- Assume EF Core + PostgreSQL unless the existing project uses something else.
- Before persistence changes, inspect DbContext, configurations, migrations, repositories/queries, and similar code. Reuse existing patterns.

## EF Core

- Keep EF Core implementation in Infrastructure.
- Use IEntityTypeConfiguration<T>.
- Configure relationships, keys, constraints, and justified indexes explicitly.
- Use migrations for every schema change.
- Do not put EF configuration in Domain unless the existing architecture requires it.

## Migrations

- Never replace migrations with manual production schema changes.
- Do not casually delete or rewrite historical migrations.
- Review generated migrations before committing.
- Verify the migration matches the intended schema and rollback when relevant.

## Queries

- Use AsNoTracking() for read-only queries.
- Project only required fields.
- Avoid unnecessary Include(), entity graphs, repeated queries, and N+1 patterns.
- Do not load large datasets only to filter/paginate in memory.
- Prefer database-side filtering, sorting, projection, and pagination.

## Relationships & Integrity

- Configure foreign keys and delete behavior explicitly.
- Enforce critical invariants with database constraints where appropriate:
  unique, FK, required, check.
- Application validation is still required for user-friendly errors.

## Pagination / Filtering / Sorting

- Paginate collection queries unless the dataset is intentionally small.
- Enforce default and maximum page sizes.
- Validate supported filter and sort fields.
- Never pass arbitrary client property names into dynamic queries.
- Never allow unbounded result sets.

## Indexes

- Add indexes only for real query patterns or integrity requirements.
- Consider foreign keys, unique identifiers, frequent filters/sorts, and justified composite indexes.
- Do not create indexes blindly.

## Transactions

- Use transactions when multiple writes must succeed/fail atomically.
- Examples: order + items, payment + order state, inventory + order.
- Do not add manual transactions around every simple operation.

## Concurrency

- Follow the project's optimistic concurrency strategy.
- Detect and handle concurrency conflicts using the project's standard response.
- Never silently overwrite concurrent changes.

## PostgreSQL

- Keep PostgreSQL-specific types, SQL, indexes, JSON/JSONB, extensions, and provider behavior in Infrastructure/migrations.
- Do not leak provider-specific details into Domain.

## Performance

Prefer:
- Projection
- AsNoTracking()
- Pagination
- Proper indexes
- Batching
- Efficient joins

Avoid:
- N+1 queries
- Huge entity graphs
- Unnecessary tracking
- Repeated database calls
- Unbounded queries

Do not add complex database optimizations without a real need.

## Boundaries

- Never expose DbContext, EF entities, IQueryable, or persistence models directly from the API.
- Persistence models are not API contracts.

## Verification

After database changes:
- Build the affected project.
- Generate/review migrations when required.
- Run relevant tests.
- Review important SQL/migration operations when practical.
- Verify relationships, constraints, query behavior, and affected endpoint performance.