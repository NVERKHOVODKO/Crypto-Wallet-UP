using Analitique.BackEnd.Handlers;
using TestApplication.Data;
using UP.Models;
using UP.ModelsEF;

namespace UP.Data
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
                        Name = "СИГМАБАНК",
                        About = "СИГМАБАНК - это современный финансовый институт," +
                                " предоставляющий широкий спектр услуг как частным лицам, " +
                                "так и корпоративным клиентам. Наш банк стремится обеспечить надежное" +
                                " и удобное обслуживание, предлагая инновационные решения " +
                                "в сфере банковских услуг. Мы ценим доверие наших клиентов " +
                                "и работаем над тем, чтобы предложить им высококачественные " +
                                "продукты и сервисы, отвечающие их потребностям и ожиданиям. " +
                                "СИГМАБАНК - ваш надежный партнер в финансовых вопросах.",
                        PhotoName = "sigma.png"
                    },
                    new Service
                    {
                        Name = "Pubg",
                        About = "PUBG: BATTLEGROUNDS — это шутер жанра «Королевская битва», " +
                                "в котором 100 игроков сражаются друг с другом. Собирайте оружие, " +
                                "транспорт и предметы на постоянно уменьшающемся поле сражений, " +
                                "чтобы превзойти своих соперников и стать последним уцелевшим игроком.",
                        PhotoName = "pubg.png"
                    },
                    new Service
                    {
                        Name = "Skyline",
                        About = "Skyline Casino - ваша вечеринка в небесах! Насладитесь " +
                                "азартом и атмосферой роскоши в нашем современном казино. У нас вы " +
                                "найдете широкий выбор игр, от классических слотов до захватывающих" +
                                " настольных игр. Почувствуйте адреналин и веселье в Skyline Casino!",
                        PhotoName = "skyline.png"
                    },
                    new Service
                    {
                        Name = "Maxler",
                        About = "Maxler - ведущая компания в области спортивного питания, " +
                                "предоставляющая продукты для активного образа жизни и достижения " +
                                "спортивных целей. Наша миссия - вдохновлять людей на здоровый " +
                                "образ жизни, предлагая инновационные продукты, разработанные с " +
                                "использованием передовых технологий и научных исследований. ",
                        PhotoName = "maxler.png"
                    },
                    new Service
                    {
                        Name = "Burger king ",
                        About = "Burger King - легендарная сеть ресторанов быстрого питания," +
                                " предлагающая свежие бургеры, аппетитные закуски и освежающие " +
                                "напитки.  У нас каждый может насладиться вкусом и атмосферой " +
                                "королевского обеда.",
                        PhotoName = "burger.png"
                    },
                    new Service
                    {
                        Name = "Rockstar ",
                        About = "Rockstar - легендарная компания в мире видеоигр, создающая " +
                                "невероятно захватывающие игровые миры. Наши игры известны " +
                                "своими уникальными сюжетами, глубокими персонажами и" +
                                " потрясающей графикой. От городских приключений в серии " +
                                "Grand Theft Auto до эпических путешествий в Red Dead Redemption.",
                        PhotoName = "rockstar.png"
                    },
                    new Service
                    {
                        Name = "Apple ",
                        About = "Apple - мировой лидер в производстве инновационных " +
                                "гаджетов и технологий. Наши продукты объединяют стильный" +
                                " дизайн, передовые технологии и безупречную производительность." +
                                " От иконических iPhone до мощных MacBook и умных гаджетов" +
                                " Apple Watch - мы создаем устройства, которые меняют " +
                                "мир и улучшают жизнь ",
                        PhotoName = "apple.png"
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
