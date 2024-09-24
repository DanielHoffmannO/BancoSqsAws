using System.ComponentModel.DataAnnotations;

namespace BancoSqsAws.Models;

public class MessageEntity
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime ReceivedAt { get; set; }
}
