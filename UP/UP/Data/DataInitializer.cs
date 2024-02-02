

namespace TestApplication.Data;

public class DataInitializer
{
    public static void Initialize(DataContext context)
    {
        context.Database.EnsureCreated();
    }
}