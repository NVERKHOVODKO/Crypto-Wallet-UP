﻿namespace UP.Models
{
    public class Withdrawal
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Quantity { get; set; }
        public double Commission { get; set; }
        public Guid UserId { get; set; }

        public Withdrawal(Guid id, DateTime date, double quantity, double commission, Guid userId)
        {
            Id = id;
            Date = date;
            Quantity = quantity;
            Commission = commission;
            UserId = userId;
        }

        public Withdrawal()
        {
        }
    }
}