/*using TestApplication.DTO;

namespace TestApplication.Services;

public interface IUserService
{
    public Task CreateUserAsync(CreateUserRequest request);
    public Task AddRoleToUserAsync(AddUserRoleRequest roleRequest);

    public Task<UserGetResponse> GetUser(Guid userId);

    //public Task<List<UserGetResponse>> GetUsers();
    public Task<List<UserGetResponse>> GetUsers(int pageNumber, int pageSize);

    public Task DeleteUserAsync(Guid userId);
    public Task EditLoginAsync(EditLoginRequest request);

    public Task<bool> IsLoginUniqueAsync(string login);
    public Task<bool> IsLoginUniqueForUserAsync(Guid userId, string login);
}*/

