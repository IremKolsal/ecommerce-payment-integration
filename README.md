# **İREM BEYZA KOLSAL — ECommerce Case**
## 📦 E-Commerce Payment Integration (Balance Service)

Balance Management Service ile entegre, **.NET 8 Web API** tabanlı e‑commerce uygulaması.

---

## 🚀 Özellikler

- **CQRS + MediatR**
  - `CreateOrderCommand`, `CompleteOrderCommand`, `GetProductsQuery`
  - Ayrı `*Handler` ve `*Result` (record) tipleri
- **Repository Pattern**
  - `IOrderRepository` (Add / GetByExternalId / SaveChanges)
- **Infrastructure Gateway**
  - `BalanceClient` (typed **HttpClient** + **Polly**)
  - Upstream DTO’ları `Infrastructure.Balance.Models` altında **internal**
  - **ResponseEnvelope<T>**, **ResponseGuard** ve özel istisnalar:  
    `UpstreamServiceException`, `EmptyResponseException`, `PayloadMissingException`
- **AutoMapper**
  - `Application.Common.Mapping.MappingProfile` (Info ➜ QueryResult)
  - Infrastructure profile
- **Validation**: FluentValidation
- **Global Exception Middleware**
- **EF Core (Code-First) + PostgreSQL**
- **Swagger**, **Docker Compose**, **Polly (retry + circuit breaker)**
- **Unit Tests**
  - `ECommerce.Application.Tests` (handlers)
  - `ECommerce.Infrastructure.Tests` (BalanceClient; **HttpStub** ile sahte HTTP)

---

## 🗂️ Proje Yapısı

```
ECommerce/
├─ docker-compose.yml
├─ ECommerce.sln
├─ src/
│  ├─ ECommerce.Api/
│  ├─ ECommerce.Application/
│  │  ├─ Commands/ (CreateOrder, CompleteOrder)
│  │  ├─ Queries/  (GetProducts)
│  │  ├─ Abstractions/ (IBalanceClient, IOrderRepository, Models/*Info)
│  │  └─ Common/Mapping/MappingProfile.cs
│  ├─ ECommerce.Domain/ (Entities: Order, OrderItem)
│  └─ ECommerce.Infrastructure/
│     ├─ Balance/
│     │  ├─ BalanceClient.cs
│     │  ├─ Models/ (internal DTO’lar, Endpoints, ResponseEnvelope)
│     │  └─ Mapping/ (profile)
│     └─ Persistence/ (AppDbContext, repository implementation)
└─ tests/
   ├─ ECommerce.Application.Tests/
   │  ├─ Handlers/
   └─ ECommerce.Infrastructure.Tests/
      └─ Clients/ (+ Support/HttpStub.cs)
```

---

## ⚙️ Kurulum & Çalıştırma

### 🐳 Docker
```bash
docker compose up -d --build
```
- Swagger: **http://localhost:8081/swagger**  
- Postgres: **localhost:5434** (user: `postgres`, pass: `postgres`, db: `ecommerce`)

### 💻 Lokal (appsettings.json)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5434;Database=ecommerce;Username=postgres;Password=postgres"
  },
  "Balance": {
    "BaseUrl": "https://balance-management-pi44.onrender.com/api/"
  }
}
```
> **Dikkat:** `BaseUrl` **/api/** ile biter. `BalanceClient` kendi içinde `products`, `balance/preorder`, `balance/complete` path’lerini ekler.

Uygulama açılışında:
- Global exception middleware aktif
- DB migrate/ensure işlemleri otomatik

---

## 🔗 Balance Entegrasyonu (Özet)

- **Typed HttpClient**: `AddHttpClient<IBalanceClient, BalanceClient>(...)`
- **Polly**: Retry + Circuit Breaker
- **JSON**: case-insensitive + camelCase
- **Guard**: `ResponseGuard.EnsureOk()` ve `ThrowIfNull()` ile upstream hataları merkezi yönetim

---

## 🧠 CQRS Akışı

**Create (Pre-Order)**
1. `_balance.PreorderAsync(amount, orderId)`  
2. `Order` entity oluştur → repo `AddAsync`  
3. `SaveChangesAsync`  
4. `CreateOrderCommandResult` döner

**Complete**
1. Repo `GetByExternalIdAsync(orderId)`  
2. Durum kontrolü: **blocked** değilse hata  
3. `_balance.CompleteAsync(orderId)`  
4. Entity güncelle → `SaveChangesAsync`  
5. `CompleteOrderCommandResult` döner

**Products**
- `GetProductsQuery` → `_balance.GetProductsAsync()`  
- AutoMapper ile `GetProductsQueryResult` listesine map

---

## 🛣️ API Endpoint’leri (Örnekler)

### GET `/api/products`
Balance servisinden ürün listesi (proxy).
```http
GET http://localhost:8081/api/products
```
Örnek Response
```json
[
  {
    "id": "prod-001",
    "name": "Premium Smartphone",
    "description": "Latest model with advanced features",
    "price": 19.99,
    "currency": "USD",
    "category": "Electronics",
    "stock": 42
  }
]
```

### POST `/api/orders/create`
Pre-order + sipariş oluşturma.
```http
POST http://localhost:8081/api/orders/create
Content-Type: application/json
```
Body
```json
{ "amount": 15, "orderId": "preorder-001" }
```
Response
```json
{
  "id": "c94ecd9d-ca15-4842-a487-bd56fbdefc2e",
  "orderId": "preorder-001",
  "status": "blocked",
  "totalAmount": 15
}
```

### POST `/api/orders/{orderId}/complete`
Siparişi tamamlama (Balance complete).
```http
POST http://localhost:8081/api/orders/preorder-001/complete
```
Response
```json
{ "orderId": "preorder-001", "status": "completed" }
```

---

## 🧪 Unit Testler

- **Application.Tests**
  - Handlers (xUnit + Moq + AutoFixture/AutoMoq)
- **Infrastructure.Tests**
  - `BalanceClient`
    - **HttpStub** ile sahte `HttpClient` (deterministik JSON)

Çalıştır:
```bash
dotnet test
```

---

## 🧾 Hata Yönetimi

- **Global**: `UseGlobalExceptions()`  
- **Upstream**: `UpstreamServiceException`, `EmptyResponseException`, `PayloadMissingException`  

---

## 🛠️ Teknolojiler

- .NET 8 (ASP.NET Core Web API)
- MediatR, AutoMapper, FluentValidation
- EF Core + PostgreSQL
- Polly (Retry + Circuit Breaker)
- Docker + Docker Compose
- Swagger (Swashbuckle)
- xUnit, Moq, AutoFixture

---

## 📌 Notlar

- `OrderItem` şimdilik future use.  
- `ExternalOrderId` = Balance servisindeki `orderId`.  
- Upstream DTO’ları **Infrastructure**’da **internal** kalır; Application katmanı **Info** modelleriyle çalışır.
