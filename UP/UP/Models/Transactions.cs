namespace UP.Models
{
    public class Transactions
    {
        public Guid Id { get; set; }
        public string CoinName { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public Transactions(Guid id, string coinName, double quantity, DateTime date, Guid senderId, Guid receiverId)
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
