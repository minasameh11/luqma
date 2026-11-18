---
trigger: always_on
---

# .NET Testing Rules

- Every behavior change needs appropriate verification.
- Test behavior, not line coverage.
- Before coding, inspect existing test projects, frameworks, fixtures, builders, factories, mocks, and database test infrastructure. Reuse existing conventions.

## Unit Tests

Prioritize isolated:
- Domain/business rules
- Application use cases
- Validation
- Authorization decisions
- Important mapping/logic

Tests must be deterministic and should not require unnecessary infrastructure.

## Integration Tests

Use integration tests for behavior involving:
- EF Core / PostgreSQL
- Authentication / Authorization
- API endpoints
- External integration boundaries

Prefer realistic application behavior over excessive mocking.

## API Tests

Cover important:
- Success/status/response
- Validation and boundary cases
- Authentication/authorization
- Ownership and tenant isolation
- Not found
- Conflict/concurrency
- Relevant external-service failures

## Security Tests

Every protected feature should verify:
- Anonymous user denied
- Authenticated unauthorized user denied
- Wrong owner denied
- Wrong tenant denied
- Authorized user allowed

## Validation Tests

Cover:
- Valid input
- Missing/empty values
- Length boundaries
- Numeric boundaries
- Invalid formats
- Invalid enum values

## Database Tests

For persistence-sensitive features verify:
- Relationships
- Constraints/unique indexes
- Filters
- Sorting
- Pagination
- Concurrency when relevant

Do not rely only on mocks for database behavior.

## Transactions

For atomic workflows, test failure scenarios and verify required rollback/no partial commit behavior.

## Test Data & Isolation

- Reuse existing builders, fixtures, and factories.
- Keep test data minimal and readable.
- Avoid duplicated setup.
- Tests must be independent of execution order.
- Avoid shared mutable state.
- Use the project's cleanup/isolation strategy.

## External Services

- Mock/fake external services for unit tests when appropriate.
- Use integration tests for important real contracts.
- Never depend on production credentials.

## Naming

Follow project conventions. Names should describe scenario and expected behavior.

Example:
`Should_Return_NotFound_When_Order_Does_Not_Exist`

## Regression Tests

Bug fixes should include a regression test when practical.

## Verification

Before claiming completion:
- Build affected projects.
- Run relevant unit tests.
- Run relevant integration/API tests.
- Review failures; never silently ignore them.

Never claim "implemented and tested" unless verification was actually performed.

When verification is incomplete, explicitly state what was/wasn't run, why, and remaining risk.