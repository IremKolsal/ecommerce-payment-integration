# İREM BEYZA KOLSAL ECommerce Case

# 📚 E-Commerce Payment Integration Challenge

Balance Management Service ile entegre, **.NET 8 Web API** tabanlı basit bir e-commerce uygulaması.

## ⏳ Zaman Kalsa Yapacaklarım

* Hata mesajlarını tek sınıfta toplamak
* Base response modeli
* Feature branch stratejisi
* CQRS + MediatR / Repository Pattern
* Unit testler (xUnit + Moq)
* Serilog, HealthChecks

## 🔧 Teknolojiler

* .NET 8 (ASP.NET Core Web API)
* PostgreSQL 17
* Entity Framework Core (Code-First + Migrations)
* FluentValidation
* Polly (retry + circuit breaker)
* Docker + Docker Compose
* Swagger (Swashbuckle)

---

## 📁 Proje Yapısı

```
ECommerce/
├─ docker-compose.yml
├─ Dockerfile
├─ ECommerce.sln
└─ src/
   ├─ ECommerce.Api/
   ├─ ECommerce.Application/
   ├─ ECommerce.Domain/
   └─ ECommerce.Infrastructure/
```

> `OrderItem` bu case’de aktif kullanılmıyor; geleceğe dönük bırakıldı.

---

## ⚙️ Kurulum & Çalıştırma

### 1) Docker Compose

```bash
docker compose up -d --build
```

* API: [http://localhost:8081/swagger](http://localhost:8081/swagger)
* DB: `localhost:5434` (user: postgres, pass: postgres, db: ecommerce)

### 2) Lokal

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5434;Database=ecommerce;Username=postgres;Password=postgres"
},
"Balance": {
  "BaseUrl": "https://balance-management-pi44.onrender.com/api/balance/"
}
```

---

## 🧱 Database & Migration

* Migrations **lokalde** oluşturuldu.
* Container içinde otomatik uygulanıyor (`Program.cs`):

---

## 🔗 Balance Service Entegrasyonu

* Base URL: `https://balance-management-pi44.onrender.com/api/balance/`
* Polly retry + circuit breaker kullanıldı.
* JSON case-insensitive parse edildi.

---

## 💨 Hata Yönetimi

Global `ExceptionMiddleware`:


## 🧠 Tasarım Kararları

* Katmanlı yapı (Api / Application / Domain / Infrastructure)
* BalanceClient + Polly
* Order tablosu (OrderItem future use)
* DTO’lar dış servise uyumlu

## 🛣️ Endpointler  
Localde çalıştırılabilecek örnek request ve responselar.  
Swagger UI: [http://localhost:8081/swagger](http://localhost:8081/swagger) (docker üzerinden de erişilebilir).  

---

### GET /api/products  
Balance servisinden ürün listesini getirir.  

**Request**  
```

GET [http://localhost:8081/api/products](http://localhost:8081/api/products)

````

**Response Body**  
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
````

---

### POST /api/orders/create

Yeni sipariş oluşturur ve Balance servisinden preorder çağırır.

**Request**

```
POST http://localhost:8081/api/orders/create
Content-Type: application/json
```

**Request Body**

```json
{
  "amount": 15,
  "orderId": "preorder-001"
}
```

**Response Body**

```json
{
  "id": "c94ecd9d-ca15-4842-a487-bd56fbdefc2e",
  "orderId": "preorder-001",
  "status": "blocked",
  "totalAmount": 15
}
```

---

### POST /api/orders/{orderId}/complete

Daha önce oluşturulmuş siparişi tamamlar (Balance complete çağrısı).

**Request**

```
POST http://localhost:8081/api/orders/preorder-001/complete
```

**Parameters**

* `orderId` (string, required, path): Tamamlanacak sipariş kimliği (örn: preorder-001).

**Response Body**

```json
{
  "orderId": "preorder-001",
  "status": "completed"
}
```

## 📝 Notlar

* `OrderItem` kullanılmıyor ama geleceğe yönelik duruyor.
* `ExternalOrderId` Balance servisindeki `orderId`.
* Migration dosyaları **lokalde** üretildi.

---
Docker Otomatik Migration Çalışmazsa (Fallback Planı)
-----------------------------------------------------
Nadir durumlarda Postgres container’ı geç hazır olabilir veya otomatik migrate uygulanmayabilir. Aşağıdaki adımlarla manuel uygulayabilirsiniz:
*Container’lar çalışsın (compose açık kalsın):
  docker compose up -d --build
*DB’de tablo var mı kontrol et (backslash gerektirmeyen sorgu):
  docker exec -it ecommerce-db psql -U postgres -d ecommerce -c "SELECT tablename FROM pg_tables WHERE schemaname='public';"
Çıktı boşsa (“Did not find any relations”) bir sonraki adıma geçin.

***Migration’ı lokalden, docker’daki DB’ye uygula:

dotnet ef database update ^
  --project src\ECommerce.Infrastructure ^
  --startup-project src\ECommerce.Api ^

Bu komut appsettings.json içindeki
Host=localhost;Port=5434;Database=ecommerce;Username=postgres;Password=postgres
bağlantısını kullanarak docker’daki Postgres’e migration’ı uygular.

Yeniden kontrol:

docker exec -it ecommerce-db psql -U postgres -d ecommerce -c "SELECT tablename FROM pg_tables WHERE schemaname='public';"

Gerekirse dotnet-ef aracını güncelleyin:
dotnet tool update --global dotnet-ef --version 8.0.0

## 🙌 Son Söz

Bu proje verilen süre içinde zorunlu gereksinimleri karşıladı.
Tüm testler `/swagger` üzerinden yapılabilir.
