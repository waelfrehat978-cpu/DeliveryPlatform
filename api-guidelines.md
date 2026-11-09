API Guidelines

1. API Design Style

This project follows RESTful API design.

REST uses resources such as Stores, Products, Customers, and Orders.

2. URL Naming Conventions

- Use plural resource names.
- Use lowercase URLs.
- Use hyphens to separate words.
- Use "/api/v1/" as the API version prefix.

Examples:

- "GET /api/v1/stores"
- "GET /api/v1/products"
- "POST /api/v1/orders"

3. HTTP Methods

Method| Purpose
GET| Retrieve resources 
POST| Create a resource
PUT| Replace or update a resource
PATCH| Partially update a resource
DELETE| Delete a resource

4. HTTP Status Codes

Status Code| Meaning
200 OK| Request completed successfully
201 Created| Resource created successfully
204 No Content| Request succeeded without a response body
400 Bad Request| Invalid request
404 Not Found| Resource not found
409 Conflict| Conflict with existing data or business rules

5. Filtering, Sorting, and Pagination

Use query parameters for filtering, sorting, and pagination.

Example:

"GET /api/v1/products?storeId=5&page=1&pageSize=20"

Paged responses use this structure:

- items
- totalCount
- page
- pageSize

Filtering, sorting, and pagination must be performed in SQL Server.

6. DTOs

Use DTOs for API requests and responses.

Do not expose database entities directly through API endpoints.

7. Error Responses

Return clear and consistent error responses.

Use appropriate HTTP status codes to describe the result of a failed request.

8. API Versioning

Use version prefixes in API URLs, such as "/api/v1/".

Introduce a new version when making breaking changes that could affect existing clients.