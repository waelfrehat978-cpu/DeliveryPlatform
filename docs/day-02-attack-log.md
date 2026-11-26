# DeliveryPlatform — Day 2: API Security Attack Log

**Project:** DeliveryPlatform  
**Environment:** Local Development API  
**Base URL:** `https://localhost:7195`  
**Testing Tool:** Postman  
**Purpose:** Document 13 controlled security tests against the project's own API.

---

## Important Documentation Notice

**The response bodies and HTTP status codes in this document are illustrative examples only. They are not actual Postman captures or verified server responses.**

They are included to demonstrate the intended documentation format. Actual results must be recorded from Postman before this document is submitted as evidence.

---

## Result Summary

| Attack | Description | Illustrative Expected Result | Verification |
|---|---|---|---|
| 1 | No Token | 401 Unauthorized | NOT VERIFIED |
| 2 | Garbage Token | 401 Unauthorized | NOT VERIFIED |
| 3 | Cross-Customer Order | 403 Forbidden | NOT VERIFIED |
| 4 | Cross-Store Product | 403 Forbidden | NOT VERIFIED |
| 5 | Tampered JWT | 401 Unauthorized | NOT VERIFIED |
| 6 | alg:none JWT | 401 Unauthorized | NOT VERIFIED |
| 7 | Expired Access Token | 401 Unauthorized | NOT VERIFIED |
| 8 | Refresh Token Reuse | 401 Unauthorized | NOT VERIFIED |
| 9 | OTP Guessing | 400 / 429 | NOT VERIFIED |
| 10 | OTP Reuse | 400 Bad Request | NOT VERIFIED |
| 11 | OTP Flooding | 429 / Request Rejected | NOT VERIFIED |
| 12 | Account Enumeration | Responses should not reveal account existence | NOT VERIFIED |
| 13 | Signup Token Reuse / Invalid Token | 400 Bad Request | NOT VERIFIED |

---

## Attack 1 — No Token

**Goal:** Verify that protected endpoints reject requests without an access token.

**Request:**
```http
GET /api/v1/orders/1
```

**Authorization:** No Auth

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects unauthenticated requests.

---

## Attack 2 — Garbage Token

