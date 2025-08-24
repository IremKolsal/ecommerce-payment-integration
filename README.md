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

---
## 🛣️ Endpointler
Localde çalıştırılabilecek örnek request ve responselar
Swagger UI: http://localhost:8081/swagger(docker üzerinden de erişilebilir).
********************************************************
***GET /api/products***
Balance servisinden ürün listesini getirir.
Request
GET http://localhost:8081/api/products


***POST /api/Orders/create***
Request
URL
https://localhost:7289/api/Orders/create
Method
POST
---Request Body---
{
  "amount": 15,
  "orderId": "preorder-001"
}
-----------------------
Response Body
{
  "id": "c94ecd9d-ca15-4842-a487-bd56fbdefc2e",
  "orderId": "preorder-001",
  "status": "blocked",
  "totalAmount": 15
}
------------------------
**************************************************
***POST /api/Orders/{orderId}/complete***
Request
URL
https://localhost:7289/api/Orders/preorder-001/complete
Method
POST
Parameters
orderId (string, required, path): The ID of the order to complete (e.g., preorder-001).
---Request Body---
preorder-001
-------------------------------
Response Body
{
  "orderId": "preorder-001",
  "status": "completed"
}
----------------------------------

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
