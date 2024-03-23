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
    
    [HttpPost("sendVerificationCode")]
    [AllowAnonymous]
    public async Task<IActionResult> SendVerificationCode(SendVerificationCodeRequest request)
    {
        await _emailService.SendVerificationCode(request.Id);
        return Ok();
    }

    [HttpPost("verifyEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _emailService.VerifyEmail(request);
        return Ok();
    }
    
    /*[HttpPost("send-restore-password-code")]
    [AllowAnonymous]
    public async Task<IActionResult> SendRestorePasswordCode([FromBody] RestorePasswordCodeDto request)
    {
        await _emailService.SendRestorePasswordCode(request.Id);
        return Ok();
    }*/
    
    [HttpPost("confirm-restore-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmRestorePassword([FromBody] VerifyEmailRequest request)
    {
        await _emailService.VerifyEmail(request);
        return Ok();
    }
    
    /*[HttpPatch("restore-password")]
    [Authorize]
    public async Task<IActionResult> ConfirmRestorePassword([FromBody] RestorePasswordRequest request)
    {
        await _emailService.RestorePassword(request, User.FindFirst("id").ToString());
        return Ok();
    }*/
    
    [HttpPatch("restore-password")]
    [AllowAnonymous]
    public async Task<IActionResult> RestorePassword([FromBody] RestorePasswordRequest request)
    {
        await _emailService.RestorePassword(request);
        return Ok();
    }
    
    [HttpPatch("send-message-block")]
    [AllowAnonymous]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        await _emailService.SendMessageBlock(request);
        return Ok();
    }
}