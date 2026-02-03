# Microservice Eğitim İçeriği

- [x] Architectural patterns
- [x] Microservice nedir?
- [x] Database per Service & Data Ownership
- [x] Category WebApi oluşturalım
- [x] Category model için Create/Read işlemleri
- [x] API Versioning
- [x] OpenAPI ve Scalar ile endpoint dokümantasyonu
- [x] Health Check ile uygulama sağlık kontrolü
- [x] CORS policy
- [x] Product WebApi oluşturalım
- [x] Product Create/Read işlemleri
- [x] Response Compression
- [x] Service Discovery Pattern (HashiCorp Consul)
- [x] Resilience Pattern (Polly)
- [x] Docker image oluşturma
- [x] Docker compose ile projeleri ayağa kaldıralım
- [x] Gateway nedir?
- [x] Ocelot nedir?
- [x] Gateway projesi oluşturup Ocelot yapısını kuralım
  - [x] RateLimit
  - [x] LoadBalance
  - [x] Service Discovery  
  - [x] Authentication
  - [x] Authorization
- [x] YARP ile Gateway
  - [x] LoadBalance
  - [x] RateLimit
  - [x] Authentication
  - [x] Authorization
  - [x] HealthCheck
- [x] Ocelot vs YARP
- [ ] Order WebAPI oluşturalım ve Create işlemi yapalım
- [ ] Payment WebAPI oluşturalım ve Create metodu yazalım
- [ ] Idempotency
- [ ] Transaction sorunu oluşturalım
- [ ] Sync (HTTP/gRPC) vs Async (Message Broker) Communication
- [ ] Saga Pattern
- [ ] Observability (with OpenTelemetry and Jaeger)
- [ ] Aspire

---

## Consul

### Docker komutu (Service Discovery)

```powershell
docker run -d --name consul -p 8500:8500 hashicorp/consul:latest
```

### NuGet Package

```dash
Steeltoe.Discovery.Consul
```

## Polly kütüphanesi BackoffType

```csharp
//🧩 DelayBackoffType Enum Türleri
//Constant    Her denemede sabit süre bekler.    Delay = 5s → 5s, 5s, 5s//Constant    Her denemede sabit süre bekler.    Delay = 5s → 5s, 5s, 5s
//Linear    Her denemede gecikme lineer (doğrusal) artar.    Delay = 5s → 5s, 10s, 15s//Linear    Her denemede gecikme lineer (doğrusal) artar.    Delay = 5s → 5s, 10s, 15s
//Exponential    Her denemede gecikme katlanarak (üstel) artar.    Delay = 5s → 5s, 10s, 20s, 40s//Exponential    Her denemede gecikme katlanarak (üstel) artar.    Delay = 5s → 5s, 10s, 20s, 40s
```

---

## Ocelot

```dash
Ocelot
Ocelot.Provider.Consul
```

```dash
https://ocelot.readthedocs.io/
```

---

## YARP

```dash
Yarp.ReverseProxy
```

```dash
https://learn.microsoft.com/tr-tr/aspnet/core/fundamentals/servers/yarp/getting-started?view=aspnetcore-10.0
```

---

## Docker CLI komutları

- Network komutları

```powershell
#docker network listele
docker network ls 

#kullanılmayan networkleri sil
docker network prune 

#yeni network oluştur
docker network create network_name
```

- Image ve container komutları

```powershell
#image dönüştürme - eğer docker file olan ana dizinde ise build komutu
docker build -t image_name . 

#image dönüştürme - eğer docker file alt dizinde ise
docker build -t image_name -f Microservice.ProductWebAPI/Dockerfile . 

#container oluşturma
docker run -d --name container_name -p 6001:8080 image_adi

#networke bağlı container oluşturma
docker run -d --network eticaret --name product -p 6001:8080 productapi 
```

- docker compose build

```powershell
#eğer ilk oluşturuyorsak
docker compose up -d

#eğer tekrar rebuild yapacaksak
docker compose up -d --build
```
