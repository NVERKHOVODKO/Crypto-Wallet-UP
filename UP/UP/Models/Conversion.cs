namespace UP.Models
{
    public class Conversion
    {
        public int Id { get; set; }
        public double Commission { get; set; }
        public double BeginCoinQuantity { get; set; }
        public double EndCoinQuantity { get; set; }
        public double QuantityUsd { get; set; }
        public String BeginCoinShortname { get; set; }
        public String EndCoinShortname { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public Conversion(int id, double commission, double beginCoinQuantity, double endCoinQuantity, double quantityUsd, string beginCoinShortname, string endCoinShortname, int userId, DateTime date)
        { 
            Id = id;
            Commission = commission;
            BeginCoinQuantity = beginCoinQuantity;
            EndCoinQuantity = endCoinQuantity;
            QuantityUsd = quantityUsd;
            BeginCoinShortname = beginCoinShortname;
            EndCoinShortname = endCoinShortname;
            UserId = userId;
            Date = date;
        }

        public Conversion()
        {
        }
    }
}