# Day 2 – Identity & Authorization

## Part 0 – Authorization Matrix

### Customer Account Creation Decision     =>قرار إنشاء حساب العميل

# Customer accounts are created only after successful OTP verification.   
=>لا يتم إنشاء حسابات العملاء إلا بعد التحقق بنجاح من رمز التحقق لمرة واحدة OTP

# Reason:** The customer's phone number must be verified before creating the account.  
=> السبب:** يجب التحقق من رقم هاتف العميل قبل إنشاء الحساب.


## Part 1 – User Table Design

### 1. Table Structure => هيكل الجدول

# We use one table named `User` for all account types: Customer, Partner, and Admin.

=> نستخدم جدولاً واحداً اسمه `User` لجميع أنواع الحسابات: Customer وPartner وAdmin.

# Reason: All account types share common identity information, and the `Role` column identifies each account type.

=> السبب: جميع أنواع الحسابات تشترك في معلومات الهوية الأساسية، وعمود `Role` يحدد نوع كل حساب.

---

### 2. Phone Number Storage => مكان تخزين رقم الهاتف

# The customer's phone number is stored in the `User` table.

=> يتم تخزين رقم هاتف العميل في جدول `User`.

# Reason: The phone number is used for customer login and OTP verification.

=> السبب: يُستخدم رقم الهاتف لتسجيل دخول العميل والتحقق باستخدام OTP.

---

### 3. Primary Key Type => نوع المفتاح الأساسي

# The `User` table uses an `int` primary key.

=> يستخدم جدول `User` مفتاحاً أساسياً من نوع `int`.

# Reason: An integer ID is sufficient for this project and matches the existing database entities.

=> السبب: رقم التعريف من نوع Integer كافٍ لهذا المشروع ويتوافق مع الكيانات الموجودة في قاعدة البيانات.

---

### 4. Partner and Store Relationship => العلاقة بين Partner والمتجر

# Each Partner account is linked to one Store, and each Store has one Partner account.

=> يرتبط كل حساب Partner بمتجر واحد، ولكل متجر حساب Partner واحد.

# Reason: This is the chosen relationship for the current project design.

=> السبب: هذه هي العلاقة التي اخترناها لتصميم المشروع الحالي.

---

### 5. Password Hashing Algorithm => خوارزمية تشفير كلمات المرور

# We use ASP.NET Core `PasswordHasher<User>` to hash passwords.

=> نستخدم `PasswordHasher<User>` من ASP.NET Core لتجزئة كلمات المرور.

# Reason: It provides a built-in password hashing implementation designed for securely storing passwords.

=> السبب: يوفر تطبيقاً جاهزاً لتجزئة كلمات المرور ومصمماً لتخزينها بشكل آمن.

---

### 6. Salt => الملح العشوائي

# The password hashing implementation generates and stores a random salt automatically.

=> تقوم آلية تجزئة كلمة المرور بإنشاء ملح عشوائي وتخزينه تلقائياً.

# Reason: A unique salt helps protect passwords against precomputed hash attacks.

=> السبب: يساعد الملح الفريد على حماية كلمات المرور من هجمات الجداول المحسوبة مسبقاً.

---

### 7. Slow Password Hashing => بطء تجزئة كلمات المرور

# Password hashing uses PBKDF2, which performs many iterations to make password verification computationally expensive.

=> تستخدم تجزئة كلمات المرور خوارزمية PBKDF2، التي تنفذ عدداً كبيراً من التكرارات لجعل عملية التحقق من كلمة المرور مكلفة حسابياً.

# Reason: Slower password hashing makes large-scale password guessing more difficult.

=> السبب: جعل تجزئة كلمات المرور أبطأ يجعل تخمين عدد كبير من كلمات المرور أكثر صعوبة.

## Part 2 – Two Ways In, One Kind of Token
## الجزء الثاني – طريقتان لتسجيل الدخول، ونوع واحد من رموز الوصول

### 1. Authentication Flows
### 1. مسارات المصادقة

The application supports two authentication flows:

يدعم التطبيق طريقتين للمصادقة:

- **Customer:** Phone Number + OTP.
  
  **العميل:** رقم الهاتف + رمز التحقق لمرة واحدة (OTP).

- **Staff:** Email + Password (Partner and Admin).
  
  **الموظفون:** البريد الإلكتروني + كلمة المرور (الشريك والمسؤول).

Both flows use the same JWT Access Token service to generate access tokens.

تستخدم كلتا الطريقتين نفس خدمة JWT Access Token لإنشاء رموز الوصول.

