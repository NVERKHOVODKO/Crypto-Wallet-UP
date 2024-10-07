namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IDbRepository dbRepository, IHashHelpers hashHelpers)
    : ControllerBase
{
    [HttpPut]
    [Route("edit-user-login")]
    public async Task<ActionResult> EditUserLogin([FromBody] EditUserLoginRequest request)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");
        if (request.Login == null)
            throw new EntityNotFoundException("Nickname can't be null");
        switch (request.Login.Length)
        {
            case < 4:
                throw new EntityNotFoundException("Nickname can't be less than 4 symbols");
            case > 20:
                throw new EntityNotFoundException("Nickname can't be above than 20 symbols");
        }

        if (!await IsNicknameUnique(request.Login, existingUser.Id))
            throw new IncorrectDataException("Nickname must be unique");
        existingUser.Login = request.Login;

        await dbRepository.SaveChangesAsync();
        return Ok("Логин изменен успешно");
    }

    [HttpPost]
    [Route("is-login-unique1")]
    private async Task<bool> IsNicknameUnique(string nickname, Guid currentUserId)
    {
        return await dbRepository.Get<User>(x => x.Login == nickname && x.Id != currentUserId).FirstOrDefaultAsync() ==
               null;
    }

    [HttpPut]
    [Route("edit-user-password")]
    public async Task<ActionResult> EditUser([FromBody] EditUserPasswordRequest request)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");
        if (request.Password == null)
            throw new EntityNotFoundException("Password can't be null");
        switch (request.Password.Length)
        {
            case < 4:
                throw new EntityNotFoundException("Password can't be less than 4 symbols");
            case > 20:
                throw new EntityNotFoundException("Password can't be above than 20 symbols");
        }

        existingUser.Password = hashHelpers.HashPassword(request.Password, existingUser.Salt);

        await dbRepository.SaveChangesAsync();
        return Ok("Пароль изменен успешно");
    }

    [HttpPut]
    [Route("edit-user-email")]
    public async Task<ActionResult> EditUserEmail([FromBody] EditUserEmailRequest request)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingUser is null)
            throw new EntityNotFoundException("User not found");
        if (request.Email is null)
            throw new IncorrectDataException("Email can't be null");
        switch (request.Email.Length)
        {
            case < 4:
                throw new IncorrectDataException("Email can't be less than 4 symbols");
            case > 20:
                throw new IncorrectDataException("Email can't be above than 20 symbols");
        }

        if (IsEmailValid(request.Email))
            throw new IncorrectDataException("Email can't be above than 20 symbols");

        if (!await IsEmailUniqueAsync(request.Email, existingUser.Id))
            throw new IncorrectDataException("Email must be unique");
        existingUser.Login = request.Email;

        await dbRepository.SaveChangesAsync();
        return Ok("Email изменен успешно");

    }

    [HttpPut]
    [Route("edit-user")]
    public Task<ActionResult> EditUser([FromBody] EditUserRequest request)
    {
        return Task.FromResult<ActionResult>(Ok("none"));
    }

    [HttpGet]
    [Route("get-previous-passwords/{id}")]
    public Task<ActionResult> GetUserPreviousPasswordsList(int id)
    {
        /*var loginHistory = await _dbRepository.Get<LoginHistory>().FirstOrDefaultAsync(x => x.UserId == id);
       if (loginHistory == null || loginHistory)
           throw new EntityNotFoundException("loginHistory not found");
       return Ok(loginHistory);*/
        return Task.FromResult<ActionResult>(Ok("none"));
    }

    [HttpGet]
    [Route("get-user-login-history/{id}")]
    public async Task<IActionResult> GetUserLoginHistory(Guid id)
    {
        var loginHistory = await dbRepository.Get<LoginHistory>().Where(x => x.UserId == id).ToListAsync();
        if (loginHistory == null)
            throw new EntityNotFoundException("loginHistory not found");
        return Ok(loginHistory);
    }

    [HttpGet]
    [Route("get-user-login/{id}")]
    public async Task<IActionResult> GetUserLoginById(Guid id)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");
        return Ok(existingUser.Login);
    }

    [HttpDelete]
    [Route("delete-account/{id}")]
    public async Task<ActionResult> DeleteAccount(Guid id)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsDeleted = true;
        existingUser.DateUpdated = DateTime.UtcNow;

        await dbRepository.SaveChangesAsync();
        return Ok("Пользователь удален");
    }

    [HttpPut]
    [Route("change-login")]
    public async Task<ActionResult> ChangeLogin(Guid id, string newLogin)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");
        switch (newLogin.Length)
        {
            case 0:
                throw new IncorrectDataException("Заполните данные");
            case > 32:
                throw new IncorrectDataException("Пароль должен быть короче 32 символов");
            case < 4:
                throw new IncorrectDataException("Пароль должен быть длиннее 4 символов");
        }

        existingUser.Login = newLogin;
        existingUser.DateUpdated = DateTime.UtcNow;

        await dbRepository.SaveChangesAsync();
        return Ok("Пароль успешно изменен");
    }

    [HttpPut]
    [Route("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var existingUser = await dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");
        if (request.Password != request.PasswordRepeat) return UnprocessableEntity("Пароли не совпадают");
        switch (request.Password.Length)
        {
            case 0:
                return UnprocessableEntity("Заполните данные");
            case > 32:
                return UnprocessableEntity("Пароль должен быть короче 32 символов");
            case < 4:
                return UnprocessableEntity("Пароль должен быть длиннее 4 символов");
        }

        existingUser.Password = hashHelpers.HashPassword(request.Password, existingUser.Salt);
        existingUser.DateUpdated = DateTime.UtcNow;

        await dbRepository.SaveChangesAsync();
        return Ok("Пароль успешно изменен");
    }

    [HttpPost]
    public async Task CreateUserAsync(CreateUserRequest request)
    {
        if (request == null)
            throw new EntityNotFoundException("User is null");
        if (request.Nickname == null)
            throw new EntityNotFoundException("Nickname can't be null");
        switch (request.Nickname.Length)
        {
            case < 4:
                throw new EntityNotFoundException("Nickname can't be less than 4 symbols");
            case > 20:
                throw new EntityNotFoundException("Nickname can't be above than 20 symbols");
        }

        if (request.Password == null)
            throw new EntityNotFoundException("Password can't be null");
        switch (request.Password.Length)
        {
            case < 4:
                throw new EntityNotFoundException("Password can't be less than 4 symbols");
            case > 40:
                throw new EntityNotFoundException("Password can't be above than 40 symbols");
        }

        var user = new User();
        if (!await IsNicknameUnique(request.Nickname)) throw new IncorrectDataException("Nickname must be unique");
        user.Login = request.Nickname;
        user.Salt = hashHelpers.GenerateSalt(30);
        user.Password = hashHelpers.HashPassword(request.Password, user.Salt);
        user.DateCreated = DateTime.UtcNow;
        user.Id = request.Id;
        await dbRepository.Add(user);
        await dbRepository.SaveChangesAsync();
    }

    [HttpPost]
    [Route("is-login-unique")]
    private async Task<bool> IsNicknameUnique(string nickname)
    {
        return await dbRepository.Get<User>(x => x.Login == nickname).FirstOrDefaultAsync() is null;
    }

    [HttpPost]
    [Route("is-email-unique/{email}/{id}")]
    public async Task<bool> IsEmailUniqueAsync(string email, Guid id)
    {
        var users = await dbRepository.Get<User>()
            .Where(x => x.Email == email && x.Id != id)
            .ToListAsync();
        
        return users.Count is 0;
    }

    [HttpPost]
    [Route("is-login-unique")]
    public async Task<bool> IsLoginUniqueAsync(string login)
    {
        var userWithSameEmail = await dbRepository.Get<User>()
            .Where(x => x.Login == login)
            .ToListAsync();
        return userWithSameEmail.Count == 0;
    }

    [HttpPost]
    [Route("is-email-valid/{email}")]
    public bool IsEmailValid(string email)
    {
        const string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }
}