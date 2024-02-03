using System;
using System.Collections.Generic;
using UP.Models;

namespace UP.Repositories
{
    public interface ITransactionsRepository
    {
        void WriteNewConversionDataToDatabase(ModelsEF.Conversion conversion);
        List<ModelsEF.Conversion> GetUserConversionsHistory(Guid userId);
        void ReplenishTheBalance(Guid userId, double quantityUsd);
        List<ModelsEF.Replenishment> GetUserDepositHistory(Guid userId);
        void WithdrawUSDT(Guid userId, double quantityUsd);
        List<ModelsEF.Withdrawal> GetUserWithdrawalsHistory(Guid userId);
        List<ModelsEF.Transactions> GetUserTransactionsHistory(Guid userId);
    }
}