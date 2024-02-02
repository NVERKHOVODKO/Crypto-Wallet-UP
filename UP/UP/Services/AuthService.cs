/*using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Analitique.BackEnd.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectX;
using ProjectX.Exceptions;
using ProjectX.Repository.Interfaces;
using Repository;
using TestApplication.DTO;
using TestApplication.Models;
using UserApi.Entities;

namespace TestApplication.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IDbRepository _dbRepository;
    private readonly IUserRepository _userRepository;


    public AuthService(IUserRepository userRepository, IConfiguration configuration, IAuthRepository authRepository, IDbRepository dbRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _authRepository = authRepository;
        _dbRepository = dbRepository;
    }

    public async Task<AuthResponse> GetTokenAsync(AuthRequest request)
    {
        var users = await _dbRepository.Get<UserModel>()
            .Where(x => x.Login == request.Login)
            .ToListAsync();

        var user = users.FirstOrDefault(x => x.Password == HashHandler.HashPassword(request.Password, x.Salt));
        
        if(user == null) throw new EntityNotFoundException("There is no such user");
        await _authRepository.RecordLoginAsync(user.Id);
        var token = await GenerateTokenAsync(user);
        
        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
        };
    }

    public async Task<JwtSecurityToken> GenerateTokenAsync(UserModel user)
    {
        var roles = await GetUserRoles(user.Login);
        var claims = new List<Claim>();
        if (roles != null && roles.Any())
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        
        claims.Add(new Claim("id", user.Id.ToString()));
        claims.Add(new Claim("name", user.Login));
        claims.Add(new Claim("openAIId", user.OpenAIApiId.ToString()));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        return new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: signIn);
    }
    
    
    public async Task<AuthResponse> GetTokenAsync(GetTokenRequest request)
    {
        var user = await _dbRepository.Get<UserModel>()
            .FirstOrDefaultAsync(x => x.Email == request.Email || x.Login == request.Email);
        if(user == null) throw new EntityNotFoundException("There is no such user");
        await _authRepository.RecordLoginAsync(user.Id);
        
        var token = await GenerateTokenAsync(user);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
        };
    }

    public async Task<bool> IsUserExists(AuthRequest request)
    {
        var userWithThisLogin = await _userRepository.GetUserModelAsync(request.Login);

        if (userWithThisLogin.Login == request.Login &&
            HashHandler.HashPassword(request.Password, userWithThisLogin.Salt) == userWithThisLogin.Password)
            return true;
        return false;
    }

    public async Task<List<string>> GetUserRoles(string login)
    {
        var userRoles = await _userRepository.GetUserRolesAsync(login);

        return userRoles;
    }
    
    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var IsLoginUnique = await _userRepository.IsEmailUniqueAsync(email);
        return IsLoginUnique;
    }
    
    public async Task VerifyEmail(VerifyEmailRequest request)
    {
        if (!IsEmailValid(request.Email)) throw new IncorrectDataException("Email isn't valid");
        if (!await IsEmailUniqueAsync(request.Email)) throw new IncorrectDataException("Email isn't unique");
        if (request.Code == null) throw new EntityNotFoundException("Code can't be null");
        var code = await _dbRepository.Get<EmailVerificationCodeModel>()
            .FirstOrDefaultAsync(x => x.Email == request.Email);
        if (code == null) throw new EntityNotFoundException("Wrong code");
        if (code.Code != request.Code) throw new AuthenticationException("Wrong code");
        DeleteEmailCodeAsync(request.Email);
    }
    
    public async Task<string> VerifyPasswordRestoring(VerifyEmailRequest request)
    {
        if (request.Code == null) throw new EntityNotFoundException("Code can't be null");
        var code = await _dbRepository.Get<RestorePasswordCodeModel>()
            .FirstOrDefaultAsync(x => x.Email == request.Email);
        if (code == null) throw new EntityNotFoundException("Wrong code");
        if (code.Code != request.Code) throw new AuthenticationException("Wrong code");

        var response = await GetTokenAsync(new GetTokenRequest
                                            {
                                                Email = request.Email
                                            });

        if (response.Token == null)
            throw new EntityNotFoundException("User not found");
        
        DeletePasswordCodeAsync(request.Email);
        return response.Token;
    }
    
    public async Task DeleteEmailCodeAsync(string email)
    {
        var code = await _dbRepository.Get<EmailVerificationCodeModel>().FirstOrDefaultAsync(x => x.Email == email);
        if (code == null) throw new EntityNotFoundException("Code not found");
        await _dbRepository.Delete<EmailVerificationCodeModel>(code.Id);
        await _dbRepository.SaveChangesAsync();
    }
    
    public async Task DeletePasswordCodeAsync(string email)
    {
        var code = await _dbRepository.Get<RestorePasswordCodeModel>().FirstOrDefaultAsync(x => x.Email == email);
        if (code == null) throw new EntityNotFoundException("Code not found");
        await _dbRepository.Remove<RestorePasswordCodeModel>(code);
        await _dbRepository.SaveChangesAsync();
    }

    public async Task SendRestorePasswordCode(string email)
    {
        if (!IsEmailValid(email)) throw new IncorrectDataException("Email isn't valid");
        /*var user = await _dbRepository.Get<UserModel>()
            .FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
            throw new EntityNotFoundException("User not found");#1#
        
        var existedCode = await _dbRepository.Get<RestorePasswordCodeModel>().FirstOrDefaultAsync(x => x.Email == email);
        var random = new Random();
        var code = random.Next(1000, 9999).ToString();
        if (existedCode != null)
        {
            existedCode.Code = code;
            await _dbRepository.SaveChangesAsync();
            SendRestorePasswordCodeAsync(email, code);// correct
            return;
        }
        var entity = new RestorePasswordCodeModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Code = code,
            DateCreated = DateTime.UtcNow,
        };
        var result = await _dbRepository.Add(entity);
        await _dbRepository.SaveChangesAsync();
        SendRestorePasswordCodeAsync(email, code);
    }
    
    public async Task SendVerificationCode(string email)
    {
        if (!IsEmailValid(email)) throw new IncorrectDataException("Email isn't valid");
        if (!await IsEmailUniqueAsync(email)) throw new IncorrectDataException("Email isn't unique");
        var existedCode = await _dbRepository.Get<EmailVerificationCodeModel>().FirstOrDefaultAsync(x => x.Email == email);
        var random = new Random();
        var code = random.Next(1000, 9999).ToString();
        if (existedCode != null)
        {
            existedCode.Code = code;
            await _dbRepository.SaveChangesAsync();
            SendVerivicationCodeAsync(email, code);// correct
            return;
        }
        var entity = new EmailVerificationCodeModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Code = code,
            DateCreated = DateTime.UtcNow,
        };
        var result = await _dbRepository.Add(entity);
        await _dbRepository.SaveChangesAsync();
        SendVerivicationCodeAsync(email, code);// correct
    }
    
    private async Task SendVerivicationCodeAsync(string email, string code)
    {
        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        mm.To.Add(email);
        mm.Subject = "Email confirmation";
        mm.Body = $"Hello!<br>" +
                  $"Thank you for registering on our website.<br><br>" +
                  $"To complete the confirmation process, please enter the following code: <strong style='font-size: 25px;'>{code}</strong><br><br><br>" +
                  $"Please do not reply to this message.";
        
        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "hors mfwv zsve lvye");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }
    
    private async Task SendRestorePasswordCodeAsync(string email, string code)
    {
        var mm = new MailMessage();
        var sc = new SmtpClient("smtp.gmail.com");
        mm.From = new MailAddress("mikita.verkhavodka@gmail.com");
        mm.To.Add(email);
        mm.Subject = "Restore password confirmation";
        mm.Body = code;
        mm.IsBodyHtml = true;
        sc.Port = 587;
        sc.Credentials = new NetworkCredential("mikita.verkhavodka@gmail.com", "hors mfwv zsve lvye");
        sc.EnableSsl = true;

        await sc.SendMailAsync(mm);
    }


    public bool IsEmailValid(string email)
    {
        if (email.Length > 100) throw new IncorrectDataException("Email isn't valid");
        var regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }
    
    
    public async Task RestorePassword(RestorePasswordRequest request, string userId)
    {
        int colonIndex = userId.IndexOf(':');
        string guidString = userId.Substring(colonIndex + 1).Trim();

        if (request.NewPassword == null) throw new IncorrectDataException("Password is empty");
        if (request.NewPassword.Length > 30) throw new IncorrectDataException("Password has to be shorter than 30 symbols");
        if (request.NewPassword.Length < 4) throw new IncorrectDataException("Password has to be longer than 4 symbols");
        
        var userToUpdate = await _dbRepository.Get<UserModel>()
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(guidString));

        if (userToUpdate == null)
            throw new EntityNotFoundException("User not found");
        
        userToUpdate.Password = HashHandler.HashPassword(request.NewPassword, userToUpdate.Salt);
        await _dbRepository.SaveChangesAsync();
    }
}*/