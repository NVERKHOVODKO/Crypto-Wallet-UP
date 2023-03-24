namespace UP.Models.Base;

public class CoinsInformation
{
    public int Id { get; set; }
    public String FullName { get; set; }
    public String ShortName { get; set; }
    public String IconPath { get; set; }
    public double DailyVolume { get; set; }
    public double DailyImpact { get; set; }

    public CoinsInformation(int id, string fullName, string shortName, string iconPath, double dailyVolume, double dailyImpact)
    {
        Id = id;
        FullName = fullName;
        ShortName = shortName;
        IconPath = iconPath;
        DailyVolume = dailyVolume;
        DailyImpact = dailyImpact;
    }

    public CoinsInformation()
    {
    }
}