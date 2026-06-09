namespace BancoSqsAws.Models;

/// <summary>
/// DTO para envio de mensagem à fila SQS.
/// </summary>
public record SqsMessageRequest(string Content);

/// <summary>
/// DTO de resposta com dados da mensagem consumida.
/// </summary>
public record MessageResponse(int Id, string Content, DateTime ReceivedAt);
