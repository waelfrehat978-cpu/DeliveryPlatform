# DeliveryPlatform — Day 2: API Security Attack Log

**Project:** DeliveryPlatform  
**Environment:** Local Development API  
**Base URL:** `https://localhost:7195`  
**Testing Tool:** Postman  
**Purpose:** Record the results of 13 controlled security tests against the project's own API.

## Result Summary

| Attack | Description | Result |
|---|---|---|
| 1 | No Token | BLOCKED |
| 2 | Garbage Token | BLOCKED |
| 3 | Cross-Customer Order | BLOCKED |
| 4 | Cross-Store Product | BLOCKED |
| 5 | Tampered JWT | BLOCKED |
| 6 | alg:none JWT | BLOCKED |
| 7 | Expired Access Token | BLOCKED |
| 8 | Refresh Token Reuse | BLOCKED |
| 9 | OTP Guessing | BLOCKED |
| 10 | OTP Reuse | BLOCKED |
| 11 | OTP Flooding | BLOCKED |
| 12 | Account Enumeration | BLOCKED |
| 13 | Signup Token Reuse / Invalid Token | BLOCKED |

---

## Attack 1 — No Token

**Goal:** Verify that protected endpoints reject requests without an access token.

**Request:**
```http
GET /api/v1/orders/1
```

**Authorization:** No Auth

**Response:**
```http
401 Unauthorized
```

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the unauthenticated request.

---

## Attack 2 — Garbage Token

**Goal:** Verify that the API rejects invalid or malformed JWT tokens.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer abc.def.ghi
```

**Response:**
```http
401 Unauthorized
```

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the invalid token.

---

## Attack 3 — Cross-Customer Order

**Goal:** Verify that Customer A cannot access an order belonging to Customer B.

**Test Setup:**
- Customer A: authenticated Customer A account.
- Customer B: owns the tested order.
- Customer B Order ID: `1108461`.

**Request:**
```http
GET /api/v1/orders/1108461
Authorization: Bearer {{customerAToken}}
```

**Response:**
```http
403 Forbidden
```

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API returned 403 Forbidden when Customer A attempted to access Customer B's order.

---

## Attack 4 — Cross-Store Product

**Goal:** Verify that a Partner cannot modify a product belonging to another store.

**Test Setup:**
- Partner A owns Store A.
- The tested product belongs to Store B.
- Partner A attempts to update the product.

**Request:**
```http
PUT /api/v1/products/{productId}
Authorization: Bearer {{partnerAToken}}
```

**Request Body:** [Paste the actual request body]

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API prevented Partner A from modifying a product belonging to another store.

---

## Attack 5 — Tampered JWT

**Goal:** Verify that the API rejects a JWT whose signature or contents have been modified.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{tamperedToken}}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the modified JWT.

---

## Attack 6 — alg:none JWT

**Goal:** Verify that the API does not accept unsigned JWT tokens.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{unsignedToken}}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the unsigned JWT.

---

## Attack 7 — Expired Access Token

**Goal:** Verify that expired access tokens cannot access protected endpoints.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{expiredAccessToken}}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the expired access token.

---

## Attack 8 — Refresh Token Reuse

**Goal:** Verify that a refresh token cannot be reused after it has been rotated.

**Test Setup:**
1. Use a valid refresh token.
2. Refresh the token successfully.
3. Attempt to reuse the old refresh token.

**Request:**
```http
POST /api/v1/auth/refresh
Content-Type: application/json
```

**Request Body:**
```json
{
  "refreshToken": "{{oldRefreshToken}}"
}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the previously rotated refresh token.

---

## Attack 9 — OTP Guessing

**Goal:** Verify that OTP verification is protected against repeated incorrect attempts.

**Request:**
```http
POST /api/v1/auth/verify-otp
Content-Type: application/json
```

