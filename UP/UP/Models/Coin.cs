namespace UP.Models
{
    public class Coin
    {
        public int Id { get; set; }
        public double Quantity { get; set; }

        public string ShortName { get; set; }

        public Coin(int id, double quantity, string shortName)
        {
            Id = id;
            Quantity = quantity;
            ShortName = shortName;
        }

        public Coin()
        {
        }
    }
}
