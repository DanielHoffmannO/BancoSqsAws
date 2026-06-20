🌐 [Português](README.md) | [English](README.en.md)

# 🏦 BancoSqsAws

API REST en **.NET 8** que demuestra integración con **AWS SQS** (Simple Queue Service) — cola de mensajes con persistencia local vía EF Core + SQLite.

## 🎯 Lo que demuestra este proyecto

| Concepto | Implementación |
|---|---|
| AWS SQS SDK | `AWSSDK.SQS` + `IAmazonSQS` vía DI |
| Options Pattern | `IOptions<SqsSettings>` con config externalizada |
| Long Polling | `WaitTimeSeconds` configurable (reduce costos) |
| At-Least-Once Delivery | Delete de la cola DESPUÉS de persistir en la base |
| Interface + DI | `ISqsService` → testabilidad |
| Structured Logging | `ILogger<T>` con contexto |
| CancellationToken | Propagado en toda la cadena async |
| Multi-stage Docker | Build optimizado con Alpine |
| Records (DTOs) | Request/Response inmutables |

## 🚀 Cómo Ejecutar

### Local

```bash
aws configure
dotnet run --project src/BancoSqsAws
# Swagger en http://localhost:5069
```

### Docker

```bash
docker build -t banco-sqs-aws .
docker run -p 8080:8080 \
  -e AWS_ACCESS_KEY_ID=xxx \
  -e AWS_SECRET_ACCESS_KEY=xxx \
  -e AWS_REGION=us-east-1 \
  banco-sqs-aws
```

## 📡 Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/sqs` | Envía mensaje a la cola SQS |
| `GET` | `/api/sqs` | Consume próximo mensaje (Long Polling) |

## 🏗️ Flujo

```
[POST] Cliente → API → AWS SQS (enqueue)
[GET]  Cliente → API → AWS SQS (dequeue + Long Polling) → SQLite (persist) → Delete de la cola
```

## 🛠️ Stack

- .NET 8 / ASP.NET Core
- AWS SQS (`AWSSDK.SQS`)
- Entity Framework Core 8 + SQLite
- Docker (Alpine multi-stage)
- Swagger / OpenAPI