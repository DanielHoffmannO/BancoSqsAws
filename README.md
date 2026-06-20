🌐 [English](README.en.md) | [Español](README.es.md)

# 🏦 BancoSqsAws

API REST em **.NET 8** que demonstra integração com **AWS SQS** (Simple Queue Service) — fila de mensagens com persistência local via EF Core + SQLite.

## 🎯 O que este projeto demonstra

| Conceito | Implementação |
|---|---|
| AWS SQS SDK | `AWSSDK.SQS` + `IAmazonSQS` via DI |
| Options Pattern | `IOptions<SqsSettings>` com config externalizada |
| Long Polling | `WaitTimeSeconds` configurável (reduz custos) |
| At-Least-Once Delivery | Delete da fila APÓS persistir no banco |
| Interface + DI | `ISqsService` → testabilidade |
| Structured Logging | `ILogger<T>` com contexto |
| CancellationToken | Propagado em toda cadeia async |
| Multi-stage Docker | Build otimizado com Alpine |
| Records (DTOs) | Request/Response imutáveis |

## 📁 Estrutura

```
BancoSqsAws/
├── src/
│   └── BancoSqsAws/
│       ├── Configuration/
│       │   └── SqsSettings.cs         # Options Pattern
│       ├── Controllers/
│       │   └── SqsController.cs       # Endpoints REST
│       ├── Services/
│       │   ├── ISqsService.cs         # Interface
│       │   └── SqsService.cs          # Implementação AWS
│       ├── Models/
│       │   ├── MessageModel.cs        # DTOs (records)
│       │   └── MessageEntity.cs       # Entidade EF Core
│       ├── Data/
│       │   └── AppDbContext.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── BancoSqsAws.csproj
├── Dockerfile
├── .dockerignore
├── .gitignore
├── BancoSqsAws.sln
└── README.md
```

## 🚀 Como Rodar

### Local

```bash
# Configurar AWS CLI
aws configure

# Editar src/BancoSqsAws/appsettings.json → SqsSettings.QueueUrl

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

## 📡 Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/sqs` | Envia mensagem para a fila SQS |
| `GET` | `/api/sqs` | Consome próxima mensagem (Long Polling) |

### POST `/api/sqs`

```json
// Request
{ "content": "Transferência de R$ 150,00 para conta 12345" }

// Response 200
{ "messageId": "abc-123-def", "status": "Mensagem enviada com sucesso" }
```

### GET `/api/sqs`

```json
// Response 200
{ "id": 1, "content": "Transferência de R$ 150,00", "receivedAt": "2024-01-15T10:30:00Z" }

// Response 204 — fila vazia
```

## 🏗️ Fluxo

```
[POST] Cliente → API → AWS SQS (enqueue)
[GET]  Cliente → API → AWS SQS (dequeue + Long Polling) → SQLite (persist) → Delete da fila
```

**At-Least-Once:** mensagem só é deletada APÓS persistir. Se falhar, SQS reenvia (visibility timeout).

## 🛠️ Stack

- .NET 8 / ASP.NET Core
- AWS SQS (`AWSSDK.SQS`)
- Entity Framework Core 8 + SQLite
- Docker (Alpine multi-stage)
- Swagger / OpenAPI

## 📄 Licença

Projeto de estudo e portfólio.
