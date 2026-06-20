🌐 [Português](README.md) | [Español](README.es.md)

# 🏦 BancoSqsAws

REST API in **.NET 8** demonstrating integration with **AWS SQS** (Simple Queue Service) — message queue with local persistence via EF Core + SQLite.

## 🎯 What this project demonstrates

| Concept | Implementation |
|---|---|
| AWS SQS SDK | `AWSSDK.SQS` + `IAmazonSQS` via DI |
| Options Pattern | `IOptions<SqsSettings>` with externalized config |
| Long Polling | Configurable `WaitTimeSeconds` (reduces costs) |
| At-Least-Once Delivery | Queue delete AFTER persisting to database |
| Interface + DI | `ISqsService` → testability |
| Structured Logging | `ILogger<T>` with context |
| CancellationToken | Propagated throughout async chain |
| Multi-stage Docker | Optimized build with Alpine |
| Records (DTOs) | Immutable Request/Response |

## 🚀 How to Run

### Local

```bash
aws configure
dotnet run --project src/BancoSqsAws
# Swagger at http://localhost:5069
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

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/sqs` | Send message to SQS queue |
| `GET` | `/api/sqs` | Consume next message (Long Polling) |

## 🏗️ Flow

```
[POST] Client → API → AWS SQS (enqueue)
[GET]  Client → API → AWS SQS (dequeue + Long Polling) → SQLite (persist) → Delete from queue
```

## 🛠️ Stack

- .NET 8 / ASP.NET Core
- AWS SQS (`AWSSDK.SQS`)
- Entity Framework Core 8 + SQLite
- Docker (Alpine multi-stage)
- Swagger / OpenAPI