using TestApplication.DTO;

namespace UP.Migrations.Services.Interfaces;

public interface IEmailService
{
    public Task SendVerificationCode(Guid id);
    public Task VerifyEmail(VerifyEmailRequest request);
    public Task RestorePassword(RestorePasswordRequest request);
}