namespace Up.Core.Models;

public class CoinsInformation
{
    public CoinsInformation(Guid id, string fullName, string shortName, string iconPath, double dailyVolume,
        double dailyImpact, double price, double percentagePriceChangePerDay, double quantity)
    {
        Id = id;
        FullName = fullName;
        ShortName = shortName;
        IconPath = iconPath;
        DailyVolume = dailyVolume;
        DailyImpact = dailyImpact;
        Price = price;
        PercentagePriceChangePerDay = percentagePriceChangePerDay;
        Quantity = quantity;
    }

    public CoinsInformation(string shortName, string fullName, string iconPath, double dailyVolume, double dailyImpact,
        double price, double percentagePriceChangePerDay)
    {
        FullName = fullName;
        ShortName = shortName;
        IconPath = iconPath;
        DailyVolume = dailyVolume;
        DailyImpact = dailyImpact;
        Price = price;
        PercentagePriceChangePerDay = percentagePriceChangePerDay;
    }

    public CoinsInformation()
    {
    }

    public CoinsInformation(Guid id, string fullName, string shortName, double dailyVolume, double dailyImpact,
        double price, double percentagePriceChangePerDay, double quantity)
    {
        Id = id;
        FullName = fullName;
        ShortName = shortName;
        DailyVolume = dailyVolume;
        DailyImpact = dailyImpact;
        Price = price;
        PercentagePriceChangePerDay = percentagePriceChangePerDay;
        Quantity = quantity;
    }

    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string ShortName { get; set; }
    public string IconPath { get; set; }
    public double DailyVolume { get; set; }
    public double DailyImpact { get; set; }
    public double Price { get; set; }
    public double PercentagePriceChangePerDay { get; set; }

    public double Quantity { get; set; }
}