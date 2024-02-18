﻿using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using TestApplication.Data;
using UP.ModelsEF;

namespace UP.Repositories;

public class TransactionsRepository(DataContext context, ICurrencyRepository currencyRepository)
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
        var user = context.Users
            .Include(u => u.Conversions).Include(user => user.Conversions)
            .FirstOrDefault(u => u.Id == userId);

        return user != null ? user.Conversions.ToList() : [];
    }

    public async void ReplenishTheBalance(Guid userId, double quantityUsd)
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
        var user = context.Users
            .Include(u => u.Replenishments)
            .FirstOrDefault(u => u.Id == userId);

        return user != null ? user.Replenishments.ToList() : [];
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
        var user = context.Users
            .Include(u => u.Replenishments).Include(user => user.Withdrawals)
            .FirstOrDefault(u => u.Id == userId);

        return user != null ? user.Withdrawals.ToList() : [];
    }

    public List<Transactions> GetUserTransactionsHistory(Guid userId)
    {
        var user = context.Users
            .Include(u => u.SentTransactions).Include(user => user.Withdrawals)
            .FirstOrDefault(u => u.Id == userId);

        return user != null ? user.SentTransactions.ToList() : [];
    }
}