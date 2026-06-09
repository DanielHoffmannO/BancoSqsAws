using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BancoSqsAws.Configuration;
using BancoSqsAws.Data;
using BancoSqsAws.Models;
using Microsoft.Extensions.Options;

namespace BancoSqsAws.Services;

public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly AppDbContext _dbContext;
    private readonly SqsSettings _settings;
    private readonly ILogger<SqsService> _logger;

    public SqsService(
        IAmazonSQS sqsClient,
        AppDbContext dbContext,
        IOptions<SqsSettings> settings,
        ILogger<SqsService> logger)
    {
        _sqsClient = sqsClient;
        _dbContext = dbContext;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(SqsMessageRequest request, CancellationToken ct = default)
    {
        var body = JsonSerializer.Serialize(request);

        _logger.LogInformation("Enviando mensagem para fila SQS: {QueueUrl}", _settings.QueueUrl);

        var response = await _sqsClient.SendMessageAsync(
            new SendMessageRequest
            {
                QueueUrl = _settings.QueueUrl,
                MessageBody = body
            }, ct);

        _logger.LogInformation("Mensagem enviada. MessageId: {MessageId}", response.MessageId);

        return response.MessageId;
    }

    public async Task<MessageResponse?> ReceiveMessageAsync(CancellationToken ct = default)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _settings.QueueUrl,
            MaxNumberOfMessages = _settings.MaxNumberOfMessages,
            WaitTimeSeconds = _settings.WaitTimeSeconds
        };

        _logger.LogInformation("Consultando fila SQS (Long Polling: {WaitTime}s)", _settings.WaitTimeSeconds);

        var response = await _sqsClient.ReceiveMessageAsync(request, ct);

        if (response.Messages.Count == 0)
        {
            _logger.LogInformation("Nenhuma mensagem disponível na fila");
            return null;
        }

        var sqsMessage = response.Messages[0];
        var parsed = JsonSerializer.Deserialize<SqsMessageRequest>(sqsMessage.Body);

        var entity = new MessageEntity
        {
            Content = parsed?.Content ?? string.Empty,
            ReceivedAt = DateTime.UtcNow,
            SqsMessageId = sqsMessage.MessageId
        };

        _dbContext.Messages.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        // Deleta da fila apenas APÓS persistir com sucesso (at-least-once delivery)
        await _sqsClient.DeleteMessageAsync(_settings.QueueUrl, sqsMessage.ReceiptHandle, ct);

        _logger.LogInformation(
            "Mensagem consumida e persistida. SqsMessageId: {SqsMessageId}, DbId: {DbId}",
            sqsMessage.MessageId, entity.Id);

        return new MessageResponse(entity.Id, entity.Content, entity.ReceivedAt);
    }
}
