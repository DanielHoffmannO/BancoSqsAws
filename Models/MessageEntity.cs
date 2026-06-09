using System.ComponentModel.DataAnnotations;

namespace BancoSqsAws.Models;

public class MessageEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; }

    /// <summary>ID da mensagem no SQS para rastreabilidade.</summary>
    public string? SqsMessageId { get; set; }
}
