İREM BEYZA KOLSAL — ECommerce Case
📦 E-Commerce Payment Integration (Balance Service)

Balance Management Service ile entegre, .NET 8 Web API tabanlı e-commerce uygulaması.

🚀 Neler Var?

CQRS + MediatR

CreateOrderCommand, CompleteOrderCommand, GetProductsQuery

Her biri için ayrı *Handler ve *Result (record) tipleri

Repository Pattern

IOrderRepository (Add/GetByExternalId/SaveChanges)

Infrastructure Gateway

BalanceClient (typed HttpClient + Polly)

Upstream DTO’ları Infrastructure.Balance.Models içinde internal tutulur

ResponseEnvelope<T> + ResponseGuard + özel exception’lar
(UpstreamServiceException, EmptyResponseException, PayloadMissingException)

AutoMapper

MappingProfile (Application) — Info → QueryResult

(Infrastructure tarafında da profil var; Balance payload → Info mapping’i destekler)

Validation

FluentValidation

Global Exception Middleware

EF Core (Code-First) + PostgreSQL

Swagger, Docker Compose, Polly (retry + circuit breaker)

Unit Tests

ECommerce.Application.Tests (handler’lar)

ECommerce.Infrastructure.Tests (BalanceClient; HttpStub ile sahte HTTP)

🗂️ Proje Yapısı
ECommerce/
├─ docker-compose.yml
├─ ECommerce.sln
├─ src/
│  ├─ ECommerce.Api/
│  ├─ ECommerce.Application/
│  │  ├─ Commands/
│  │  ├─ Queries/
│  │  ├─ Abstractions/ (IBalanceClient, IOrderRepository, Models/*Info)
│  │  └─ Common/Mapping/MappingProfile.cs
│  ├─ ECommerce.Domain/ (Entities: Order, OrderItem)
│  └─ ECommerce.Infrastructure/
│     ├─ Balance/
│     │  ├─ BalanceClient.cs
│     │  ├─ Models/ (internal DTO’lar, Endpoints)
│     │  └─ Mapping/ (profile)
│     └─ Persistence/ (AppDbContext, repo impl.)
└─ tests/
   ├─ ECommerce.Application.Tests/
   │  ├─ Handlers/
   └─ ECommerce.Infrastructure.Tests/
      └─ Clients/ (+ Support/HttpStub.cs)


⚙️ Kurulum & Çalıştırma
Docker
docker compose up -d --build


Swagger: http://localhost:8081/swagger

Postgres: localhost:5434 (user: postgres, pass: postgres, db: ecommerce)

Lokal (appsettings.json)
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5434;Database=ecommerce;Username=postgres;Password=postgres"
},
"Balance": {
  "BaseUrl": "https://balance-management-pi44.onrender.com/api/"
}


Dikkat: BaseUrl /api/ ile biter. BalanceClient kendi içinde balance/preorder, balance/complete, products gibi path’leri ekler.

Uygulama açılışında:

Global exception middleware aktif

DB migrate/ensure işlemleri otomatik

🔗 Balance Entegrasyonu

Typed HttpClient: AddHttpClient<IBalanceClient, BalanceClient>(...)

Polly: Retry + Circuit Breaker

JSON: case-insensitive + camelCase policy

Guard: ResponseGuard.EnsureOk, ThrowIfNull ile upstream hataları tek yerden yönetilir.

🧠 CQRS Akışı (Özet)

Create
CreateOrderCommand(amount, orderId) → handler

_balance.PreorderAsync(amount, orderId)

Order entity oluştur + repo AddAsync

SaveChangesAsync

CreateOrderCommandResult döner

Complete
CompleteOrderCommand(orderId) → handler

repo’dan GetByExternalIdAsync

state guard (blocked değilse hata)

_balance.CompleteAsync(orderId)

entity güncelle + SaveChangesAsync

CompleteOrderCommandResult döner

Products
GetProductsQuery → _balance.GetProductsAsync()
→ AutoMapper ile GetProductsQueryResult listesine map

🛣️ API Endpoint’leri (Örnekler)
GET /api/products

Balance servisinden ürün listesi (proxy).

GET http://localhost:8081/api/products


Örnek Response

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

POST /api/orders/create

Pre-order + sipariş oluşturma.

POST http://localhost:8081/api/orders/create
Content-Type: application/json


Body

{ "amount": 15, "orderId": "preorder-001" }


Response

{
  "id": "c94ecd9d-ca15-4842-a487-bd56fbdefc2e",
  "orderId": "preorder-001",
  "status": "blocked",
  "totalAmount": 15
}

POST /api/orders/{orderId}/complete

Siparişi tamamlama (Balance complete).

POST http://localhost:8081/api/orders/preorder-001/complete


Response

{ "orderId": "preorder-001", "status": "completed" }

🧪 Unit Testler

Application.Tests

Handler/Query testleri (xUnit + Moq + AutoFixture/AutoMoq)

SaveChangesAsync imzası Task<int> ise mock: .ReturnsAsync(1)

Infrastructure.Tests

BalanceClient

HttpStub ile sahte HttpClient (deterministik JSON)

IMapper mock (internal DTO’lara referans yok)

Success + error flow’ları (400, upstream error, empty body)

Çalıştır:

dotnet test

🧾 Hata Yönetimi

Global: UseGlobalExceptions()

Upstream: UpstreamServiceException, EmptyResponseException, PayloadMissingException

(İsteğe bağlı) AppErrors helper ile tutarlı mesaj üretimi

🛠️ Teknolojiler

.NET 8 (ASP.NET Core Web API)

MediatR, AutoMapper, FluentValidation

EF Core + PostgreSQL

Polly (Retry + Circuit Breaker)

Docker + Docker Compose

Swagger (Swashbuckle)

xUnit, Moq, AutoFixture

📌 Notlar

OrderItem şimdilik future use.

ExternalOrderId = Balance servisindeki orderId.

Upstream DTO’ları Infrastructure içinde internal; uygulama katmanı Info modelleriyle çalışır.

Hazır 🎯
Her şey docker veya lokal konfig ile ayağa kalkar; testler dotnet test ile geçer.