### 2. Customer Authentication – OTP
### 2. مصادقة العميل – رمز التحقق لمرة واحدة (OTP)

Customers authenticate using their phone numbers and a One-Time Password (OTP).

يقوم العملاء بتسجيل الدخول باستخدام أرقام هواتفهم ورمز تحقق لمرة واحدة (OTP).

Implemented endpoints:

نقاط النهاية التي تم تنفيذها:

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/v1/auth/request-otp` | Request an OTP code<br/>طلب رمز تحقق |
| POST | `/api/v1/auth/verify-otp` | Verify the OTP code<br/>التحقق من رمز OTP |
| POST | `/api/v1/auth/complete-signup` | Complete customer registration<br/>إكمال تسجيل العميل |

OTP security rules:

قواعد أمان رمز التحقق (OTP):

- OTP codes are generated using `RandomNumberGenerator`.
  
  يتم إنشاء رموز OTP باستخدام `RandomNumberGenerator`.

- OTP codes contain six digits.
  
  تتكون رموز OTP من ستة أرقام.

- OTP expiration time is 5 minutes.
  
  تنتهي صلاحية رمز OTP بعد 5 دقائق.

- Maximum of 5 incorrect attempts.
  
  الحد الأقصى للمحاولات الخاطئة هو 5 محاولات.

- A minimum of 60 seconds between requests.
  
  يجب الانتظار 60 ثانية على الأقل بين كل طلب وآخر.

- Maximum of 5 requests per hour.
  
  الحد الأقصى هو 5 طلبات خلال ساعة واحدة.

- Only the latest OTP code is accepted.
  
  يتم قبول أحدث رمز OTP فقط.

- OTP codes are not returned in API responses.
  
  لا يتم إرجاع رموز OTP ضمن استجابات API.

New customer accounts are created only after successful OTP verification.

يتم إنشاء حسابات العملاء الجدد فقط بعد التحقق بنجاح من رمز OTP.

### 3. Staff Authentication – Email and Password
### 3. مصادقة الموظفين – البريد الإلكتروني وكلمة المرور

Partners and Admins authenticate using their email addresses and passwords.

يقوم الشركاء والمسؤولون بتسجيل الدخول باستخدام عناوين بريدهم الإلكتروني وكلمات المرور الخاصة بهم.

Endpoint:

نقطة النهاية:

`POST /api/v1/auth/staff-login`

Password verification uses ASP.NET Core `PasswordHasher<User>`.

يتم التحقق من كلمات المرور باستخدام `PasswordHasher<User>` في ASP.NET Core.

Invalid credentials return a generic `401 Unauthorized` response:

عند إدخال بيانات اعتماد غير صحيحة، يتم إرجاع استجابة عامة برمز `401 Unauthorized`:

`Invalid credentials.`

This prevents the API from revealing whether an email address exists.

وهذا يمنع API من كشف ما إذا كان عنوان البريد الإلكتروني موجوداً أم لا.

### 4. JWT Access Token
### 4. رمز الوصول JWT

Both authentication flows use the same `IJwtTokenService` to generate JWT Access Tokens.

تستخدم كلتا طريقتي المصادقة نفس `IJwtTokenService` لإنشاء رموز الوصول JWT.

JWT configuration:

إعدادات JWT:

| Setting | Value |
|---|---|
| Access Token Lifetime | 15 minutes<br/>15 دقيقة |
| Signing Algorithm | HMAC-SHA256<br/>خوارزمية التوقيع |
| Issuer | Configured in appsettings<br/>تم تحديده في appsettings |
| Audience | Configured in appsettings<br/>تم تحديده في appsettings |

JWT claims:

البيانات (Claims) الموجودة داخل JWT:

- `sub`: User ID.
  
  معرّف المستخدم.

- `role`: User role.
  
  دور المستخدم.

- `storeId`: Included for Partner accounts.
  
  يتم تضمينه لحسابات الشركاء.

- `customerId`: Included for Customer accounts.
  
  يتم تضمينه لحسابات العملاء.

JWT Bearer authentication is configured in ASP.NET Core, and Swagger supports Bearer token authorization.

تم إعداد مصادقة JWT Bearer في ASP.NET Core، كما يدعم Swagger استخدام رمز Bearer للمصادقة.

**Note:** Refresh Token creation, rotation, and revocation are implemented in Part 5.

**ملاحظة:** سيتم تنفيذ إنشاء Refresh Token وتجديده وإلغائه في الجزء الخامس (Part 5).

### 5. Partner Account Creation
### 5. إنشاء حساب الشريك

Only Admin users can create Partner accounts.

يمكن للمستخدمين الذين يمتلكون دور Admin فقط إنشاء حسابات الشركاء.

Endpoint:

نقطة النهاية:

`POST /api/v1/auth/create-partner`

Authorization:

الصلاحيات المطلوبة:

`[Authorize(Roles = "Admin")]`

Before creating a Partner account, the service checks:

قبل إنشاء حساب الشريك، تقوم الخدمة بالتحقق مما يلي:

- The requested Store exists.
  
  أن المتجر المطلوب موجود.

- The Store does not already have a Partner.
  
  أن المتجر لا يمتلك شريكاً بالفعل.

- The email address is not already registered.
  
  أن البريد الإلكتروني غير مسجل مسبقاً.

A successful request returns `201 Created`.

عند نجاح الطلب، يتم إرجاع `201 Created`.

Attempting to assign a second Partner to the same Store returns `409 Conflict`.

محاولة تعيين شريك ثانٍ لنفس المتجر تؤدي إلى إرجاع `409 Conflict`.

### 6. Product Management
### 6. إدارة المنتجات

The following endpoints were implemented:

تم تنفيذ نقاط النهاية التالية:

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/v1/products` | Create a product<br/>إنشاء منتج |
| PUT | `/api/v1/products/{id}` | Update a product<br/>تعديل منتج |

