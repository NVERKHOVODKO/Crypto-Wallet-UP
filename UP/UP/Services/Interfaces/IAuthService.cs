using UP.DTO;
using AuthResponse = UP.DTO.AuthResponse;

namespace UP.Services.Interfaces;

public interface IAuthService
{
    public Task<AuthResponse> GetTokenAsync(string email);
}