**Goal:** Verify that the API rejects invalid or malformed JWT tokens.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer abc.def.ghi
```

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects malformed JWT tokens.

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

**Illustrative Expected Response:**
```http
403 Forbidden
```

**Illustrative Response Body:**
```json
{
  "statusCode": 403,
  "message": "You are not authorized to access this resource.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that a customer cannot access another customer's order.

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

**Request Body — Illustrative Example:**
```json
{
  "name": "Unauthorized Product Update",
  "price": 100.00
}
```

**Illustrative Expected Response:**
```http
403 Forbidden
```

**Illustrative Response Body:**
```json
{
  "statusCode": 403,
  "message": "You are not authorized to access this resource.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that a Partner cannot modify products owned by another store.

---

## Attack 5 — Tampered JWT

**Goal:** Verify that the API rejects a JWT whose signature or contents have been modified.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{tamperedToken}}
```

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects JWT tokens with invalid signatures.

---

## Attack 6 — alg:none JWT

**Goal:** Verify that the API does not accept unsigned JWT tokens.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{unsignedToken}}
```

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects unsigned JWT tokens.

---

## Attack 7 — Expired Access Token

**Goal:** Verify that expired access tokens cannot access protected endpoints.

**Request:**
```http
GET /api/v1/orders/1
Authorization: Bearer {{expiredAccessToken}}
```

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects expired access tokens.

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

**Illustrative Expected Response:**
```http
401 Unauthorized
```

**Illustrative Response Body:**
```json
{
  "statusCode": 401,
  "message": "Invalid or revoked refresh token.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects previously rotated or revoked refresh tokens.

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

**Illustrative Expected Response:**
```http
400 Bad Request
```

**Illustrative Response Body:**
```json
{
  "statusCode": 400,
  "message": "Invalid or expired OTP.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that incorrect OTP attempts are rejected and the configured attempt limit is enforced.

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

**Illustrative Expected Response:**
```http
400 Bad Request
```

**Illustrative Response Body:**
```json
{
  "statusCode": 400,
  "message": "Invalid or expired OTP.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that a previously used OTP is rejected.

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

**Illustrative Expected Response:**
```http
429 Too Many Requests
```

**Illustrative Response Body:**
```json
{
  "statusCode": 429,
  "message": "Too many requests. Please try again later.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that repeated OTP requests are controlled by the configured request limits. The actual status and message depend on the implemented mechanism.

---

## Attack 12 — Account Enumeration

**Goal:** Verify that authentication responses do not reveal whether a phone number or staff email exists.

### Test A — Registered Phone

**Request:**
```http
POST /api/v1/auth/request-otp
```

**Request Body:**
```json
{
  "phoneNumber": "{{registeredPhone}}"
}
```

**Illustrative Response:**
```json
{
  "message": "If the account is eligible, an OTP will be sent."
}
```

**Status:** NOT VERIFIED

**Response Time:** NOT RECORDED

### Test B — Unregistered Phone

**Request:**
```http
POST /api/v1/auth/request-otp
```

**Request Body:**
```json
{
  "phoneNumber": "{{unregisteredPhone}}"
}
```

**Illustrative Response:**
```json
{
  "message": "If the account is eligible, an OTP will be sent."
}
```

**Status:** NOT VERIFIED

**Response Time:** NOT RECORDED

### Test C — Registered Staff Email With Wrong Password

**Request:**
```http
POST /api/v1/auth/staff-login
```

**Request Body:**
```json
{
  "email": "{{registeredStaffEmail}}",
  "password": "{{wrongPassword}}"
}
```

**Illustrative Response:**
```json
{
  "statusCode": 401,
  "message": "Invalid email or password.",
  "errors": null
}
```

**Status:** NOT VERIFIED

**Response Time:** NOT RECORDED

### Test D — Unregistered Staff Email With Wrong Password

**Request:**
```http
POST /api/v1/auth/staff-login
```

**Request Body:**
```json
{
  "email": "{{unregisteredStaffEmail}}",
  "password": "{{wrongPassword}}"
}
```

**Illustrative Response:**
```json
{
  "statusCode": 401,
  "message": "Invalid email or password.",
  "errors": null
}
```

**Status:** NOT VERIFIED

**Response Time:** NOT RECORDED

### Comparison

The illustrative examples show generic responses that do not disclose account existence.

Actual status codes, response bodies, and response times must be compared using Postman before confirming that account enumeration is blocked.

**Illustrative Result:** NOT VERIFIED

**Verification:** NOT VERIFIED

**Conclusion:** The intended behavior is to use generic authentication responses that do not reveal whether an account exists.

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

**Illustrative Expected Response:**
```http
400 Bad Request
```

**Illustrative Response Body:**
```json
{
  "statusCode": 400,
  "message": "Invalid or expired signup token.",
  "errors": null
}
```

**Illustrative Result:** BLOCKED

**Verification:** NOT VERIFIED

**Conclusion:** The expected behavior is that the API rejects reused or invalid signup tokens.

---

## Final Assessment

- Total security test scenarios: 13
- Illustrative expected blocked outcomes: 13
- Actual verified outcomes: NOT VERIFIED
- Actual GOT IN count: NOT DETERMINED
- Actual Postman evidence: Not included in this illustrative document.

**Important:** The illustrative examples must not be presented as actual test evidence. Update the verification status and response details after capturing real Postman results.

---

## Security Notes

- Never store JWT tokens, refresh tokens, signup tokens, OTP codes, passwords, or real phone numbers in this document.
- Keep all security testing limited to the locally owned development API.
- Any attack marked GOT IN must be investigated, fixed, and retested.
- Preserve both the original and retest results when a security fix is applied.
- Record actual Postman responses; never replace missing observations with expected results.
- Clearly distinguish illustrative examples from verified test evidence.
