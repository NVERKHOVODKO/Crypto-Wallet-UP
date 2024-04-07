using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApplication.DTO;
using UP.DTO;
using UP.Services.Interfaces;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    /// <summary>
    /// Отправляет код верификации на указанный email.
    /// </summary>
    /// <param name="request">Запрос на отправку кода верификации.</param>
    /// <returns>Результат операции отправки кода верификации.</returns>
    [HttpPost("sendVerificationCode")]
    [AllowAnonymous]

    public async Task<IActionResult> SendVerificationCode(SendVerificationCodeRequest request)
    {
        await _emailService.SendVerificationCode(request.Id);
        return Ok();
    }

    /// <summary>
    /// Подтверждает адрес электронной почты пользователя по коду верификации.
    /// </summary>
    /// <param name="request">Запрос на подтверждение адреса электронной почты.</param>
    /// <returns>Результат операции подтверждения адреса электронной почты.</returns>
    [HttpPost("verifyEmail")]
    [AllowAnonymous]

    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _emailService.VerifyEmail(request);
        return Ok();
    }
    
    /// <summary>
    /// Подтверждает восстановление пароля пользователя по коду верификации.
    /// </summary>
    /// <param name="request">Запрос на подтверждение восстановления пароля.</param>
    /// <returns>Результат операции подтверждения восстановления пароля.</returns>
    [HttpPost("confirm-restore-password")]
    [AllowAnonymous]

    public async Task<IActionResult> ConfirmRestorePassword([FromBody] VerifyEmailRequest request)
    {
        await _emailService.VerifyEmail(request);
        return Ok();
    }
    
    /// <summary>
    /// Восстанавливает пароль пользователя по запросу на восстановление пароля.
    /// </summary>
    /// <param name="request">Запрос на восстановление пароля.</param>
    /// <returns>Результат операции восстановления пароля пользователя.</returns>
    [HttpPatch("restore-password")]
    [AllowAnonymous]
    public async Task<IActionResult> RestorePassword([FromBody] RestorePasswordRequest request)
    {
        await _emailService.RestorePassword(request);
        return Ok();
    }
    
    /// <summary>
    /// Отправляет сообщение о блокировке пользователю.
    /// </summary>
    /// <param name="request">Запрос на отправку сообщения о блокировке.</param>
    /// <returns>Результат операции отправки сообщения о блокировке.</returns>
    [HttpPatch("send-message-block")]
    [AllowAnonymous]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        await _emailService.SendMessageBlock(request);
        return Ok();
    }
}