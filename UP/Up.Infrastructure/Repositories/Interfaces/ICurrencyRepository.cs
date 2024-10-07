namespace Up.Infrastructure.Repositories.Interfaces;

public interface ICurrencyRepository
{
    void BuyCrypto(Guid userId, string shortname, double quantity);
    List<Coin> GetUserCoins(Guid userId);
    void AddCryptoToUserWallet(Guid userId, string shortname, double quantity);
    void SendCrypto(Guid receiverId, Guid senderId, string shortname, double quantity);
    bool IsCoinAlreadyPurchased(List<Coin> coins, string shortName);
    Task<double> GetCoinPrice(double quantity, string shortName);
    Task<double> GetCoinQuantity(double quantityUSD, string shortName);
    public void SellCrypto(Guid userId, string shortname, double quantityForSale);
    Task<double> GetUserBalance(Guid userId);
    public void SubtractCoinFromUser(Guid userId, string shortname, double quantityForSubtract);
    void DeleteCoin(Guid coinId);
    Task<CoinsInformation> GetFullCoinInformation(string shortName);
    void UpdateCoinQuantity(Guid id, double quantity);
    void WriteTransactionToDatabase(string coinName, double quantity, Guid senderId, Guid receiverId);
    void WriteWithdrawToDatabase(double quantity, double commission, Guid userId);
}