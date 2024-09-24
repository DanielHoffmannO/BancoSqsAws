using Microsoft.AspNetCore.Mvc;
using BancoSqsAws.Services;
using BancoSqsAws.Models;

namespace BancoSqsAws.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SqsController : ControllerBase
{
    private readonly SqsService _sqsService;

    public SqsController(SqsService sqsService)
    {
        _sqsService = sqsService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] MessageModel message)
    {
        await _sqsService.SendMessageAsync(message);
        return Ok("Mensagem enviada com sucesso!");
    }

    [HttpGet]
    public async Task<IActionResult> ReceiveMessage()
    {
        var message = await _sqsService.ReceiveMessageAsync();
        if (message == null)
            return NotFound("Nenhuma mensagem encontrada.");

        return Ok(message);
    }
}
