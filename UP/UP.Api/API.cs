namespace UP;

public class API
{
    private const string key = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";

    public async Task<double> getPrice(string ShortName)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", key);

        var url = "https://min-api.cryptocompare.com/data/price?fsym=" + ShortName + "&tsyms=USD";

        var response = await httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        return double.TryParse(responseContent, out var result) ? result : 0.0;
    }
}