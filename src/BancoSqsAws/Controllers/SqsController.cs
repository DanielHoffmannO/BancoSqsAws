using BancoSqsAws.Models;
using BancoSqsAws.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSqsAws.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SqsController : ControllerBase
{
    private readonly ISqsService _sqsService;

    public SqsController(ISqsService sqsService)
    {
        _sqsService = sqsService;
    }

    /// <summary>
    /// Envia uma mensagem para a fila AWS SQS.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(
        [FromBody] SqsMessageRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "Content é obrigatório." });

        var messageId = await _sqsService.SendMessageAsync(request, ct);

        return Ok(new { messageId, status = "Mensagem enviada com sucesso" });
    }

    /// <summary>
    /// Consome a próxima mensagem da fila SQS e persiste no banco.
    /// Utiliza Long Polling para reduzir chamadas desnecessárias.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReceiveMessage(CancellationToken ct)
    {
        var message = await _sqsService.ReceiveMessageAsync(ct);

        if (message is null)
            return NoContent();

        return Ok(message);
    }
}
