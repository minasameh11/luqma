---
trigger: always_on
---

# .NET Security Rules

- Security is enforced server-side. Never trust client-provided identity, tenant, ownership, role, permission, or authorization metadata.
- Derive security context from authenticated identity and trusted server data.

## Authentication

- Use the project's configured JWT/Identity authentication.
- Do not implement custom authentication in controllers.
- Do not manually trust unvalidated JWT claims.
- Follow existing token validation configuration.

## Authorization

- Every protected resource requires server-side authorization.
- Use the project's policies, roles, claims, and resource-based authorization.
- Verify both permission and resource ownership/tenant scope.
- Authentication does not prove ownership.

For protected resources, verify:
- Authenticated user
- Tenant scope when applicable
- Ownership/access
- Required permission
- Valid resource state

Never trust client IDs such as `userId` or `tenantId` as proof of access.

## Identity & DTOs

- Never expose Identity or persistence entities directly.
- Never return passwords/hashes, security stamps, refresh tokens, or authentication internals unless explicitly required and safe.
- Use explicit DTOs.

## Secrets

Never hardcode or commit:
- Passwords
- JWT signing keys
- API keys
- Refresh tokens
- Encryption keys
- Secret connection strings
- External credentials

Use approved configuration/secret providers.

## Logging

Never log:
- Passwords
- Access/refresh tokens
- API keys
- Authorization headers/cookies
- Secret connection strings
- Sensitive personal data
- Full bodies containing secrets

Use structured logs with safe context.

## Errors

- Use centralized exception handling.
- Never expose stack traces, internal exception messages, SQL containing secrets, file paths, connection details, or infrastructure internals.

## Input Validation

Validate all untrusted input before executing the use case:
- Body
- Route/query parameters
- Relevant headers
- File uploads
- Webhooks/external callbacks

Do not rely on frontend validation.

## Mass Assignment

Explicitly map allowed request fields.

Never allow clients to change privileged fields such as:
- UserId
- TenantId
- Role
- Permissions
- IsAdmin
- PaymentStatus
- CreatedBy
- CreatedAt

unless the use case explicitly permits it.

## File Uploads

- Validate type and size.
- Do not trust file extensions alone.
- Generate server-side filenames.
- Never use client-provided paths directly.
- Do not execute uploaded files.
- Store uploads outside executable paths when appropriate.

## Webhooks / External Integrations

Validate requests according to the provider's security model.

Use when supported:
- Signature validation
- Shared secrets
- Provider authentication
- Idempotency

Do not trust a request merely because it reaches an internal endpoint.

## SignalR

- Authorize hubs.
- Never trust client-provided user/group/tenant IDs.
- Resolve groups from server-verified identity and permissions.
- Keep payloads DTO-based.

## Redis

- Do not store sensitive data unless required and protected.
- Do not expose secrets through cache keys.
- Use appropriate scope, TTL, and access controls.

## Security Verification

Before completion, verify:
- Authentication
- Authorization
- Ownership
- Tenant isolation
- Input validation
- Mass-assignment protection
- Error handling
- Secret handling
- Sensitive logging

Test both allowed and denied scenarios.