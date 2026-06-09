# 🏦 BancoSqsAws

API REST em **.NET 8** que demonstra integração com **AWS SQS** (Simple Queue Service) — fila de mensagens com persistência local via EF Core + SQLite.

## 🎯 O que este projeto demonstra

| Conceito AWS / .NET | Implementação |
|---|---|
| AWS SQS SDK | `AWSSDK.SQS` com `IAmazonSQS` via DI |
| Options Pattern | `IOptions<SqsSettings>` com configuração externalizada |
| Long Polling | `WaitTimeSeconds` configurável para reduzir custos |
| At-Least-Once Delivery | Delete da fila APENAS após persistir no banco |
| Interface + DI | `ISqsService` registrada via `AddScoped` |
| Structured Logging | `ILogger<T>` com dados de contexto (MessageId, etc.) |
| CancellationToken | Propagado em toda a cadeia assíncrona |
| Records (DTOs) | Request/Response imutáveis |
| EF Core Migrations | Auto-apply na inicialização |

## 🛠️ Stack

- .NET 8 / ASP.NET Core Web API
- AWS SQS (SDK `AWSSDK.SQS`)
- Entity Framework Core 8 + SQLite
- Swagger / OpenAPI

## 📁 Estrutura

```
BancoSqsAws/
├── Configuration/
│   └── SqsSettings.cs         # Options Pattern (config tipada)
├── Controllers/
│   └── SqsController.cs       # Endpoints com ProducesResponseType
├── Services/
│   ├── ISqsService.cs         # Interface (contrato)
│   └── SqsService.cs          # Implementação com logging + error handling
├── Models/
│   ├── MessageModel.cs        # DTOs (records imutáveis)
│   └── MessageEntity.cs       # Entidade persistida
├── Data/
│   └── AppDbContext.cs        # DbContext
├── Program.cs                 # DI + Pipeline organizado
└── appsettings.json           # Config AWS + SQS + ConnectionString
```

## 🚀 Como Rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [AWS CLI](https://aws.amazon.com/cli/) configurado (`aws configure`)
- Fila SQS criada no console AWS

### Setup

1. Configure a URL da sua fila no `appsettings.json`:
   ```json
   "SqsSettings": {
     "QueueUrl": "https://sqs.us-east-1.amazonaws.com/123456789/sua-fila"
   }
   ```

2. Execute:
   ```bash
   dotnet run
   ```

3. Swagger em: `http://localhost:5069`

## 📡 Endpoints

### POST `/api/sqs` — Enviar mensagem

```json
// Request
{ "content": "Transferência de R$ 150,00 para conta 12345" }

// Response 200
{ "messageId": "abc-123-def", "status": "Mensagem enviada com sucesso" }
```

### GET `/api/sqs` — Consumir mensagem

```json
// Response 200
{ "id": 1, "content": "Transferência de R$ 150,00 para conta 12345", "receivedAt": "2024-01-15T10:30:00Z" }

// Response 204 (fila vazia)
```

## 🏗️ Fluxo

```
[POST] Cliente → API → AWS SQS (enqueue)
[GET]  Cliente → API → AWS SQS (dequeue + Long Polling) → SQLite (persist) → Delete da fila
```

**At-Least-Once:** A mensagem só é deletada da fila APÓS persistir no banco. Se o processo falhar, o SQS reenvia automaticamente (visibility timeout).

## 📄 Licença

Projeto de estudo e portfólio.
