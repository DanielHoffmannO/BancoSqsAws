🌐 [English](README.en.md) | [Español](README.es.md)

# 🏦 BancoSqsAws

[![.NET CI](https://github.com/DanielHoffmannO/BancoSqsAws/actions/workflows/dotnet.yml/badge.svg)](https://github.com/DanielHoffmannO/BancoSqsAws/actions/workflows/dotnet.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![AWS SQS](https://img.shields.io/badge/AWS-SQS-FF9900?logo=amazonsqs)
![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Alpine-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

> API REST demonstrando integração com AWS SQS — filas de mensagens com persistência local e padrão At-Least-Once Delivery.

---

## 🎯 Conceitos Demonstrados

| Conceito | Descrição |
|----------|-----------|
| **Message Queue** | Comunicação assíncrona via AWS SQS |
| **Long Polling** | Redução de chamadas vazias ao consumir mensagens |
| **At-Least-Once Delivery** | Garantia de entrega com delete explícito após processamento |
| **Options Pattern** | Configuração tipada via `IOptions<SqsSettings>` |
| **Dependency Injection** | `IAmazonSQS` registrado no container DI |
| **Persistência Local** | EF Core + SQLite para armazenar mensagens consumidas |
| **Multi-stage Build** | Docker Alpine otimizado para produção |

---

## 🛠️ Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **AWSSDK.SQS** — Client oficial AWS para SQS
- **Entity Framework Core** — ORM com provider SQLite
- **Swagger / OpenAPI** — Documentação interativa
- **Docker** — Multi-stage build com Alpine
- **GitHub Actions** — CI automatizado

---

## 🚀 Como Rodar

### Pré-requisitos

- .NET 8 SDK
- Conta AWS com fila SQS criada (ou LocalStack)
- Docker (opcional)

### Local

```bash
# Clone o repositório
git clone https://github.com/DanielHoffmannO/BancoSqsAws.git
cd BancoSqsAws

# Configure as credenciais AWS no appsettings.json ou variáveis de ambiente
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

## 📡 Endpoints

### `POST /api/sqs` — Enviar mensagem para a fila

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

### `GET /api/sqs` — Consumir mensagem da fila (Long Polling)

**Response (200):**
```json
{
  "messageId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "body": "Pagamento de R$ 150,00 processado",
  "persistedAt": "2026-06-25T10:00:00Z"
}
```

**Response (204):** Nenhuma mensagem disponível na fila.

---

## 🔄 Fluxo

```
┌──────────┐       ┌───────────┐       ┌──────────┐
│  Client  │──POST─▶│  API REST │──────▶│ AWS SQS  │
│          │       │           │       │  (Queue) │
└──────────┘       └───────────┘       └────┬─────┘
                                            │
┌──────────┐       ┌───────────┐            │
│  Client  │◀─200──│  API REST │◀───GET─────┘
│          │       │     │     │   (Long Polling)
└──────────┘       └─────┼─────┘
                         │
                         ▼
                   ┌───────────┐
                   │  SQLite   │  ← Persiste mensagem
                   │    DB     │
                   └───────────┘
                         │
                         ▼
                   Delete da fila SQS
                   (At-Least-Once ✓)
```

---

## 🏗️ Arquitetura

```
BancoSqsAws/
├── Controllers/
│   └── SqsController.cs        # Endpoints POST e GET
├── Models/
│   └── SqsSettings.cs          # Options Pattern
├── Data/
│   └── AppDbContext.cs          # EF Core + SQLite
├── Program.cs                   # DI, IAmazonSQS, EF Core
├── Dockerfile                   # Multi-stage Alpine
├── appsettings.json             # Configuração SQS
└── .github/workflows/
    └── dotnet.yml               # CI Pipeline
```

---

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

## 👤 Autor

**Daniel Hoffmann**

[![GitHub](https://img.shields.io/badge/GitHub-DanielHoffmannO-181717?logo=github)](https://github.com/DanielHoffmannO)
