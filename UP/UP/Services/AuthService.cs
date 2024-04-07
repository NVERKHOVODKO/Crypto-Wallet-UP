using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Analitique.BackEnd.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectX.Exceptions;
using Repository;
using UP.DTO;
using UP.ModelsEF;
using UP.Services.Interfaces;
using AuthResponse = UP.DTO.AuthResponse;

namespace UP.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IDbRepository _dbRepository;


    public AuthService(IConfiguration configuration, IDbRepository dbRepository)
    {
        _configuration = configuration;
        _dbRepository = dbRepository;
    }

    public async Task<AuthResponse> GetTokenAsync(AuthRequest request)
    {
        var users = await _dbRepository.Get<User>()
            .Where(x => x.Login == request.Login)
            .ToListAsync();

        var user = users.FirstOrDefault(x => x.Password == HashHandler.HashPassword(request.Password, x.Salt));

        if(user == null) throw new EntityNotFoundException("There is no such user");

        var token = await GenerateTokenAsync(user);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
        };
    }

    private Task<JwtSecurityToken> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new ("id", user.Id.ToString()),
            new ("name", user.Login),
            new ("role", "admin")
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return Task.FromResult(new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: signIn));
    }

    public async Task<AuthResponse> GetTokenAsync(string email)
    {
        var user = await _dbRepository.Get<User>()
            .FirstOrDefaultAsync(x => x.Email == email || x.Login == email);
        if(user == null) throw new EntityNotFoundException("There is no such user");

        var token = await GenerateTokenAsync(user);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
}