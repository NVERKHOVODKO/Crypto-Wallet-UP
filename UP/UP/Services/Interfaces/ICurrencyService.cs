namespace UP.Services.Interfaces;

public interface ICurrencyService
{
    public void AddIfNotExists(string coinShortName, double price);
}