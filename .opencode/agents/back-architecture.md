---
trigger: always_on
---

# .NET Architecture Rules

- Treat this as a production ASP.NET Core Web API.
- Before changing code, inspect the solution, existing features/modules, shared utilities, infrastructure, and similar implementations.
- Reuse existing conventions and modify only files required for the task.

## Architecture

Preserve the existing Clean Architecture boundaries:

API -> Application -> Domain
Infrastructure -> Application / Domain

Domain must not depend on:
- ASP.NET Core
- EF Core
- PostgreSQL
- Redis
- SignalR
- Identity
- HTTP
- Infrastructure
- External services

Do not reference Infrastructure from Application.

## Domain

Domain owns:
- Entities
- Value Objects
- Domain Events
- Enums
- Business rules
- Required domain services
- Meaningful repository abstractions

Domain must remain framework-, persistence-, and transport-agnostic.

Do not put HTTP, DTOs, controllers, EF configuration, database queries, Redis, SignalR, or external API calls in Domain.

## Application

Application owns:
- Use cases
- Commands / Queries
- DTOs
- Validation
- Authorization requirements
- Application interfaces
- External-service abstractions
- Workflow orchestration

Application must not contain controllers, HTTP handling, infrastructure implementations, direct Redis usage, DbContext usage, or provider-specific database logic unless the existing architecture explicitly requires it.

## Infrastructure

Infrastructure owns:
- EF Core / DbContext
- IEntityTypeConfiguration<T>
- Migrations
- Repository implementations
- Identity
- Redis
- External integrations
- Notifications/messages
- PostgreSQL-specific behavior

Infrastructure implements abstractions defined by inner layers.

## API / Presentation

API owns:
- Controllers / endpoints
- Request binding
- Middleware
- Authentication setup
- HTTP response mapping
- API versioning
- Serialization

Keep endpoints thin:
request -> application use case -> response.

No business rules, database queries, Redis operations, complex workflows, or trust in client-provided authorization data inside controllers.

## Organization

Follow the project's existing structure.

If feature-based organization already exists, keep it consistent:

Application/
└── Features/
    └── Orders/
        ├── Commands/
        ├── Queries/
        ├── DTOs/
        └── Validators/

Do not reorganize the project just to impose a preferred structure.

## Abstractions

Do not introduce generic repositories, generic services, mediator layers, wrappers, factories, or extra domain services without a real need.

Existing project patterns are the source of truth.

## Change Scope

Make the smallest correct change.

Do not:
- Refactor unrelated code
- Rename unrelated types
- Reorganize unrelated folders
- Upgrade packages without necessity
- Change architectural patterns unnecessarily