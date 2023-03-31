namespace UP.Models.Base;

public class CoinsInformation
{
    public int Id { get; set; }
    public String FullName { get; set; }
    public String ShortName { get; set; }
    public String IconPath { get; set; }
    public double DailyVolume { get; set; }
    public double DailyImpact { get; set; }
    public double Price { get; set; }


    public CoinsInformation(int id, string fullName, string shortName, string iconPath, double dailyVolume, double dailyImpact, double price)
    {
        Id = id;
        FullName = fullName;
        ShortName = shortName;
        IconPath = iconPath;
        DailyVolume = dailyVolume;
        DailyImpact = dailyImpact;
        Price = price;
    }

    public CoinsInformation()
    {
    }
}