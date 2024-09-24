using BancoSqsAws.Models;
using BancoSqsAws.Data;
using System.Text.Json;
using Amazon.SQS;

namespace BancoSqsAws.Services;

public class SqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly AppDbContext _dbContext;
    private readonly string _queueUrl;

    public SqsService(IAmazonSQS sqsClient, AppDbContext dbContext)
    {
        _sqsClient = sqsClient;
        _dbContext = dbContext;
        _queueUrl = "YOUR_SQS_QUEUE_URL"; 
    }

    public async Task SendMessageAsync(MessageModel message)
    {
        var messageBody = JsonSerializer.Serialize(message);
        await _sqsClient.SendMessageAsync(_queueUrl, messageBody);
    }

    public async Task<MessageEntity> ReceiveMessageAsync()
    {
        var response = await _sqsClient.ReceiveMessageAsync(_queueUrl);
        if (response.Messages.Count > 0)
        {
            var message = JsonSerializer.Deserialize<MessageModel>(response.Messages[0].Body);

            var messageEntity = new MessageEntity
            {
                Content = message.Content,
                ReceivedAt = DateTime.UtcNow
            };

            _dbContext.Messages.Add(messageEntity);
            await _dbContext.SaveChangesAsync();

            await _sqsClient.DeleteMessageAsync(_queueUrl, response.Messages[0].ReceiptHandle);

            return messageEntity;
        }

        return null;
    }
}