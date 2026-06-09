namespace BancoSqsAws.Configuration;

/// <summary>
/// Configurações do AWS SQS carregadas do appsettings.json.
/// Demonstra o uso do Options Pattern para configuração externalizada.
/// </summary>
public class SqsSettings
{
    public const string SectionName = "SqsSettings";

    public string QueueUrl { get; set; } = string.Empty;
    public int MaxNumberOfMessages { get; set; } = 1;
    public int WaitTimeSeconds { get; set; } = 5;
}
