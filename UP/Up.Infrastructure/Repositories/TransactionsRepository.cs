namespace Up.Infrastructure.Repositories;

public class TransactionsRepository(AppDbContext context, ICurrencyRepository currencyRepository)
    : RepositoryBase, ITransactionsRepository
{
    public void WriteNewConversionDataToDatabase(Conversion conversion)
    {
        var en = new Conversion
        {
            Id = Guid.NewGuid(),
            Commission = conversion.Commission,
            BeginCoinQuantity = conversion.BeginCoinQuantity,
            EndCoinQuantity = conversion.EndCoinQuantity,
            QuantityUSD = conversion.QuantityUSD,
            BeginCoinShortname = conversion.BeginCoinShortname,
            EndCoinShortname = conversion.EndCoinShortname,
            UserId = conversion.UserId,
            DateCreated = DateTime.UtcNow
        };

        context.Conversions.Add(en);
        context.SaveChanges();
    }

    public List<Conversion> GetUserConversionsHistory(Guid userId)
    {
        var conversions = context.Conversions
            .Where(c => c.UserId == userId).ToList();

        return conversions;
    }

    public void ReplenishTheBalance(Guid userId, double quantityUsd)
    {
        var userCoins = context.UsersCoins
            .Include(uc => uc.Coin)
            .Where(uc => uc.UserId == userId)
            .ToList();

        var existingCoin = userCoins.FirstOrDefault(uc => uc.Coin.Shortname == "usdt");

        if (existingCoin != null)
        {
            existingCoin.Coin.Quantity += quantityUsd;
        }
        else
        {
            var user = context.Users.Find(userId);

            if (user == null)
            {
                throw new EntityNotFoundException("Пользователь не найден");
            }

            var newCoin = new Coin
            {
                Id = Guid.NewGuid(),
                Quantity = quantityUsd,
                Shortname = "usdt"
            };

            context.Coins.Add(newCoin);

            var userCoin = new UsersCoins
            {
                UserId = userId,
                CoinId = newCoin.Id,
                Coin = newCoin
            };

            context.UsersCoins.Add(userCoin);
        }
        var replenishment = new Replenishment
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd * 0.98,
            Commission = quantityUsd * 0.02,
            UserId = userId
        };

        context.Replenishments.Add(replenishment);

        context.SaveChanges();
    }

    public List<Replenishment> GetUserDepositHistory(Guid userId)
    {
        var replenishments = context.Replenishments
            .Where(c => c.UserId == userId)
            .ToList();

        return replenishments;
    }

    public void WithdrawUSDT(Guid userId, double quantityUsd)
    {
        const double commission = 0.02;
        currencyRepository.SellCrypto(userId, "usdt", quantityUsd + quantityUsd * commission);

        var withdrawal = new Withdrawal
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd,
            Commission = commission,
            UserId = userId
        };

        context.Withdrawals.Add(withdrawal);
        var replenishment = new Withdrawal
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd * 0.98,
            Commission = quantityUsd * 0.02,
            UserId = userId
        };

        context.Withdrawals.Add(replenishment);
        context.SaveChanges();
    }

    public List<Withdrawal> GetUserWithdrawalsHistory(Guid userId)
    {
        var withdrawals = context.Withdrawals
            .Where(c => c.UserId == userId)
            .ToList();

        return withdrawals;
    }

    public List<Transactions> GetUserTransactionsHistory(Guid userId)
    {
        var transactions = context.Transactions
            .Where(c => c.ReceiverId == userId || c.SenderId == userId)
            .ToList();

        return transactions;
    }
}