Product creation validates that the referenced Store and Category exist.

عند إنشاء منتج، يتم التحقق من وجود المتجر والتصنيف المرتبطين به.

Product updates validate that the product and requested Category exist.

عند تعديل منتج، يتم التحقق من وجود المنتج والتصنيف المطلوب.

Both operations return the updated or created product details.

كلتا العمليتين تعيدان تفاصيل المنتج الذي تم إنشاؤه أو تعديله.

### 7. Database Seeder
### 7. أداة تعبئة قاعدة البيانات

The DataSeeder was updated to create:

تم تحديث DataSeeder لإنشاء ما يلي:

- One Customer User for every seeded Customer.
  
  مستخدم من نوع Customer لكل عميل تتم إضافته بواسطة Seeder.

- One Partner User for every seeded Store.
  
  مستخدم من نوع Partner لكل متجر تتم إضافته بواسطة Seeder.

- Exactly one Admin User.
  
  مستخدم Admin واحد فقط.

Additional rules:

قواعد إضافية:

- Customer users are associated with their Customer records.
  
  يتم ربط مستخدمي Customer بسجلات العملاء الخاصة بهم.

- Partner users are associated with their Store records.
  
  يتم ربط مستخدمي Partner بسجلات المتاجر الخاصة بهم.

- Staff passwords are hashed using `PasswordHasher<User>`.
  
  يتم تشفير كلمات مرور الموظفين باستخدام `PasswordHasher<User>`.

- Seed credentials are loaded from configuration.
  
  يتم تحميل بيانات اعتماد Seeder من ملف الإعدادات.

- OTP codes are not seeded.
  
  لا تتم إضافة رموز OTP بواسطة Seeder.

### 8. Testing Results
### 8. نتائج الاختبار

The following operations were tested using `requests.http`:

تم اختبار العمليات التالية باستخدام `requests.http`:

| Test | Expected Result | Actual Result |
|---|---|---|
| Customer OTP authentication<br/>مصادقة العميل باستخدام OTP | Successful authentication<br/>تسجيل دخول ناجح | Passed<br/>ناجح |
| Customer signup after OTP verification<br/>تسجيل العميل بعد التحقق من OTP | Account created<br/>إنشاء الحساب | Passed<br/>ناجح |
| Duplicate customer signup<br/>محاولة تسجيل العميل مرتين | 409 Conflict | Passed<br/>ناجح |
| Staff login with valid credentials<br/>تسجيل دخول الموظف ببيانات صحيحة | 200 OK | Passed<br/>ناجح |
| Staff login with invalid credentials<br/>تسجيل دخول الموظف ببيانات خاطئة | 401 Unauthorized | Passed<br/>ناجح |
| Create Partner as Admin<br/>إنشاء شريك بواسطة Admin | 201 Created | Passed<br/>ناجح |
| Create Partner for a Store that already has a Partner<br/>إنشاء شريك لمتجر لديه شريك بالفعل | 409 Conflict | Passed<br/>ناجح |
| Create a new Store<br/>إنشاء متجر جديد | 201 Created | Passed<br/>ناجح |
| Create Product<br/>إنشاء منتج | 201 Created | Passed<br/>ناجح |
| Update Product<br/>تعديل منتج | 200 OK | Passed<br/>ناجح |

