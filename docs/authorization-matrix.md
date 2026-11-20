# Authorization Matrix

## 1. Authorization Matrix

| Operation | Customer | Partner | Admin |
|---|---|---|---|
| POST `/api/v1/auth/request-otp` | Public | Public | Public |
| POST `/api/v1/auth/verify-otp` | Public | Public | Public |
| POST `/api/v1/auth/complete-signup` | Public | Public | Public |
| POST `/api/v1/auth/staff-login` | Public | Public | Public |
| POST `/api/v1/auth/logout` | Own | Own | Own |
| POST `/api/v1/auth/create-partner` | — | — | All |
| POST `/api/v1/auth/refresh` | Public | Public | Public |
| GET `/api/v1/auth/me` | Own | Own | Own |
| POST `/api/v1/addresses` | Own | — | All |
| GET `/api/v1/addresses` | Own | — | All |
| POST `/api/v1/orders` | Own | — | — |
| GET `/api/v1/orders/{orderId}` | Own | Own | All |
| GET `/api/v1/orders` | Own | — | All |
| PUT `/api/v1/orders/{orderId}/status` | — | Own | All |
| GET `/api/v1/customers/{id}` | Own | — | All |
| PUT `/api/v1/customers/{id}` | Own | — | All |
| GET `/api/v1/stores` | Public | Public | Public |
| GET `/api/v1/stores/{id}` | Public | Public | Public |
| POST `/api/v1/stores` | — | — | All |
| GET `/api/v1/stores/{storeId}/dashboard` | — | Own | All |
| GET `/api/v1/products` | Public | Public | Public |
| GET `/api/v1/products/{id}` | Public | Public | Public |
| POST `/api/v1/products` | — | Own | All |
| PUT `/api/v1/products/{id}` | — | Own | All |

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