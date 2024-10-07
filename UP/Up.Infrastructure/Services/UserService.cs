/*using System.Text.RegularExpressions;
using Analitique.BackEnd.Handlers;
using ProjectX;
using ProjectX.Exceptions;
using ProjectX.Repository.Interfaces;
using TestApplication.Controllers;
using TestApplication.DTO;
using TestApplication.Models;

namespace TestApplication.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, ILogger<UserController> logger)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        if (request.Login == null || request.Password == null || request.Email == null)
            throw new IncorrectDataException("Fill in all details");
        if (request.Login.Length > 20) throw new IncorrectDataException("Login has to be shorter than 20 symbols");
        if (request.Login.Length < 4) throw new IncorrectDataException("Login has to be longer than 4 symbols");
        if (request.Password.Length > 40)
            throw new IncorrectDataException("Password has to be shorter than 40 symbols");
        if (request.Password.Length < 4) throw new IncorrectDataException("Password has to be longer than 4 symbols");
        if (!IsEmailValid(request.Email)) throw new IncorrectDataException("Email isn't valid");
        if (!await IsEmailUniqueAsync(request.Email)) throw new IncorrectDataException("Email isn't unique");
        if (!await IsLoginUniqueAsync(request.Login)) throw new IncorrectDataException("Login isn't unique");
        var entity = new UserModel
        {
            Login = request.Login,
            Password = request.Password,
            Email = request.Email,
            Salt = HashHandler.GenerateSalt(30),
            OpenAIApiId = Guid.NewGuid()
        };
        await _userRepository.CreateAsync(entity);
    }


    public async Task EditLoginAsync(EditLoginRequest request)
    {
        if (request.NewLogin == null || request.UserId == null) throw new IncorrectDataException("Fill in all details");
        if (request.NewLogin.Length > 20) throw new IncorrectDataException("Login has to be shorter than 20 symbols");
        if (request.NewLogin.Length < 4) throw new IncorrectDataException("Login has to be longer than 4 symbols");
        if (!await IsLoginUniqueForUserAsync(request.UserId, request.NewLogin))
            throw new IncorrectDataException("Login isn't unique");
        var userToUpdate = await _userRepository.GetUserModelAsync(request.UserId);
        if (userToUpdate != null)
            await _userRepository.EditLoginAsync(userToUpdate, request.NewLogin);
        else
            _logger.LogInformation($"User with Id {request.UserId} was not found.");
    }

    public async Task AddRoleToUserAsync(AddUserRoleRequest roleRequest)
    {
        await _userRepository.AddRoleToUserAsync(roleRequest);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetUserModelAsync(userId);
        if (user == null) throw new EntityNotFoundException("User not found");
        await _userRepository.DeleteAsync(user);
    }


    public async Task<UserGetResponse> GetUser(Guid userId)
    {
        var user = await _userRepository.GetUserAsync(userId);
        if (user == null) throw new EntityNotFoundException("User not found");
        return user;
    }


    public async Task<List<UserGetResponse>> GetUsers(int pageNumber, int pageSize)
    {
        if (pageSize < 1 || pageNumber < 1) throw new IncorrectDataException("Invalid page number or page size.");
        var userList = await _userRepository.GetUsersAsync(pageNumber, pageSize);
        if (userList == null) throw new EntityNotFoundException("Users not found");
        return userList;
    }


    public async Task<bool> IsLoginUniqueAsync(string login)
    {
        var IsLoginUnique = await _userRepository.IsLoginUniqueAsync(login);
        return IsLoginUnique;
    }


    public async Task<bool> IsLoginUniqueForUserAsync(Guid userId, string login)
    {
        var IsLoginUnique = await _userRepository.IsLoginUniqueForUserAsync(userId, login);
        return IsLoginUnique;
    }

    public bool IsEmailValid(string email)
    {
        var regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var IsLoginUnique = await _userRepository.IsEmailUniqueAsync(email);
        return IsLoginUnique;
    }

    /*public async Task<List<UserGetResponse>> GetFilteredAndSortedUsers(FilterSortUserRequest request)
    {
        IQueryable<UserModel> query = _context.Users
            .Include(u => u.UserRoleModels)
            .ThenInclude(ur => ur.RoleModel);

        foreach (var param in request.Filters)
        {
            if (!string.IsNullOrWhiteSpace(param.Param) && param.Min >= 0 && param.Max >= param.Min)
            {
                switch (param.Param.ToLower())
                {
                    case "age":
                        query = query.Where(u => u.Age >= param.Min && u.Age <= param.Max);
                        break;
                    case "name":
                        query = query.Where(u => u.Login.Length >= param.Min && u.Login.Length <= param.Max);
                        break;
                    case "email":
                        query = query.Where(u => u.Email.Length >= param.Min && u.Email.Length <= param.Max);
                        break;
                }
            }
        }

        switch (request.SortField.ToLower())
        {
            case "age":
                query = request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(u => u.Age)
                    : query.OrderByDescending(u => u.Age);
                break;
            case "name":
                query = request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(u => u.Login)
                    : query.OrderByDescending(u => u.Login);
                break;
            case "email":
                query = request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(u => u.Email)
                    : query.OrderByDescending(u => u.Email);
                break;
        }

        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var users = await query.Skip(skipCount).Take(request.PageSize).ToListAsync();
        var userGetResponses = users.Select(user => new UserGetResponse
        {
            Id = user.Id,
            Name = user.Login,
            Email = user.Email,
            Age = user.Age,
            Roles = user.UserRoleModels
                .Select(ur => new RoleModel
                {
                    Id = ur.RoleModel.Id,
                    Role = ur.RoleModel.Role
                })
                .ToList()
        }).ToList();

        return userGetResponses;
    }#1#


    /*public async Task<List<RoleModel>> GetFilteredAndSortedRoles(FilterSortRolesRequest request)
    {
        IQueryable<RoleModel> query = _context.Roles;
        if (request.SelectedRoles != null && request.SelectedRoles.Any())
        {
            query = query.Where(role => request.SelectedRoles.Contains(role.Role));
        }

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            switch (request.SortField.ToLower())
            {
                case "role":
                    if (request.SortDirection == SortDirection.Ascending)
                    {
                        query = query.OrderBy(role => role.Role);
                    }
                    else
                    {
                        query = query.OrderByDescending(role => role.Role);
                    }
                    break;
            }
        }
        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var roles = await query.Skip(skipCount).Take(request.PageSize).ToListAsync();

        return roles;
    }#1#
}*/

