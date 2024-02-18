using Analitique.BackEnd.Handlers;
using UP.Models;
using UP.ModelsEF;

namespace TestApplication.Data
{
    public class DataInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Services.Any())
            {
                var services = new[]
                {
                    new Service
                    {
                        Name = "Покупка игровых предметов",
                        About = "Поддержите свою игровую страсть, приобретая различные предметы, бонусы и улучшения для вашей игры. Насладитесь преимуществами, улучшайте свой игровой опыт и достигайте новых вершин с нашей услугой покупки игровых предметов.",
                        PhotoName = "sigma.png",
                    },
                    new Service
                    {
                        Name = "Пополнение игрового счета",
                        About = "Увеличьте баланс своего игрового счета для непрерывного и комфортного игрового процесса. Наша услуга пополнения игрового счета позволит вам быстро и безопасно пополнить баланс и наслаждаться игровым миром без прерываний.",
                        PhotoName = "pubg.png",
                    },
                    new Service
                    {
                        Name = "Подписка на премиум-аккаунт",
                        About = "Получите доступ к эксклюзивным функциям, контенту и привилегиям с нашей услугой подписки на премиум-аккаунт. Усилите свой опыт игры, получите дополнительные возможности и наслаждайтесь игрой на новом уровне.",
                        PhotoName = "skyline.png",
                    },
                    new Service
                    {
                        Name = "Оплата онлайн-курсов",
                        About = "Инвестируйте в своё образование, оплачивая онлайн-курсы с нашей платежной системой. Получите доступ к качественному обучению, экспертным знаниям и профессиональному развитию в удобном онлайн формате.",
                        PhotoName = "maxler.png",
                    },
                    new Service
                    {
                        Name = "Покупка цифровых товаров",
                        About = "Расширьте вашу цифровую коллекцию, приобретая различные цифровые товары, от музыки и фильмов до электронных книг и программного обеспечения. Насладитесь удобством онлайн-шопинга и моментальной доставкой с нашей услугой покупки цифровых товаров.",
                        PhotoName = "burger.png",
                    },
                    new Service
                    {
                        Name = "Оплата за онлайн-консультации",
                        About = "Получайте профессиональные консультации и поддержку, оплачивая за онлайн-консультации с нашей платежной системой. Получите экспертные советы, решения и помощь в решении ваших вопросов и проблем из уюта вашего дома.",
                        PhotoName = "rockstar.png",
                    },
                    new Service
                    {
                        Name = "Поддержка музыкантов (донат)",
                        About = "Поддержите вашего любимого музыканта или творческого проекта, сделав донат с помощью нашей платежной системы. Помогите продвинуть талантливых артистов и способствуйте созданию новой качественной музыки и контента.",
                        PhotoName = "apple.png",
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
                        Salt = "Service",
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
                        Salt = "user",
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
            if (!context.CoinListInfos.Any())
            {
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
}
