using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Замените YOUR_API_KEY на свой ключ API 4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614
        string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
        
        string[] cryptoCoins = new string[]
        {
            "btc",
            "eth",
            "bnb",
            "xlm",
            "shib",
            "doge",
            "rose",
            "spell",
            "ltc",
            "dot"
        };

        for (int i = 0; i < 10; i++)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

            string coinName = cryptoCoins[i];

            string url = $"https://min-api.cryptocompare.com/data/price?fsym=" + coinName + "&tsyms=USD";

            //string url = $"https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + coinName + "&tsyms=USD";

            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine(coinName + ": " + responseContent); // Вывод полученных данных
        }
    }
}