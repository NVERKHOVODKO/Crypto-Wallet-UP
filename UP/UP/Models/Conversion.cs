namespace UP.Models
{
    public class Conversion
    {
        public Guid Id { get; set; }
        public double Commission { get; set; }
        public double BeginCoinQuantity { get; set; }
        public double EndCoinQuantity { get; set; }
        public double QuantityUsd { get; set; }
        public String BeginCoinShortname { get; set; }
        public String EndCoinShortname { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }

        public Conversion(Guid id, double commission, double beginCoinQuantity, double endCoinQuantity, double quantityUsd, string beginCoinShortname, string endCoinShortname, Guid userId, DateTime date)
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