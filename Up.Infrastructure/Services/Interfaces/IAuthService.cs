using AuthResponse = Up.Core.DTO.AuthResponse;

namespace UP.Services.Interfaces;

public interface IAuthService
{
    public Task<AuthResponse> GetTokenAsync(string email);
}

