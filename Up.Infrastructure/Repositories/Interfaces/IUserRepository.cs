namespace Up.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    public List<Coin> GetUserCoins(Guid userId);
    public Task<List<CoinsInformation>> GetUserCoinsFull(Guid userId);
    public double GetCoinQuantityInUserWallet(Guid userId, string coinShortname);
}