namespace UP.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public string CoinName { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public Transactions(int id, string coinName, double quantity, DateTime date, int senderId, int receiverId)
        {
            Id = id;
            CoinName = coinName;
            Quantity = quantity;
            Date = date;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}
