# API Guidelines

## 1. API Design Style

This project follows RESTful API design.

REST uses resources such as Stores, Products, Customers, and Orders.

## 2. URL Naming Conventions

- Use plural resource names.
- Use lowercase URLs.
- Use hyphens to separate words.
- Use `/api/v1/` as the API version prefix.
- Use query strings when filtering a list by another resource.
- Use the resource ID in the URL when accessing data or actions belonging to a specific resource.

Examples:

- `GET /api/v1/stores`
- `GET /api/v1/products`
- `POST /api/v1/orders`
- `GET /api/v1/orders?customerId=5`
- `GET /api/v1/addresses?customerId=5`
- `GET /api/v1/stores/5/dashboard`
- `PUT /api/v1/orders/7/status`

## 3. HTTP Methods

| Method | Purpose |
|---|---|
| GET | Retrieve resources |
| POST | Create a resource |
| PUT | Replace or update a resource |
| PATCH | Partially update a resource |
| DELETE | Delete a resource |

## 4. HTTP Status Codes

| Status Code | Meaning |
|---|---|
| 200 OK | Request completed successfully |
| 201 Created | Resource created successfully |
| 204 No Content | Request succeeded without a response body |
| 400 Bad Request | Invalid request or validation errors |
| 403 Forbidden | The caller is not allowed to perform this action |
| 404 Not Found | Resource not found |
| 409 Conflict | Conflict with existing data or business rules |
| 422 Unprocessable Entity | A business rule prevents the requested operation |
| 500 Internal Server Error | An unexpected server error occurred |

## 5. Filtering, Sorting, and Pagination

Use query parameters for filtering, sorting, and pagination.

Examples:

- `GET /api/v1/products?storeId=5&page=1&pageSize=20`
- `GET /api/v1/orders?customerId=5&page=1&pageSize=10`
- `GET /api/v1/addresses?customerId=5`

Paged responses use this structure:

- items
- totalCount
- page
- pageSize

Filtering, sorting, and pagination must be performed in SQL Server.

## 6. DTOs

Use DTOs for API requests and responses.

Do not expose database entities directly through API endpoints.

## 7. Error Responses

Return clear and consistent error responses.

Use appropriate HTTP status codes to describe the result of a failed request.

All errors follow the same response structure:

```json
{
  "statusCode": 400,
  "message": "Validation failed.",
  "errors": {
    "page": [
      "Page must be greater than 0."
    ]
  },
  "correlationId": "00-example-correlation-id"
}
```

- `statusCode`: The HTTP status code.
- `message`: A description of the error.
- `errors`: Field-specific validation errors. It is `null` when no specific field errors exist.
- `correlationId`: An identifier used to trace the request in server logs.

Unexpected server errors return status code 500 with a generic message.

## 8. API Versioning

Use version prefixes in API URLs, such as `/api/v1/`.

Introduce a new version when making breaking changes that could affect existing clients.