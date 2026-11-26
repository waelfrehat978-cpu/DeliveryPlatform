# Authorization Matrix

## 1. Authorization Matrix

| Operation | Customer | Partner | Admin |
|---|---|---|---|
| POST `/api/v1/auth/request-otp` | Public | Public | Public |
| POST `/api/v1/auth/verify-otp` | Public | Public | Public |
| POST `/api/v1/auth/complete-signup` | Public | Public | Public |
| POST `/api/v1/auth/staff-login` | Public | Public | Public |
| POST `/api/v1/auth/logout` | All | All | All |
| POST `/api/v1/auth/refresh` | Public | Public | Public |
| GET `/api/v1/auth/me` | All | All | All |
| POST `/api/v1/auth/create-partner` | — | — | All |
| POST `/api/v1/addresses` | Own | — | All |
| GET `/api/v1/addresses` | Own | — | All |
| POST `/api/v1/orders` | Own | — | — |
| GET `/api/v1/orders/{orderId}` | Own | Own | All |
| GET `/api/v1/orders` | Own | — | All |
| PUT `/api/v1/orders/{orderId}/status` | — | Own | All |
| GET `/api/v1/customers/{id}` | Own | — | All |
| PUT `/api/v1/customers/{id}` | Own | — | All |
| GET `/api/v1/stores` | All | All | All |
| GET `/api/v1/stores/{id}` | All | All | All |
| POST `/api/v1/stores` | — | — | All |
| PUT `/api/v1/stores/{id}/name` | — | Own | All |
| DELETE `/api/v1/stores/{id}` | — | — | All |
| GET `/api/v1/stores/{storeId}/dashboard` | — | Own | All |
| GET `/api/v1/products` | All | All | All |
| GET `/api/v1/products/{id}` | All | All | All |
| POST `/api/v1/products` | — | Own | All |
| PUT `/api/v1/products/{id}` | — | Own | All |
| DELETE `/api/v1/products/{id}` | — | Own | All |
| GET `/api/v1/products/{id}/history` | — | Own | All |
| GET `/api/v1/orders/{orderId}/history` | — | — | All |
| GET `/api/v1/customers/{id}/history` | — | — | All |
| GET `/api/v1/stores/{id}/history` | — | — | All |

## 2. Order Status Authorization

- Partner can update order status only according to the allowed status transition map.
- Partner cannot cancel orders.
- Admin can set any order status.

## 3. Customer Creation

Customer creation through the old endpoint was removed.

Customer registration is now handled through:

`POST /api/v1/auth/complete-signup`

## 4. Permission Definitions

- **Own:** Access is limited to the user's own records or resources.
- **All:** Access to all applicable records or resources.
- **Public:** Endpoint is accessible without authentication.
- **—:** No permission.

## 5. Audit History Authorization

- Admin can read audit history for all products, orders, customers, and stores.
- Partner can read product history only for products belonging to their own store.
- Partner cannot read order, customer, or store audit history.
- Customer cannot read any audit history.
- Unauthorized access returns HTTP 403 Forbidden.q