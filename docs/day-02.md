# Day 2 – Identity & Authorization

# اليوم الثاني – الهوية والصلاحيات

## Part 0 – Authorization Matrix

### Customer Account Creation Decision – قرار إنشاء حساب العميل

Customer accounts are created only after successful OTP verification.

لا يتم إنشاء حسابات العملاء إلا بعد التحقق بنجاح من رمز التحقق لمرة واحدة OTP.

**Reason:** The customer's phone number must be verified before creating the account.

**السبب:** يجب التحقق من رقم هاتف العميل قبل إنشاء الحساب.

---

## Part 1 – User Table Design

### 1. Table Structure – هيكل الجداول

The application uses two separate tables: `User` and `Customer`.

يستخدم التطبيق جدولين منفصلين: `User` و`Customer`.

**User table:**

| Column | Purpose |
|---|---|
| Id | Primary key |
| Email | Staff login email |
| PhoneNumber | Customer login phone number |
| PasswordHash | Hashed staff password |
| Role | Customer, Partner, or Admin |
| CustomerId | Optional link to the Customer record |

**Customer table:**

| Column | Purpose |
|---|---|
| Id | Primary key |
| FirstName | Customer first name |
| LastName | Customer last name |
| Email | Optional customer email |
| Other customer profile fields | Customer profile information |

The `User` table stores authentication and identity information, while the `Customer` table stores customer profile information.

يخزن جدول `User` معلومات الهوية والمصادقة، بينما يخزن جدول `Customer` معلومات الملف الشخصي للعميل.

### 2. Why Two Tables Instead of One?

We use two tables because authentication information and customer profile information have different responsibilities.

نستخدم جدولين لأن معلومات المصادقة ومعلومات الملف الشخصي للعميل لها مسؤوليات مختلفة.

- `User` manages login credentials, roles, and authentication.
- `Customer` manages customer profile information.

- جدول `User` مسؤول عن بيانات تسجيل الدخول والأدوار والمصادقة.
- جدول `Customer` مسؤول عن معلومات الملف الشخصي للعميل.

Keeping them separate avoids mixing customer-specific information with staff account information.

فصل الجدولين يمنع خلط معلومات العملاء مع معلومات حسابات الموظفين.

### 3. Three Practical Design Questions

**Question 1: Where should the customer's phone number be stored?**

It belongs in `User` because the phone number is used for customer authentication and OTP verification.

**السؤال الأول: أين يجب تخزين رقم هاتف العميل؟**

يتم تخزينه في `User` لأنه يُستخدم لمصادقة العميل والتحقق باستخدام OTP.

**Question 2: Should Admin and Partner accounts have customer profile records?**

No. Admin and Partner accounts use the `User` table for authentication, but they do not need customer profile records.

**السؤال الثاني: هل يجب أن يمتلك Admin وPartner سجلات في جدول Customer؟**

لا. يستخدم Admin وPartner جدول `User` للمصادقة، لكنهما لا يحتاجان إلى سجلات ملفات شخصية في جدول `Customer`.

**Question 3: What happens if customer profile information changes?**

Customer profile information can be updated in the `Customer` table without mixing those changes with authentication data.

**السؤال الثالث: ماذا يحدث إذا تغيرت معلومات الملف الشخصي للعميل؟**

يمكن تحديث معلومات العميل في جدول `Customer` دون خلط هذه التغييرات مع بيانات المصادقة.

### 4. Primary Key Type – نوع المفتاح الأساسي

The `User` table uses an `int` primary key.

يستخدم جدول `User` مفتاحاً أساسياً من نوع `int`.

**Reason:** An integer ID is sufficient for this project and matches the existing database entities.

**السبب:** رقم التعريف من نوع Integer كافٍ لهذا المشروع ويتوافق مع الكيانات الموجودة في قاعدة البيانات.

### 5. Partner and Store Relationship – العلاقة بين Partner والمتجر

Each Partner account is linked to one Store, and each Store has one Partner account.

يرتبط كل حساب Partner بمتجر واحد، ولكل متجر حساب Partner واحد.