### 9. Part 2 Status
### 9. حالة الجزء الثاني

The authentication flows, JWT Access Token generation, Partner account creation, Product creation and updating, and Seeder modifications have been implemented and tested.

تم تنفيذ واختبار مسارات المصادقة، وإنشاء رموز الوصول JWT، وإنشاء حسابات الشركاء، وإنشاء المنتجات وتعديلها، والتعديلات على Seeder.

Refresh Token functionality is scheduled for Part 5.

تم تحديد تنفيذ وظائف Refresh Token للجزء الخامس (Part 5).

## Part 3 — One service that knows who is calling

## Part 3 — خدمة واحدة تعرف من هو المستخدم الحالي

### 1. ICurrentUser Interface

The `ICurrentUser` interface was created in the Data project. It provides five properties: `IsAuthenticated`, `UserId`, `Role`, `StoreId`, and `CustomerId`.

تم إنشاء الواجهة `ICurrentUser` داخل مشروع Data. وتوفر خمس خصائص: `IsAuthenticated` و`UserId` و`Role` و`StoreId` و`CustomerId`.

### 2. CurrentUser Implementation

The `CurrentUser` class was created in the API project and implements `ICurrentUser`. It reads the current user's information from `IHttpContextAccessor.HttpContext.User`, using the claims provided by the validated JWT token.

تم إنشاء الكلاس `CurrentUser` داخل مشروع API ويطبق الواجهة `ICurrentUser`. ويقرأ معلومات المستخدم الحالي من `IHttpContextAccessor.HttpContext.User` باستخدام الـ Claims الموجودة في JWT Token الذي تم التحقق منه.

### 3. Dependency Injection

`AddHttpContextAccessor()` and `AddScoped<ICurrentUser, CurrentUser>()` were registered in `Program.cs`. The Scoped lifetime ensures that the current user service is associated with the current request.

تم تسجيل `AddHttpContextAccessor()` و`AddScoped<ICurrentUser, CurrentUser>()` داخل `Program.cs`. ويضمن عمر Scoped ارتباط خدمة المستخدم الحالي بالطلب الحالي.

### 4. When Nobody Is Logged In

When nobody is logged in, `ICurrentUser` returns:

- `IsAuthenticated = false`
- `UserId = null`
- `Role = null`
- `StoreId = null`
- `CustomerId = null`

This is correct because there is no authenticated user, and the application must not invent a user identity.

عندما لا يكون هناك مستخدم مسجل الدخول، ترجع `ICurrentUser` القيم التالية:

- `IsAuthenticated = false`
- `UserId = null`
- `Role = null`
- `StoreId = null`
- `CustomerId = null`

وهذا صحيح لأنه لا يوجد مستخدم تم التحقق من هويته، ولا يجب على التطبيق اختراع هوية مستخدم غير حقيقية.

### 5. Who Did It When There Is No User?

For background or system operations without an authenticated user, the actor is considered a **System / Background Operation**, not a real user. No fake user ID is assigned.

بالنسبة للعمليات الخلفية أو عمليات النظام التي لا يوجد فيها مستخدم مسجل، نعتبر المنفذ **System / Background Operation** وليس مستخدماً حقيقياً. ولا نعطيه رقم مستخدم وهمياً.

### 6. Service Layer Independence

The Service project was searched for `HttpContext`, and no results were found. This confirms that the Service layer does not depend on HTTP context.

تم البحث عن `HttpContext` داخل مشروع Service ولم تظهر أي نتائج. وهذا يؤكد أن طبقة Service لا تعتمد على سياق HTTP.

### Status

Part 3 implementation and documentation completed.

تم الانتهاء من تنفيذ وتوثيق Part 3.
## Part 4 — Two Layers of Checks, on Every Endpoint

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

Centralizing ownership checks avoids duplicating authorization logic across multiple services and endpoints.

It also ensures that the same ownership rules are applied consistently throughout the application.

### 1. المكان المركزي لفحوصات الملكية

تم وضع جميع فحوصات ملكية الموارد في مكان مركزي اسمه `ResourceAuthorizationService`.

هذه الخدمة مسؤولة عن التحقق مما إذا كان المستخدم الحالي يملك صلاحية الوصول إلى مورد معين.

تستخدم الخدمة `ICurrentUser` لمعرفة هوية المستخدم الحالي، وتستخدم `DeliveryPlatformDbContext` للتحقق من ملكية المورد داخل قاعدة البيانات.

توفّر الخدمة الدوال التالية:

- `EnsureStoreAccessAsync`
- `EnsureProductAccessAsync`
- `EnsureCustomerAccessAsync`
- `EnsureOrderAccessAsync`

