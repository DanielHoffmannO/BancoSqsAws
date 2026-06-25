ðŸŒ [English](README.en.md) | [EspaÃ±ol](README.es.md)

# ðŸ¦ BancoSqsAws

[![.NET CI](https://github.com/DanielHoffmannO/BancoSqsAws/actions/workflows/dotnet.yml/badge.svg)](https://github.com/DanielHoffmannO/BancoSqsAws/actions/workflows/dotnet.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![AWS SQS](https://img.shields.io/badge/AWS-SQS-FF9900?logo=amazonsqs)
![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Alpine-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

> API REST demonstrando integraÃ§Ã£o com AWS SQS â€” filas de mensagens com persistÃªncia local e padrÃ£o At-Least-Once Delivery.

---

## ðŸŽ¯ Conceitos Demonstrados

| Conceito | DescriÃ§Ã£o |
|----------|-----------|
| **Message Queue** | ComunicaÃ§Ã£o assÃ­ncrona via AWS SQS |
| **Long Polling** | ReduÃ§Ã£o de chamadas vazias ao consumir mensagens |
| **At-Least-Once Delivery** | Garantia de entrega com delete explÃ­cito apÃ³s processamento |
| **Options Pattern** | ConfiguraÃ§Ã£o tipada via `IOptions<SqsSettings>` |
| **Dependency Injection** | `IAmazonSQS` registrado no container DI |
| **PersistÃªncia Local** | EF Core + SQLite para armazenar mensagens consumidas |
| **Multi-stage Build** | Docker Alpine otimizado para produÃ§Ã£o |

---

## ðŸ› ï¸ Tech Stack

- **.NET 8** â€” ASP.NET Core Web API
- **AWSSDK.SQS** â€” Client oficial AWS para SQS
- **Entity Framework Core** â€” ORM com provider SQLite
- **Swagger / OpenAPI** â€” DocumentaÃ§Ã£o interativa
- **Docker** â€” Multi-stage build com Alpine
- **GitHub Actions** â€” CI automatizado

---

## ðŸš€ Como Rodar

### PrÃ©-requisitos

- .NET 8 SDK
- Conta AWS com fila SQS criada (ou LocalStack)
- Docker (opcional)

### Local

```bash
# Clone o repositÃ³rio
git clone https://github.com/DanielHoffmannO/BancoSqsAws.git
cd BancoSqsAws

# Configure as credenciais AWS no appsettings.json ou variÃ¡veis de ambiente
# AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_REGION

# Execute
dotnet run
```

Acesse o Swagger em: `https://localhost:5001/swagger`

### Docker

```bash
# Build
docker build -t banco-sqs-aws .

# Run
docker run -p 8080:8080 \
  -e AWS_ACCESS_KEY_ID=your_key \
  -e AWS_SECRET_ACCESS_KEY=your_secret \
  -e AWS_REGION=us-east-1 \
  banco-sqs-aws
```

---

## ðŸ“¡ Endpoints

### `POST /api/sqs` â€” Enviar mensagem para a fila

**Request:**
```json
{
  "messageBody": "Pagamento de R$ 150,00 processado"
}
```

**Response (200):**
```json
{
  "messageId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "statusCode": 200
}
```

---

### `GET /api/sqs` â€” Consumir mensagem da fila (Long Polling)

**Response (200):**
```json
{
  "messageId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "body": "Pagamento de R$ 150,00 processado",
  "persistedAt": "2026-06-25T10:00:00Z"
}
```

**Response (204):** Nenhuma mensagem disponÃ­vel na fila.

---

## ðŸ”„ Fluxo

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  Client  â”‚â”€â”€POSTâ”€â–¶â”‚  API REST â”‚â”€â”€â”€â”€â”€â”€â–¶â”‚ AWS SQS  â”‚
â”‚          â”‚       â”‚           â”‚       â”‚  (Queue) â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”˜
                                            â”‚
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”            â”‚
â”‚  Client  â”‚â—€â”€200â”€â”€â”‚  API REST â”‚â—€â”€â”€â”€GETâ”€â”€â”€â”€â”€â”˜
â”‚          â”‚       â”‚     â”‚     â”‚   (Long Polling)
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”€â”¼â”€â”€â”€â”€â”€â”˜
                         â”‚
                         â–¼
                   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
                   â”‚  SQLite   â”‚  â† Persiste mensagem
                   â”‚    DB     â”‚
                   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                         â”‚
                         â–¼
                   Delete da fila SQS
                   (At-Least-Once âœ“)
```

---

## ðŸ—ï¸ Arquitetura

```
BancoSqsAws/
â”œâ”€â”€ Controllers/
â”‚   â””â”€â”€ SqsController.cs        # Endpoints POST e GET
â”œâ”€â”€ Models/
â”‚   â””â”€â”€ SqsSettings.cs          # Options Pattern
â”œâ”€â”€ Data/
â”‚   â””â”€â”€ AppDbContext.cs          # EF Core + SQLite
â”œâ”€â”€ Program.cs                   # DI, IAmazonSQS, EF Core
â”œâ”€â”€ Dockerfile                   # Multi-stage Alpine
â”œâ”€â”€ appsettings.json             # ConfiguraÃ§Ã£o SQS
â””â”€â”€ .github/workflows/
    â””â”€â”€ dotnet.yml               # CI Pipeline
```

---

## ðŸ“„ LicenÃ§a

Este projeto estÃ¡ licenciado sob a [MIT License](LICENSE).

---

## ðŸ‘¤ Autor

**Daniel Hoffmann**

[![GitHub](https://img.shields.io/badge/GitHub-DanielHoffmannO-181717?logo=github)](https://github.com/DanielHoffmannO)