**Why this relationship?**

A Partner is responsible for managing one specific store. This relationship makes it clear which store the Partner is authorized to manage and prevents assigning multiple Partner accounts to the same store.

**لماذا اخترنا هذه العلاقة؟**

لأن الشريك مسؤول عن إدارة متجر محدد. تجعل هذه العلاقة المتجر الذي يحق للشريك إدارته واضحاً، وتمنع تعيين أكثر من حساب Partner للمتجر نفسه.

### 6. Password Hashing Algorithm – خوارزمية تجزئة كلمات المرور

We use ASP.NET Core `PasswordHasher<User>` to hash passwords.

نستخدم `PasswordHasher<User>` من ASP.NET Core لتجزئة كلمات المرور.

**Reason:** It provides a built-in password hashing implementation designed for securely storing passwords.

**السبب:** يوفر تطبيقاً جاهزاً لتجزئة كلمات المرور ومصمماً لتخزينها بشكل آمن.

### 7. Salt – الملح العشوائي

The password hashing implementation generates and stores a random salt automatically.

تقوم آلية تجزئة كلمة المرور بإنشاء ملح عشوائي وتخزينه تلقائياً.

**Reason:** A unique salt helps protect passwords against precomputed hash attacks.

**السبب:** يساعد الملح الفريد على حماية كلمات المرور من هجمات الجداول المحسوبة مسبقاً.

### 8. Slow Password Hashing – بطء تجزئة كلمات المرور

Password hashing uses PBKDF2, which performs many iterations to make password verification computationally expensive.

تستخدم تجزئة كلمات المرور خوارزمية PBKDF2، التي تنفذ عدداً كبيراً من التكرارات لجعل عملية التحقق من كلمة المرور مكلفة حسابياً.

**Reason:** Slower password hashing makes large-scale password guessing more difficult.

**السبب:** جعل تجزئة كلمات المرور أبطأ يجعل تخمين عدد كبير من كلمات المرور أكثر صعوبة.

---

## Part 2 – Two Ways In, One Kind of Token

## الجزء الثاني – طريقتان لتسجيل الدخول، ونوع واحد من رموز الوصول

### 1. Authentication Flows – مسارات المصادقة

The application supports two authentication flows:

يدعم التطبيق طريقتين للمصادقة:

- **Customer:** Phone Number + OTP.
- **Staff:** Email + Password (Partner and Admin).

- **العميل:** رقم الهاتف + رمز التحقق لمرة واحدة.
- **الموظفون:** البريد الإلكتروني + كلمة المرور (Partner وAdmin).

Both flows use the same JWT Access Token service to generate access tokens.

تستخدم كلتا الطريقتين نفس خدمة JWT Access Token لإنشاء رموز الوصول.

### 2. Customer Authentication – OTP

Customers authenticate using their phone numbers and a One-Time Password (OTP).

يقوم العملاء بتسجيل الدخول باستخدام أرقام هواتفهم ورمز تحقق لمرة واحدة.

