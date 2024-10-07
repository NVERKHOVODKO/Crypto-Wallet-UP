namespace Up.Infrastructure.Repositories.Interfaces;

public interface ITransactionsRepository
{
    void WriteNewConversionDataToDatabase(Conversion conversion);
    List<Conversion> GetUserConversionsHistory(Guid userId);
    void ReplenishTheBalance(Guid userId, double quantityUsd);
    List<Replenishment> GetUserDepositHistory(Guid userId);
    void WithdrawUSDT(Guid userId, double quantityUsd);
    List<Withdrawal> GetUserWithdrawalsHistory(Guid userId);
    List<Transactions> GetUserTransactionsHistory(Guid userId);
}