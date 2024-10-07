using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Up.Core.DTO;
using Up.Core.Exceptions;
using Up.Core.Handlers;
using Up.Core.Models;
using Up.Infrastructure.Repositories.Interfaces;
using UP.Services.Interfaces;

namespace Up.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IDbRepository _dbRepository;

    public EmailService(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    

    public async Task VerifyEmail(VerifyEmailRequest request)
    {
        if (request.Code == null) throw new EntityNotFoundException("Code can't be null");
        var code = await _dbRepository.Get<EmailVerificationCodeModel>()
            .FirstOrDefaultAsync(x => x.UserId == request.Id);
        if (code == null) throw new EntityNotFoundException("Wrong code");
        if (code.Code != request.Code) throw new AuthenticationException("Wrong code");
        await DeleteEmailCodeAsync(request.Id);
    }

    private async Task DeleteEmailCodeAsync(Guid id)
    {
        var code = await _dbRepository.Get<EmailVerificationCodeModel>().FirstOrDefaultAsync(x => x.UserId == id);
        if (code == null) throw new EntityNotFoundException("Code not found");
        code.IsApproved = true;
        code.DateUpdated = DateTime.UtcNow;
        await _dbRepository.SaveChangesAsync();
    }

    public async Task SendVerificationCode(Guid id)
    {
        var existedCode =
            await _dbRepository.Get<EmailVerificationCodeModel>().FirstOrDefaultAsync(x => x.UserId == id);
        var random = new Random();
        var code = random.Next(1000, 9999).ToString();
        if (existedCode != null)
        {
            existedCode.Code = code;
            existedCode.DateUpdated = DateTime.UtcNow;
            await _dbRepository.SaveChangesAsync();
            await SendVerificationCodeAsync(id, code); // correct
            return;
        }
        var user = await _dbRepository.Get<User>(x => x.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new IncorrectDataException("Пользователь не найден");
        
        var entity = new EmailVerificationCodeModel
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            UserId = user.Id,
            Code = code,
            DateCreated = DateTime.UtcNow,
        };
        var result = await _dbRepository.Add(entity);
        await _dbRepository.SaveChangesAsync();
        await SendVerificationCodeAsync(id, code); // correct
    }

    private async Task SendVerificationCodeAsync(Guid id, string code)
    {
        var user = await _dbRepository.Get<User>(x => x.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new IncorrectDataException("Пользователь не найден");

        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        if (user.Email != null) 
            mm.To.Add(user.Email);
        mm.Subject = "Подтверждение изменения пароля";
        mm.Body = $$"""
                    
                            <html>
                            <head>
                                <style>
                                    /* CSS стили */
                                    body {
                                        font-family: Arial, sans-serif;
                                        color: #333;
                                        background-color: #f5f5f5;
                                    }
                                    .container {
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        background-color: #fff;
                                        border: 1px solid #ddd;
                                        border-radius: 5px;
                                    }
                                    h1 {
                                        color: #007bff;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1>Подтверждение изменения пароля</h1>
                                    <p>Привет!</p>
                                    <p>Не сообщайте код никому.</p>
                                    <p>Для завершения процесса подтверждения, введите следующий код: <strong style='font-size: 25px;'>{{code}}</strong></p>
                                    <p>Пожалуйста, не отвечайте на это сообщение.</p>
                                </div>
                            </body>
                            </html>
                            
                    """;

        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "lmqa tylg iawd ipuh");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }


    private static async Task SendRestorePasswordCodeAsync(string email, string code)
    {
        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        mm.To.Add(email);
        mm.Subject = "Restore password confirmation";
        mm.Body = code;
        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "lmqa tylg iawd ipuh");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }

    private static bool IsEmailValid(string email)
    {
        if (email.Length > 100) throw new IncorrectDataException("Email isn't valid");
        const string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }
    
    public async Task RestorePassword(RestorePasswordRequest request)
    {
        if (request.NewPassword == null) throw new IncorrectDataException("Password is empty");
        if (request.NewPassword.Length > 30)
            throw new IncorrectDataException("Password has to be shorter than 30 symbols");
        if (request.NewPassword.Length < 4)
            throw new IncorrectDataException("Password has to be longer than 4 symbols");
        var existedCode =
            await _dbRepository.Get<EmailVerificationCodeModel>().FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if(existedCode?.IsApproved == false)
            throw new IncorrectDataException("Email не подтвержден");
        
        var userToUpdate = await _dbRepository.Get<User>()
            .FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (userToUpdate == null)
            throw new EntityNotFoundException("User not found");

        userToUpdate.Password = HashHandler.HashPassword(request.NewPassword, userToUpdate.Salt);
        await _dbRepository.Remove(existedCode);
        await _dbRepository.SaveChangesAsync();
    }
    
    public async Task SendMessageBlock(SendMessageRequest request)
    {
        var user = await _dbRepository.Get<User>(x => x.Id == request.UserId).FirstOrDefaultAsync();
        if (user == null)
            throw new IncorrectDataException("Пользователь не найден");
    
        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        mm.To.Add(user.Email!);
        mm.Subject = "UP.Api";
        
        mm.Body = $@"
        <html>
        <head>
            <style>
                /* CSS стили */
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                    background-color: #f5f5f5;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #fff;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                }}
                h1 {{
                    color: #007bff;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Ваш аккаунт заблокирован</h1>
                <p>{request.Message}</p>
                <p>Действие отменить невозможно.</p>
                <p>Пожалуйста, не отвечайте на это сообщение.</p>
            </div>
        </body>
        </html>";

        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "lmqa tylg iawd ipuh");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }
    public async Task SendEmailMessageTransactionsAsync(Guid id, string coinName, double quantity, bool isGet, Guid recieverId)
    {
        var user = await _dbRepository.Get<User>(x => x.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new IncorrectDataException("Пользователь не найден");
    
        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        mm.To.Add(user.Email!);
        mm.Subject = "UP.Api";

        var transactionType = isGet ? "На ваш счет переведены" : "С вашего счета переведены";

        
        mm.Body = $@"
        <html>
        <head>
            <style>
                /* CSS стили */
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                    background-color: #f5f5f5;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #fff;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                }}
                h1 {{
                    color: #007bff;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Уведомление о транзакции</h1>
                <p>{transactionType} {quantity} {coinName} на кошелек {recieverId}.</p>
                <p>Действие отменить невозможно.</p>
                <p>Пожалуйста, не отвечайте на это сообщение.</p>
            </div>
        </body>
        </html>
    ";

        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "lmqa tylg iawd ipuh");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }
}