Implemented endpoints:

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/v1/auth/request-otp` | Request an OTP code |
| POST | `/api/v1/auth/verify-otp` | Verify the OTP code |
| POST | `/api/v1/auth/complete-signup` | Complete customer registration |

OTP security rules:

- OTP codes are generated using `RandomNumberGenerator`.
- OTP codes contain six digits.
- OTP expiration time is 5 minutes.
- Maximum of 5 incorrect attempts.
- A minimum of 60 seconds between requests.
- Maximum of 5 requests per hour.
- Only the latest OTP code is accepted.
- OTP codes are not returned in API responses.

قواعد أمان OTP:

- يتم إنشاء الرموز باستخدام `RandomNumberGenerator`.
- يتكون الرمز من ستة أرقام.
- تنتهي صلاحيته بعد 5 دقائق.
- الحد الأقصى للمحاولات الخاطئة 5.
- يجب الانتظار 60 ثانية على الأقل بين الطلبات.
- الحد الأقصى 5 طلبات خلال ساعة.
- يتم قبول أحدث رمز فقط.
- لا يتم إرجاع الرمز ضمن استجابة API.

### 3. Where Are OTP Codes Stored?

OTP records are stored in the database in the OTP table.

يتم تخزين سجلات OTP في قاعدة البيانات داخل جدول OTP.

The application stores the OTP hash rather than keeping the original code as the stored verification value.

يخزن التطبيق قيمة Hash للرمز بدلاً من الاحتفاظ بالرمز الأصلي كقيمة التحقق المخزنة.

**Why?**

If the database is exposed, storing a hash reduces the risk of revealing the original OTP codes.

**لماذا؟**

إذا تم الوصول إلى قاعدة البيانات دون تصريح، فإن تخزين Hash يقلل خطر كشف رموز OTP الأصلية.

### 4. Why Is the OTP Not Returned by the API?

The OTP is delivered through the configured SMS sender. It must not be returned in the API response.

يتم إرسال OTP من خلال خدمة إرسال الرسائل النصية SMS المحددة في الإعدادات، ولا يجب إرجاعه في استجابة API.

**Reason:** Returning the OTP in the API response would allow someone who can access that response to obtain the verification code without receiving the SMS.

**السبب:** إرجاع OTP في استجابة API قد يسمح لشخص يستطيع الوصول إلى الاستجابة بالحصول على رمز التحقق دون استلام الرسالة النصية.

### 5. New Customer Account Creation

A new Customer account is created only after successful OTP verification.

يتم إنشاء حساب العميل الجديد فقط بعد التحقق بنجاح من OTP.

The verification process returns a temporary signup token. The customer then uses this token to complete registration.

تعيد عملية التحقق رمز تسجيل مؤقتاً، ثم يستخدم العميل هذا الرمز لإكمال التسجيل.

The signup token is stored as a hash in the database and is not reusable after successful completion.

يتم تخزين رمز التسجيل على شكل Hash في قاعدة البيانات، ولا يمكن إعادة استخدامه بعد إكمال التسجيل بنجاح.

**Why this design?**

It ensures that creating a customer account requires proof of phone ownership and prevents completing registration using only a phone number.

**لماذا اخترنا هذا التصميم؟**

لأنه يضمن أن إنشاء حساب العميل يتطلب إثبات ملكية رقم الهاتف، ويمنع إكمال التسجيل باستخدام رقم الهاتف وحده.

### 6. Staff Authentication – Email and Password

Partners and Admins authenticate using their email addresses and passwords.

يقوم الشركاء والمسؤولون بتسجيل الدخول باستخدام البريد الإلكتروني وكلمات المرور.

Endpoint:

`POST /api/v1/auth/staff-login`

Password verification uses ASP.NET Core `PasswordHasher<User>`.

يتم التحقق من كلمات المرور باستخدام `PasswordHasher<User>` في ASP.NET Core.

Invalid credentials return a generic `401 Unauthorized` response:

`Invalid credentials.`

This prevents the API from revealing whether an email address exists.

وهذا يمنع API من كشف ما إذا كان عنوان البريد الإلكتروني موجوداً أم لا.

### 7. JWT Signing Key Storage

The JWT signing key must be stored in configuration that is kept outside source control, such as an environment variable or development User Secrets.

يجب تخزين مفتاح توقيع JWT في إعدادات خارج نظام التحكم بالمصدر، مثل متغيرات البيئة أو User Secrets أثناء التطوير.

**Reason:** Anyone who obtains the signing key may be able to create tokens that appear to be valid.

**السبب:** أي شخص يحصل على مفتاح التوقيع قد يتمكن من إنشاء Tokens تبدو وكأنها صالحة.

The real signing key must never be committed to GitHub.

يجب عدم رفع مفتاح التوقيع الحقيقي إلى GitHub.

### 8. JWT Access Token

Both authentication flows use the same `IJwtTokenService` to generate JWT Access Tokens.

تستخدم كلتا طريقتي المصادقة نفس `IJwtTokenService` لإنشاء رموز الوصول JWT.

JWT configuration:

| Setting | Value |
|---|---|
| Access Token Lifetime | 15 minutes |
| Signing Algorithm | HMAC-SHA256 |
| Issuer | Configured in appsettings |
| Audience | Configured in appsettings |

JWT claims:

- `sub`: User ID.
- `role`: User role.
- `storeId`: Included for Partner accounts.
- `customerId`: Included for Customer accounts.

تمثل هذه الـ Claims المعلومات التالية:

- `sub`: معرّف المستخدم.
- `role`: دور المستخدم.
- `storeId`: يتم تضمينه لحسابات Partner.
- `customerId`: يتم تضمينه لحسابات Customer.

### 9. TokenValidationParameters

The JWT Bearer configuration validates incoming tokens.

يقوم إعداد JWT Bearer بالتحقق من صحة Tokens الواردة.

The following settings explain the purpose of the common validation parameters. Confirm that each one matches the actual configuration in `Program.cs`.

- `ValidateIssuer`: Ensures that the token was issued by the expected issuer.
- `ValidateAudience`: Ensures that the token is intended for this application.
- `ValidateLifetime`: Rejects tokens that have expired.
- `ValidateIssuerSigningKey`: Ensures that the token signature is checked using the configured signing key.
- `ValidIssuer`: Specifies the trusted token issuer.
- `ValidAudience`: Specifies the intended token audience.
- `IssuerSigningKey`: Provides the key used to validate the token signature.
- `ClockSkew`: Allows a small time difference between servers when checking token expiration.

توضيح إعدادات التحقق:

- `ValidateIssuer`: يتأكد أن مصدر إصدار Token هو المصدر المتوقع.
- `ValidateAudience`: يتأكد أن Token موجه لهذا التطبيق.
- `ValidateLifetime`: يرفض Tokens التي انتهت صلاحيتها.
- `ValidateIssuerSigningKey`: يضمن التحقق من التوقيع باستخدام مفتاح التوقيع المحدد.
- `ValidIssuer`: يحدد مصدر إصدار Token الموثوق.
- `ValidAudience`: يحدد الجهة المستهدفة بالـ Token.
- `IssuerSigningKey`: يوفر المفتاح المستخدم للتحقق من توقيع Token.
- `ClockSkew`: يسمح بفارق زمني صغير بين الخوادم عند التحقق من انتهاء الصلاحية.

JWT Bearer authentication is configured in ASP.NET Core.

تم إعداد JWT Bearer Authentication داخل ASP.NET Core.

**Note:** Refresh Token creation, rotation, and revocation are implemented in Part 5.

**ملاحظة:** تم تنفيذ إنشاء Refresh Token وتجديده وإلغاؤه في Part 5.

### 10. Partner Account Creation

Only Admin users can create Partner accounts.

يمكن لمستخدمي Admin فقط إنشاء حسابات Partner.

Endpoint:

`POST /api/v1/auth/create-partner`

Authorization:

`[Authorize(Roles = "Admin")]`

Before creating a Partner account, the service checks:

- The requested Store exists.
- The Store does not already have a Partner.
- The email address is not already registered.

قبل إنشاء حساب Partner، تتحقق الخدمة من:

- وجود المتجر المطلوب.
- عدم امتلاك المتجر لحساب Partner بالفعل.
- عدم تسجيل البريد الإلكتروني مسبقاً.

A successful request returns `201 Created`.

محاولة تعيين Partner ثانٍ لنفس المتجر تعيد `409 Conflict`.

### 11. Product Management

The following endpoints were implemented:

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/v1/products` | Create a product |
| PUT | `/api/v1/products/{id}` | Update a product |

