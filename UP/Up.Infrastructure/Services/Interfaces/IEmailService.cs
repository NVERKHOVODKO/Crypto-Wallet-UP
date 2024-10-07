using Up.Core.DTO;

namespace UP.Services.Interfaces;

public interface IEmailService
{
    public Task SendVerificationCode(Guid id);
    
    public Task VerifyEmail(VerifyEmailRequest request);
    public Task RestorePassword(RestorePasswordRequest request);
    public Task SendMessageBlock(SendMessageRequest request);

    public Task SendEmailMessageTransactionsAsync(Guid id, string coinName, double quantity,
        bool isGet, Guid recieverId);
}