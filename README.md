# 🏦 BancoSqsAws

API REST em .NET 8 que simula um sistema bancário de mensageria utilizando **AWS SQS** para enfileiramento de mensagens e **SQLite** para persistência local.

## 📋 Sobre o Projeto

Este projeto demonstra a integração entre uma API ASP.NET Core e o serviço de filas da AWS (Simple Queue Service). O fluxo consiste em:

1. **Enviar mensagem** → publica na fila SQS
2. **Receber mensagem** → consome da fila SQS, persiste no banco SQLite e remove da fila

## 🛠️ Stack

| Tecnologia | Uso |
|---|---|
| .NET 8 | Framework principal |
| ASP.NET Core Web API | Endpoints REST |
| AWS SQS | Fila de mensagens |
| Entity Framework Core 8 | ORM |
| SQLite | Banco de dados local |
| Swagger | Documentação interativa da API |

## 📁 Estrutura

```
BancoSqsAws/
├── Controllers/
│   └── SqsController.cs      # Endpoints POST e GET
├── Services/
│   └── SqsService.cs         # Lógica de envio/recebimento SQS
├── Models/
│   ├── MessageModel.cs        # DTO de entrada
│   └── MessageEntity.cs       # Entidade persistida no banco
├── Data/
│   └── AppDbContext.cs        # DbContext (EF Core + SQLite)
├── Program.cs                 # Configuração e DI
├── appsettings.json           # Config AWS (região, profile)
└── BancoSqsAws.csproj        # Dependências do projeto
```

## 🚀 Como Rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [AWS CLI](https://aws.amazon.com/cli/) configurado com credenciais válidas
- Fila SQS criada no console AWS

### Configuração

1. Configure suas credenciais AWS:
   ```bash
   aws configure
   ```

2. No arquivo `Services/SqsService.cs`, substitua `YOUR_SQS_QUEUE_URL` pela URL da sua fila:
   ```csharp
   _queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789/sua-fila";
   ```

3. Execute:
   ```bash
   dotnet restore
   dotnet run
   ```

4. Acesse o Swagger: `http://localhost:5069`

## 📡 Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/sqs` | Envia mensagem para a fila SQS |
| `GET` | `/api/sqs` | Consome próxima mensagem da fila e persiste no banco |

### Exemplo de Request (POST)

```json
{
  "id": "msg-001",
  "content": "Transferência de R$ 150,00 para conta 12345"
}
```

### Exemplo de Response (GET)

```json
{
  "id": 1,
  "content": "Transferência de R$ 150,00 para conta 12345",
  "receivedAt": "2024-01-15T10:30:00Z"
}
```

## 🏗️ Arquitetura

```
Cliente → [POST /api/sqs] → AWS SQS (fila)
Cliente → [GET /api/sqs]  → AWS SQS (consume) → SQLite (persiste)
```

O padrão Producer/Consumer garante desacoplamento e resiliência — mensagens ficam na fila até serem processadas.

## 📄 Licença

Este projeto é apenas para fins de estudo e portfólio.