Product creation validates that the referenced Store and Category exist.

عند إنشاء منتج، يتم التحقق من وجود المتجر والتصنيف المرتبطين به.

Product updates validate that the product and requested Category exist.

عند تعديل منتج، يتم التحقق من وجود المنتج والتصنيف المطلوب.

### 12. Database Seeder

The DataSeeder was updated to create:

- One Customer User for every seeded Customer.
- One Partner User for every seeded Store.
- Exactly one Admin User.

تم تحديث DataSeeder لإنشاء:

- مستخدم Customer لكل عميل تتم إضافته بواسطة Seeder.
- مستخدم Partner لكل متجر.
- مستخدم Admin واحد فقط.

Additional rules:

- Customer users are associated with their Customer records.
- Partner users are associated with their Store records.
- Staff passwords are hashed using `PasswordHasher<User>`.
- Seed credentials are loaded from configuration.
- OTP codes are not seeded.

### 13. Testing Results

The following operations were tested using Postman.

تم اختبار العمليات التالية باستخدام Postman.

| Test | Expected Result | Actual Result |
|---|---|---|
| Customer OTP authentication | Successful authentication | Passed |
| Customer signup after OTP verification | Account created | Passed |
| Duplicate customer signup | 409 Conflict | Passed |
| Staff login with valid credentials | 200 OK | Passed |
| Staff login with invalid credentials | 401 Unauthorized | Passed |
| Create Partner as Admin | 201 Created | Passed |
| Create Partner for a Store that already has a Partner | 409 Conflict | Passed |
| Create a new Store | 201 Created | Passed |
| Create Product | 201 Created | Passed |
| Update Product | 200 OK | Passed |

