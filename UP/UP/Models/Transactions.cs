namespace UP.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public String CoinName { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public int SenderId { get; set; }
        public int RecieverId { get; set; }
    }
}