**لماذا اخترنا هذا الأسلوب؟**

وضع فحوصات الملكية في مكان مركزي يمنع تكرار منطق الصلاحيات في عدة خدمات وواجهات API.

كما يضمن تطبيق نفس قواعد الملكية بشكل موحّد في جميع أجزاء التطبيق.

---

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

### 2. مدى الثقة بقيمة `storeId` الموجودة في JWT

نستخدم قيمة `storeId` الموجودة في JWT لتحديد المتجر المرتبط بحساب الـ Partner.

لكننا لا نثق بهذه القيمة بشكل كامل؛ لأن ارتباط الـ Partner بالمتجر قد يتغير بينما يظل التوكن القديم صالحاً.

لذلك، عند كل عملية تحقق من ملكية المتجر، يقوم التطبيق بما يلي:

1. يقرأ قيمة `storeId` من توكن المستخدم الحالي.
2. يجلب قيمة `StoreId` الحالية للـ Partner من قاعدة البيانات.
3. يقارن قيمة قاعدة البيانات مع قيمة التوكن ومعرّف المتجر المطلوب.
4. يسمح بالوصول فقط إذا كانت جميع القيم متطابقة.

**لماذا؟**

هذا يمنع التوكن القديم من منح Partner صلاحية الوصول إلى متجر لم يعد مرتبطاً به.

وتبقى قاعدة البيانات هي المصدر الأساسي لمعرفة المتجر الحالي المرتبط بالـ Partner.
# Day 2 – Part 5: Refresh Tokens & Fast Seeding

# اليوم الثاني – الجزء الخامس: Refresh Tokens وإعادة تهيئة البيانات بسرعة

## 1. Refresh Token Storage

The RefreshToken table stores the hashed refresh token, its UserId, expiration date, revocation date, and replacement token ID.

يخزن جدول RefreshToken قيمة الـ Refresh Token بعد تشفيرها باستخدام SHA-256، بالإضافة إلى UserId وتاريخ الانتهاء وتاريخ الإلغاء ومعرّف الرمز البديل.

The original refresh token is sent to the client, while only its hash is stored in the database.

يتم إرسال الـ Refresh Token الأصلي إلى العميل، بينما يتم تخزين الـ Hash فقط في قاعدة البيانات.

## 2. Refresh Token Generation

Refresh tokens are generated using 32 cryptographically secure random bytes and converted to Base64.

يتم إنشاء Refresh Tokens باستخدام 32 بايت عشوائية آمنة تشفيرياً، ثم تحويلها إلى Base64.

Refresh tokens are not JWTs.

الـ Refresh Tokens ليست JWTs.

## 3. Token Rotation

When a valid refresh token is used, the system issues a new access token and a new refresh token.

عند استخدام Refresh Token صالح، يقوم النظام بإصدار Access Token جديد وRefresh Token جديد.

The old refresh token is revoked, and its ReplacedByTokenId references the new token.

يتم إلغاء الـ Refresh Token القديم، ويشير ReplacedByTokenId إلى الرمز الجديد.

## 4. Refresh Token Reuse Detection

If a revoked refresh token is reused, the system revokes all refresh tokens belonging to that user and returns HTTP 401 Unauthorized.

إذا تمت إعادة استخدام Refresh Token ملغى، يقوم النظام بإلغاء جميع Refresh Tokens الخاصة بالمستخدم، ثم يعيد HTTP 401 Unauthorized.

## 5. Logout

Logout requires authentication and accepts a refresh token.

تتطلب عملية تسجيل الخروج وجود مستخدم مسجل الدخول، وتستقبل Refresh Token.

The system revokes the submitted token only if it belongs to the authenticated user.

يقوم النظام بإلغاء الرمز المرسل فقط إذا كان تابعاً للمستخدم الذي قام بتسجيل الدخول.

## 6. Refresh and Logout Endpoints

- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`

## 7. Full Database Reseed

The full database reseed completed successfully.

تمت إعادة تهيئة قاعدة البيانات بالكامل بنجاح.

### Actual Seed Results

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

## 8. Why Is Seeding Fast?

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

## 9. Part 5 Completion

Refresh token generation, storage, rotation, reuse detection, logout, and full database reseeding have been implemented and tested.

تم تنفيذ واختبار إنشاء Refresh Tokens وتخزينها وتدويرها واكتشاف إعادة استخدامها وتسجيل الخروج وإعادة تهيئة قاعدة البيانات بالكامل.

**Part 5 completed.**

**اكتمل الجزء الخامس.**