### 14. Part 2 Status

The authentication flows, JWT Access Token generation, Partner account creation, Product creation and updating, and Seeder modifications have been implemented and tested.

تم تنفيذ واختبار مسارات المصادقة وإنشاء رموز الوصول JWT وإنشاء حسابات Partner وإنشاء المنتجات وتعديلها وتحديث Seeder.

Refresh Token functionality is documented in Part 5.

---

## Part 3 – One Service That Knows Who Is Calling

## الجزء الثالث – خدمة واحدة تعرف من هو المستخدم الحالي

### 1. ICurrentUser Interface

The `ICurrentUser` interface was created in the Data project. It provides five properties: `IsAuthenticated`, `UserId`, `Role`, `StoreId`, and `CustomerId`.

تم إنشاء الواجهة `ICurrentUser` داخل مشروع Data، وتوفر خمس خصائص: `IsAuthenticated` و`UserId` و`Role` و`StoreId` و`CustomerId`.

### 2. CurrentUser Implementation

The `CurrentUser` class was created in the API project and implements `ICurrentUser`. It reads the current user's information from `IHttpContextAccessor.HttpContext.User`, using the claims provided by the validated JWT token.

تم إنشاء الكلاس `CurrentUser` داخل مشروع API ويطبق الواجهة `ICurrentUser`. ويقرأ معلومات المستخدم الحالي من `IHttpContextAccessor.HttpContext.User` باستخدام الـ Claims الموجودة في JWT Token الذي تم التحقق منه.

### 3. Dependency Injection

`AddHttpContextAccessor()` and `AddScoped<ICurrentUser, CurrentUser>()` were registered in `Program.cs`.

تم تسجيل `AddHttpContextAccessor()` و`AddScoped<ICurrentUser, CurrentUser>()` داخل `Program.cs`.

The Scoped lifetime ensures that the current user service is associated with the current request.

يضمن عمر Scoped ارتباط خدمة المستخدم الحالي بالطلب الحالي.

### 4. When Nobody Is Logged In

When nobody is logged in, `ICurrentUser` returns:

- `IsAuthenticated = false`
- `UserId = null`
- `Role = null`
- `StoreId = null`
- `CustomerId = null`

This is correct because there is no authenticated user, and the application must not invent a user identity.

عندما لا يكون هناك مستخدم مسجل الدخول، ترجع `ICurrentUser` القيم المذكورة أعلاه. وهذا صحيح لأنه لا يوجد مستخدم تم التحقق من هويته، ولا يجب على التطبيق اختراع هوية مستخدم غير حقيقية.

### 5. Who Did It When There Is No User?

For background or system operations without an authenticated user, the actor is considered a **System / Background Operation**, not a real user. No fake user ID is assigned.

