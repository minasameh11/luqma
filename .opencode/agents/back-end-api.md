---
trigger: always_on
---

# ASP.NET Core API Rules

- Inspect existing endpoints, routing, DTOs, responses, pagination, filtering, sorting, and versioning before adding or changing an endpoint. Reuse project conventions.

## Endpoints
- Keep controllers/endpoints thin: bind request -> call Application use case -> map result -> return HTTP response.
- No business logic, EF Core, Redis, complex workflows, or authorization decisions in controllers.

## Contracts
- Use explicit request/response DTOs.
- Never expose Domain, EF Core, Identity entities, database models, or internal exceptions.
- Follow existing mapping conventions; do not add mapping libraries without a real need.

## Responses
- Follow existing status-code and error conventions.
- Use centralized exception handling.
- Never expose stack traces, infrastructure details, or exception internals.

Common status codes:
- 200: success with response
- 201: created
- 204: success without body
- 400: invalid request/input
- 401: unauthenticated
- 403: forbidden
- 404: not found
- 409: conflict/concurrency
- 422: only if already used by the project

## Validation
- Validate input before executing the use case.
- Use the existing validation pipeline/library.
- Return field-level validation errors consistently.
- Do not rely on DB/EF exceptions for normal input validation.

## Routing & Versioning
- Follow existing route and API-versioning conventions.
- Do not introduce a new API style or bypass versioning.

## Queries
- Paginate collection endpoints by default.
- Enforce default and maximum page sizes.
- Validate supported filter fields/values.
- Allow-list or validate sort fields.
- Never pass arbitrary client strings into dynamic queries.

## Security Context
- Get user identity from authenticated server context.
- Never trust client-provided user IDs, tenant IDs, roles, or ownership data.
- Verify authorization and resource ownership for protected operations.

## Async & Cancellation
- Use async APIs end-to-end.
- Accept and propagate CancellationToken.
- Never use `.Result`, `.Wait()`, `Thread.Sleep()`, synchronous DB/network I/O, or unsafe fire-and-forget work.

## Middleware & Logging
- Keep cross-cutting concerns in existing middleware/pipelines.
- Use structured, non-sensitive logs.
- Never log tokens, passwords, secrets, authorization headers, or sensitive personal data.

## Response Consistency
- Preserve existing envelopes, pagination metadata, error format, property naming, null handling, date/time, and enum serialization.

## Completion
Before claiming completion:
- Verify validation, authorization, ownership, HTTP responses, error handling, cancellation, and DTOs.
- Build the affected project.
- Run relevant tests.
- Do not claim verification that was not performed.