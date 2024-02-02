namespace ProjectX.Repository.Interfaces;

public interface IAuthRepository
{
    public Task RecordLoginAsync(Guid userId);
}