بالنسبة للعمليات الخلفية أو عمليات النظام التي لا يوجد فيها مستخدم مسجل، نعتبر المنفذ **System / Background Operation** وليس مستخدماً حقيقياً. ولا نعطيه رقم مستخدم وهمياً.

### 6. Service Layer Independence

The Service project was searched for `HttpContext`, and no results were found. This confirms that the Service layer does not depend on HTTP context.

تم البحث عن `HttpContext` داخل مشروع Service ولم تظهر أي نتائج. وهذا يؤكد أن طبقة Service لا تعتمد على سياق HTTP.

### Status

Part 3 implementation and documentation completed.

تم الانتهاء من تنفيذ وتوثيق Part 3.

---

## Part 4 – Two Layers of Checks on Every Endpoint

### 1. Central Home for Owner Checks

All resource ownership checks are centralized in `ResourceAuthorizationService`.

This service is responsible for checking whether the current user has permission to access a specific resource.

It uses `ICurrentUser` to identify the caller and `DeliveryPlatformDbContext` to verify resource ownership in the database.

The service provides the following methods:

- `EnsureStoreAccessAsync`
- `EnsureProductAccessAsync`
- `EnsureCustomerAccessAsync`
- `EnsureOrderAccessAsync`

**Why this approach?**

Centralizing ownership checks avoids duplicating authorization logic across multiple services and endpoints. It also ensures that the same ownership rules are applied consistently throughout the application.

تم وضع فحوصات ملكية الموارد في `ResourceAuthorizationService`. وتستخدم الخدمة `ICurrentUser` لمعرفة المستخدم الحالي و`DeliveryPlatformDbContext` للتحقق من ملكية المورد داخل قاعدة البيانات.

ويساعد هذا الأسلوب على منع تكرار منطق الصلاحيات وتطبيق قواعد الملكية بشكل موحد.

### 2. Trusting the `storeId` JWT Claim

The `storeId` claim is used to identify the Partner's assigned store.

However, we do not fully trust this claim because a Partner's store assignment may change while an old JWT is still valid.

For every store ownership check, the application:

1. Reads the `storeId` claim from the current user's token.
2. Loads the Partner's current `StoreId` from the database.
3. Compares the database value with the token claim and the requested store ID.
4. Grants access only if all values match.

**Why?**

This prevents an old token from granting access to a store that the Partner is no longer assigned to.

The database remains the source of truth for the Partner's current store assignment.

يتم التحقق من قيمة `storeId` الموجودة في JWT بمقارنتها مع القيمة الحالية في قاعدة البيانات ومع المتجر المطلوب.

هذا يمنع التوكن القديم من منح Partner صلاحية الوصول إلى متجر لم يعد مرتبطاً به، وتبقى قاعدة البيانات المصدر الأساسي لمعرفة المتجر الحالي.

### 3. What Does HTTP 403 Reveal?

HTTP `403 Forbidden` means that the server understood the request, but the current user is not allowed to access the requested resource.

HTTP `403 Forbidden` تعني أن الخادم فهم الطلب، لكن المستخدم الحالي غير مسموح له بالوصول إلى المورد المطلوب.

**What does this tell an attacker that 404 would not?**

A `403` response may reveal that the requested resource exists but access is forbidden.

A `404 Not Found` response does not confirm that the resource exists.

**ماذا تكشف 403 للمهاجم ولا تكشفه 404؟**

قد تكشف `403` أن المورد المطلوب موجود، لكن الوصول إليه ممنوع.

أما `404` فلا تؤكد وجود المورد.

For sensitive resources, the application may use `404` to avoid revealing whether a resource exists, depending on the authorization policy.

بالنسبة للموارد الحساسة، يمكن للتطبيق استخدام `404` لتجنب كشف وجود المورد، حسب سياسة الصلاحيات.

---

## Part 5 – Refresh Tokens & Fast Seeding

### 1. Refresh Token Storage

