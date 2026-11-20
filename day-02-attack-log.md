

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

**Result:** BLOCKED

**Conclusion:** The API correctly rejected an unauthenticated request.

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

**Result:** BLOCKED

**Conclusion:** The API rejected the invalid token.

---

## Attack 3 — Cross-Customer Order

**Goal:** Verify that Customer A cannot access an order belonging to Customer B.

**Test Setup:**

- Customer A: `customerAToken`
- Customer B: `customerBToken`
- Customer B Order ID: `1108461`

**Request:**

```http
GET /api/v1/orders/1108461
Authorization: Bearer {{customerAToken}}
```

**Response:** Pending — actual response not recorded yet.

**Result:** PENDING

**Conclusion:** The test must confirm that Customer A cannot access Customer B's order.

Expected protection may be `403 Forbidden` or `404 Not Found`, depending on the API design.

---

## Attack 4 — Cross-Store Product

**Goal:** Verify that a Partner cannot access or modify a product belonging to another store.

**Test Setup:**

- Partner A owns Store A.
- Product belongs to Store B.
- Partner A attempts to access the product.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should prevent a Partner from accessing or modifying another store's products.

---

## Attack 5 — Tampered JWT

**Goal:** Verify that the API rejects a JWT whose contents or signature have been modified.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should reject tokens with invalid signatures.

---

## Attack 6 — alg:none JWT

**Goal:** Verify that the API does not accept unsigned JWT tokens.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should require a valid signature using its configured signing algorithm.

---

## Attack 7 — Expired Access Token

**Goal:** Verify that expired access tokens cannot access protected endpoints.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should reject expired access tokens, normally with `401 Unauthorized`.

---

## Attack 8 — Refresh Token Reuse

**Goal:** Verify that a refresh token cannot be reused after it has been rotated or revoked.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** A previously used or revoked refresh token should be rejected.

---

## Attack 9 — OTP Guessing

**Goal:** Verify that the OTP verification endpoint has protection against repeated incorrect attempts.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should limit incorrect OTP attempts and prevent unlimited verification attempts.

---

## Attack 10 — OTP Reuse

**Goal:** Verify that an OTP cannot be used more than once.

**Test Setup:**

1. Request an OTP.
2. Verify the OTP successfully.
3. Attempt to verify using the same OTP again.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** A successfully used OTP should not be accepted again.

---

## Attack 11 — OTP Flooding

**Goal:** Verify that repeated OTP requests are controlled by rate limiting or another abuse-prevention mechanism.

**Request:** Not recorded yet.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** The API should limit repeated OTP requests to prevent abuse.

---

## Attack 12 — Account Enumeration

**Goal:** Verify that the API does not reveal whether a phone number belongs to a registered account.

**Test Setup:**

Compare the responses for:

- An existing test account.
- An unregistered test number.

**Response:** Not tested.

**Result:** NOT TESTED

**Conclusion:** Public responses should not reveal whether an account exists.

---

## Final Assessment

Current results:

- BLOCKED: 2 attacks.
- PENDING: 1 attack.
- NOT TESTED: 9 attacks.

The API cannot be considered to have passed all 12 security tests until every test has been executed and its actual response has been documented.

## Security Notes

- Never store JWT tokens or refresh tokens in this document.
- Never store OTP codes, passwords, or real phone numbers.
- Keep all security testing limited to the locally owned development API.
- Any test marked as GOT IN must be investigated and fixed.
- After applying a security fix, repeat the test to verify the result.
