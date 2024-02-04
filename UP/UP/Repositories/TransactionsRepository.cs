using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using TestApplication.Data;
using UP.Controllers;
using UP.ModelsEF;

namespace UP.Repositories;

public class TransactionsRepository : RepositoryBase, ITransactionsRepository
{
    private readonly DataContext _context;
    private readonly ICurrencyRepository _currencyRepository;

    public TransactionsRepository(DataContext context, ICurrencyRepository currencyRepository)
    {
        _context = context;
        _currencyRepository = currencyRepository;
    }

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

        _context.Conversions.Add(en);
        _context.SaveChanges();
    }

    public List<Conversion> GetUserConversionsHistory(Guid userId)
    {
        var user = _context.Users
            .Include(u => u.Conversions).Include(user => user.Conversions)
            .FirstOrDefault(u => u.Id == userId);

        if (user != null)
            return user.Conversions.ToList();
        return new List<Conversion>();
    }

    public async void ReplenishTheBalance(Guid userId, double quantityUsd)
    {
        var userCoins = _context.UsersCoins
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
            var user = _context.Users.Find(userId);

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

            _context.Coins.Add(newCoin);

            var userCoin = new UsersCoins
            {
                UserId = userId,
                CoinId = newCoin.Id,
                Coin = newCoin
            };

            _context.UsersCoins.Add(userCoin);
        }
        var replenishment = new Replenishment
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd * 0.98,
            Commission = quantityUsd * 0.02,
            UserId = userId
        };

        _context.Replenishments.Add(replenishment);

        _context.SaveChanges();
    }

    public List<Replenishment> GetUserDepositHistory(Guid userId)
    {
        var user = _context.Users
            .Include(u => u.Replenishments)
            .FirstOrDefault(u => u.Id == userId);

        if (user != null)
            return user.Replenishments.ToList();
        return new List<Replenishment>();
    }

    public void WithdrawUSDT(Guid userId, double quantityUsd)
    {
        var commission = 0.02;
        _currencyRepository.SellCrypto(userId, "usdt", quantityUsd + quantityUsd * commission);

        var withdrawal = new Withdrawal
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd,
            Commission = commission,
            UserId = userId
        };

        _context.Withdrawals.Add(withdrawal);
        var replenishment = new Withdrawal
        {
            Id = Guid.NewGuid(),
            Quantity = quantityUsd * 0.98,
            Commission = quantityUsd * 0.02,
            UserId = userId
        };

        _context.Withdrawals.Add(replenishment);
        _context.SaveChanges();
    }

    public List<Withdrawal> GetUserWithdrawalsHistory(Guid userId)
    {
        var user = _context.Users
            .Include(u => u.Replenishments).Include(user => user.Withdrawals)
            .FirstOrDefault(u => u.Id == userId);

        if (user != null)
            return user.Withdrawals.ToList();
        return new List<Withdrawal>();
    }

    public List<Transactions> GetUserTransactionsHistory(Guid userId)
    {
        var user = _context.Users
            .Include(u => u.SentTransactions).Include(user => user.Withdrawals)
            .FirstOrDefault(u => u.Id == userId);

        if (user != null)
            return user.SentTransactions.ToList();
        return new List<Transactions>();
    }
}