using BancoSqsAws.Models;

namespace BancoSqsAws.Services;

public interface ISqsService
{
    Task<string> SendMessageAsync(SqsMessageRequest request, CancellationToken ct = default);
    Task<MessageResponse?> ReceiveMessageAsync(CancellationToken ct = default);
}
