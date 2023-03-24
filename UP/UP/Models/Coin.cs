namespace UP.Models
{
    public class Coin
    {
        public int id { get; set; }
        public double quantity { get; set; }

        public string shortName { get; set; }

        public Coin(int id, double quantity, string shortName)
        {
            this.id = id;
            this.quantity = quantity;
            this.shortName = shortName;
        }

        public Coin()
        {
        }
    }
}