The RefreshToken table stores the hashed refresh token, its UserId, expiration date, revocation date, and replacement token ID.

يخزن جدول RefreshToken قيمة الـ Refresh Token بعد تطبيق SHA-256 عليها، بالإضافة إلى UserId وتاريخ الانتهاء وتاريخ الإلغاء ومعرّف الرمز البديل.

The original refresh token is sent to the client, while only its hash is stored in the database.

يتم إرسال Refresh Token الأصلي إلى العميل، بينما يتم تخزين Hash فقط في قاعدة البيانات.

### 2. Refresh Token Generation

Refresh tokens are generated using 32 cryptographically secure random bytes and converted to Base64.

يتم إنشاء Refresh Tokens باستخدام 32 بايت عشوائية آمنة تشفيرياً، ثم تحويلها إلى Base64.

Refresh tokens are not JWTs.

الـ Refresh Tokens ليست JWTs.

### 3. Token Rotation

When a valid refresh token is used, the system issues a new access token and a new refresh token.

عند استخدام Refresh Token صالح، يصدر النظام Access Token جديداً وRefresh Token جديداً.

The old refresh token is revoked, and its `ReplacedByTokenId` references the new token.

يتم إلغاء Refresh Token القديم، ويشير `ReplacedByTokenId` إلى الرمز الجديد.

### 4. Refresh Token Reuse Detection

If a revoked refresh token is reused, the system revokes all refresh tokens belonging to that user and returns HTTP 401 Unauthorized.

إذا تمت إعادة استخدام Refresh Token ملغى، يقوم النظام بإلغاء جميع Refresh Tokens الخاصة بالمستخدم، ثم يعيد HTTP 401 Unauthorized.

### 5. Logout

Logout requires authentication and accepts a refresh token.

تتطلب عملية تسجيل الخروج وجود مستخدم مسجل الدخول، وتستقبل Refresh Token.

The system revokes the submitted token only if it belongs to the authenticated user.

يقوم النظام بإلغاء الرمز المرسل فقط إذا كان تابعاً للمستخدم الذي قام بتسجيل الدخول.

### 6. Refresh and Logout Endpoints

- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`

### 7. Full Database Reseed

The full database reseed completed successfully.

تمت إعادة تهيئة قاعدة البيانات بالكامل بنجاح.

#### Actual Seed Results

| Table | Rows | Insert Time |
|---|---:|---:|
| Stores | 50 | 0.651 sec |
| Categories | 30 | 0.115 sec |
| Customers | 20,000 | 0.888 sec |
| Users | 20,051 | 0.306 sec |
| Addresses | 35,000 | 1.610 sec |
| Products | 40,000 | 1.715 sec |
| Orders | 300,000 | 18.431 sec |
| OrderLines | 970,341 | 15.353 sec |
| OrderStatusHistory | 1,389,143 | 9.480 sec |

**Total duration, including generation: 56.317 seconds.**

**إجمالي الوقت، بما في ذلك توليد البيانات: 56.317 ثانية.**

#### Why Is Seeding Fast?

Only 51 password hashes are generated:

- 50 Partner accounts.
- 1 Admin account.

يتم إنشاء 51 Password Hash فقط:

- 50 حساب Partner.
- حساب Admin واحد.

The 20,000 Customer accounts do not have passwords or PasswordHash values in the seed data.

حسابات العملاء البالغ عددها 20,000 لا تحتوي على كلمات مرور أو قيم PasswordHash ضمن بيانات Seed.

This avoids unnecessary password hashing during development seeding without weakening the real authentication process.

هذا يتجنب عمليات Password Hashing غير الضرورية أثناء تجهيز بيانات التطوير، دون إضعاف عملية تسجيل الدخول الحقيقية.

### 8. Day 1 vs Day 2 Seeding Time

The Day 2 seed results are documented above.

The Day 1 comparison must use the actual measurements from `docs/day-01.md`.

| Measurement | Day 1 | Day 2 |
|---|---:|---:|
| Total duration | TODO: Add Day 1 result | 56.317 sec |
| Customer rows | TODO: Add Day 1 result | 20,000 |
| User rows | TODO: Add Day 1 result | 20,051 |

The Day 2 seed includes the new `Users` table and 51 password hashes. The remaining seed data is largely unchanged.

تتضمن بيانات Day 2 جدول `Users` الجديد و51 عملية Password Hash. أما بقية بيانات Seed فلم تتغير بشكل كبير.

### 9. Order Code Sequence

Order codes use a SQL Server sequence named `OrderCodeSequence`.

يستخدم إنشاء أرقام الطلبات Sequence في SQL Server باسم `OrderCodeSequence`.

For each new order, the application requests the next sequence value using:

```sql
NEXT VALUE FOR dbo.OrderCodeSequence
```

This provides a database-generated sequence value for order code generation.

توفر هذه العملية قيمة تسلسلية جديدة من قاعدة البيانات عند إنشاء كل طلب.

### 10. Part 5 Completion

Refresh token generation, storage, rotation, reuse detection, logout, and full database reseeding have been implemented and tested.

تم تنفيذ واختبار إنشاء Refresh Tokens وتخزينها وتدويرها واكتشاف إعادة استخدامها وتسجيل الخروج وإعادة تهيئة قاعدة البيانات بالكامل.

**Part 5 completed.**

**اكتمل الجزء الخامس.**

---

## Part 6 – Security Testing

### 1. Attack Testing Results

The authorization and authentication tests were performed against the local DeliveryPlatform API.

تم إجراء اختبارات المصادقة والصلاحيات على نسخة DeliveryPlatform المحلية.

The tests checked whether protected endpoints reject unauthorized requests and whether users can access only resources they are allowed to access.

تحققت الاختبارات من رفض الطلبات غير المصرح بها، ومن أن المستخدم لا يستطيع الوصول إلا إلى الموارد المسموح له بها.

### 2. GOT IN Count

**Number of attacks that successfully GOT IN: TODO**

عدد الهجمات التي نجحت في تجاوز الحماية: **TODO**

### 3. How Were the Issues Fixed?

The identified issues were addressed through changes to authentication, authorization, resource ownership checks, and token validation.

تمت معالجة المشكلات المكتشفة من خلال تعديلات على المصادقة والصلاحيات وفحوصات ملكية الموارد والتحقق من Tokens.

**Specific result:** TODO – Add the actual number of successful unauthorized accesses and identify the fixes that prevented them.

**النتيجة الفعلية:** TODO – أضف العدد الحقيقي لحالات الوصول غير المصرح بها، واذكر الإصلاحات التي منعتها.

---

## Part 7 – Reflection

### 1. Hardest Problem

**TODO: Describe the hardest problem you faced during Day 2.**

**أصعب مشكلة:** TODO – اكتب المشكلة التي واجهتك فعلياً أثناء تنفيذ Day 2.

### 2. What Would You Do Differently?

**TODO: Describe one thing you would do differently if you repeated Day 2.**

**ما الشيء الذي ستفعله بطريقة مختلفة؟** TODO – اكتب شيئاً واحداً كنت ستفعله بطريقة مختلفة لو أعدت تنفيذ Day 2.

---

## Final Day 2 Status

Day 2 covered identity design, customer and staff authentication, OTP verification, JWT access tokens, refresh tokens, current-user identification, resource ownership checks, authorization, security testing, and database reseeding.

غطى Day 2 تصميم الهوية، ومصادقة العملاء والموظفين، والتحقق باستخدام OTP، ورموز JWT، وRefresh Tokens، وتحديد المستخدم الحالي، وفحوصات ملكية الموارد والصلاحيات، واختبارات الأمان، وإعادة تهيئة قاعدة البيانات.

**Documentation status:** Remaining TODO entries require actual project measurements, test results, or personal reflection before the document is considered complete.

**حالة التوثيق:** ما زالت نقاط TODO تحتاج إلى القياسات ونتائج الاختبارات الفعلية أو إجاباتك الشخصية قبل اعتبار الملف مكتملاً.