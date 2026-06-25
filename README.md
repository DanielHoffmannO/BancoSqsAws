[>] [English](README.en.md) | [Espanol](README.es.md)

# {&} BancoSqsAws

[![.NET CI](https://github.com/DanielHoffmannO/BancoSqsAws/actions/workflows/dotnet.yml/badge.svg)](https://github.com/DanielHoffmannO/BancoSqsAws/actions)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![AWS SQS](https://img.shields.io/badge/AWS-SQS-FF9900?logo=amazonaws)
![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

> API REST demonstrando integracao com AWS SQS -- filas de mensagens com persistencia local e padrao At-Least-Once Delivery.

## [*] Conceitos Demonstrados

| Conceito | Implementacao |
|----------|--------------|
| AWS SQS SDK | `AWSSDK.SQS` + `IAmazonSQS` via DI |
| Options Pattern | `IOptions<SqsSettings>` com config externalizada |
| Long Polling | `WaitTimeSeconds` configuravel (reduz custos) |
| At-Least-Once | Delete da fila APOS persistir no banco |
| Interface + DI | `ISqsService` -> testabilidade |
| Structured Logging | `ILogger<T>` com contexto |
| CancellationToken | Propagado em toda cadeia async |
| Multi-stage Docker | Build otimizado com Alpine |
| Records (DTOs) | Request/Response imutaveis |

## {=} Tech Stack

- .NET 8 / ASP.NET Core
- AWS SQS (`AWSSDK.SQS`)
- Entity Framework Core 8 + SQLite
- Docker (Alpine multi-stage)
- Swagger / OpenAPI

## [!] Como Rodar

### Local

```bash
aws configure
# Editar src/BancoSqsAws/appsettings.json -> SqsSettings.QueueUrl

dotnet run --project src/BancoSqsAws
# Swagger em http://localhost:5069
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

## [>] Endpoints

### POST `/api/sqs` -- Envia mensagem

```json
// Request
{ "content": "Transferencia de R$ 150,00 para conta 12345" }

// Response 200
{ "messageId": "abc-123-def", "status": "Mensagem enviada com sucesso" }
```

### GET `/api/sqs` -- Consome mensagem (Long Polling)

```json
// Response 200
{ "id": 1, "content": "Transferencia de R$ 150,00", "receivedAt": "2024-01-15T10:30:00Z" }

// Response 204 -- fila vazia
```

## {?} Fluxo

```
[POST] Cliente -> API -> AWS SQS (enqueue)
[GET]  Cliente -> API -> AWS SQS (dequeue + Long Polling) -> SQLite (persist) -> Delete da fila
```

**At-Least-Once:** mensagem so e deletada APOS persistir. Se falhar, SQS reenvia (visibility timeout).

## {/} Arquitetura

```
src/BancoSqsAws/
+-- Configuration/   <- SqsSettings (Options Pattern)
+-- Controllers/     <- SqsController (endpoints REST)
+-- Services/        <- ISqsService + SqsService (AWS)
+-- Models/          <- DTOs (records) + Entity EF Core
+-- Data/            <- AppDbContext
+-- Program.cs
```

## [$] Licenca

Este projeto esta sob a licenca [MIT](LICENSE).
