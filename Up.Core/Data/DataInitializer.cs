namespace Up.Core.Data
{
    public class DataInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Services.Any())
            {
                var services = new[]
                {
                    new Service
                    {
                        Name = "СИГМАБАНК",
                        About = "Финансовый институт, услуги частным и корпоративным.",
                        PhotoName = "sigma.png"
                    },
                    new Service
                    {
                        Name = "Pubg",
                        About = "Шутер жанра «Королевская битва», 100 игроков.",
                        PhotoName = "pubg.png"
                    },
                    new Service
                    {
                        Name = "Skyline",
                        About = "Казино в небесах, азарт и роскошь.",
                        PhotoName = "skyline.png"
                    },
                    new Service
                    {
                        Name = "Maxler",
                        About = "Спортивное питание, активный образ жизни.",
                        PhotoName = "maxler.png"
                    },
                    new Service
                    {
                        Name = "Burger king ",
                        About = "Ресторан быстрого питания, свежие бургеры.",
                        PhotoName = "burger.png"
                    }
                };
                context.Services.AddRange(services);
                context.SaveChanges();
            }
            if (!context.Users.Any())
            {
                var users = new[]
                {
                    new User
                    {
                        DateCreated = DateTime.UtcNow,
                        Login = "Service",
                        Password = "Service",
                        Email = "Service",
                        IsDeleted = false,
                        RoleId = 0,
                        IsBlocked = false,
                        Salt = "Service"
                    },
                    new User
                    {
                        DateCreated = DateTime.UtcNow,
                        Login = "user",
                        Password = HashHandler.HashPassword("user", "user"),
                        Email = "user",
                        IsDeleted = false,
                        RoleId = 1,
                        IsBlocked = false,
                        Salt = "user"
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (context.CoinListInfos.Any()) return;
            var coins = new Dictionary<string, string>
            {
                {"btc", "Bitcoin"},
                {"eth", "Ethereum"},
                {"usdt", "Tether"},
                {"bnb", "Binance Coin"},
                {"sol", "Solana"},
                {"ada", "Cardano"},
                {"xrp", "XRP"},
                {"dot", "Polkadot"},
                {"doge", "Dogecoin"},
                {"uni", "Uniswap"},
                {"luna", "Terra"},
                {"link", "Chainlink"},
                {"avax", "Avalanche"},
                {"matic", "Polygon"},
                {"shib", "Shiba Inu"},
                {"atom", "Cosmos"},
                {"fil", "Filecoin"},
                {"xtz", "Tezos"},
                {"ltc", "Litecoin"},
                {"ftt", "FTX Token"},
                {"algo", "Algorand"},
                {"vet", "VeChain"},
                {"eos", "EOS"},
                {"trb", "Tellor"},
                {"ksm", "Kusama"},
                {"cake", "PancakeSwap"},
                {"tfuel", "Theta Fuel"},
                {"sushi", "SushiSwap"},
                {"dcr", "Decred"},
                {"fet", "Fetch.ai"}
            };

            foreach (var coin in coins)
            {
                context.CoinListInfos.Add(new CoinListInfo
                {
                    Id = Guid.NewGuid(),
                    ShortName = coin.Key,
                    FullName = coin.Value,
                    IsActive = true
                });
            }

            context.SaveChanges();
        }
    }
}