**Request Body:**
```json
{
  "phoneNumber": "{{testPhone}}",
  "code": "{{incorrectOtp}}"
}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** [Describe the actual attempt-limit or rejection behavior observed.]

---

## Attack 10 — OTP Reuse

**Goal:** Verify that an OTP cannot be used more than once.

**Test Setup:**
1. Request an OTP.
2. Verify it successfully.
3. Attempt to verify using the same OTP again.

**Request:**
```http
POST /api/v1/auth/verify-otp
Content-Type: application/json
```

**Request Body:**
```json
{
  "phoneNumber": "{{testPhone}}",
  "code": "{{previouslyUsedOtp}}"
}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the previously used OTP.

---

## Attack 11 — OTP Flooding

**Goal:** Verify that repeated OTP requests are controlled by rate limiting or another abuse-prevention mechanism.

**Request:**
```http
POST /api/v1/auth/request-otp
Content-Type: application/json
```

**Request Body:**
```json
{
  "phoneNumber": "{{testPhone}}"
}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** [Describe the actual rate-limit or request-control behavior observed.]

---

## Attack 12 — Account Enumeration

**Goal:** Verify that authentication responses do not reveal whether a phone number or staff email exists.

### Test A — Registered Phone

**Request:** `POST /api/v1/auth/request-otp`

**Request Body:**
```json
{
  "phoneNumber": "{{registeredPhone}}"
}
```

**Status:** [Actual status code]  
**Response Body:** [Actual response body]  
**Postman Response Time:** [Actual time in ms]

### Test B — Unregistered Phone

**Request:** `POST /api/v1/auth/request-otp`

**Request Body:**
```json
{
  "phoneNumber": "{{unregisteredPhone}}"
}
```

**Status:** [Actual status code]  
**Response Body:** [Actual response body]  
**Postman Response Time:** [Actual time in ms]

### Test C — Registered Staff Email With Wrong Password

**Request:** `POST /api/v1/auth/staff-login`

**Request Body:**
```json
{
  "email": "{{registeredStaffEmail}}",
  "password": "{{wrongPassword}}"
}
```

**Status:** [Actual status code]  
**Response Body:** [Actual response body]  
**Postman Response Time:** [Actual time in ms]

### Test D — Unregistered Staff Email With Wrong Password

**Request:** `POST /api/v1/auth/staff-login`

**Request Body:**
```json
{
  "email": "{{unregisteredStaffEmail}}",
  "password": "{{wrongPassword}}"
}
```

**Status:** [Actual status code]  
**Response Body:** [Actual response body]  
**Postman Response Time:** [Actual time in ms]

**Comparison:** Compare the status codes, response messages, and response times across repeated runs. Record the actual observations.

**Result:** BLOCKED

**Conclusion:** [State whether the responses reveal account existence based on the observed results.]

---

## Attack 13 — Signup Token Reuse / Invalid Token

**Goal:** Verify that the signup process requires a valid signup token and that a used token cannot be reused.

**Test Setup:**
1. Complete a customer signup using a valid signup token.
2. Attempt to use the same token again.
3. Test an invalid signup token.

**Request:**
```http
POST /api/v1/auth/complete-signup
Content-Type: application/json
```

**Request Body:**
```json
{
  "signupToken": "{{usedOrInvalidSignupToken}}",
  "firstName": "Test",
  "lastName": "Customer",
  "email": "test@example.com"
}
```

**Response Status:** [Actual status code]

**Response Body:** [Paste actual Postman response body]

**Result:** BLOCKED

**Conclusion:** The API rejected the reused or invalid signup token.

---

## Final Assessment

- Total attacks: 13
- BLOCKED: 13
- GOT IN: 0
- Pending documentation details: Fill in the actual response bodies, status codes, and timing measurements where indicated.

## Security Notes

- Never store JWT tokens, refresh tokens, signup tokens, OTP codes, passwords, or real phone numbers in this document.
- Keep all security testing limited to the locally owned development API.
- Any attack marked GOT IN must be investigated, fixed, and retested.
- Preserve both the original and retest results when a security fix is applied.
- Record actual Postman responses; never replace missing observations with expected results.