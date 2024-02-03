namespace UP.DTO;

public class SendCryptoRequest
{
    public Guid ReceiverId { get; set; }
    public Guid SenderId { get; set; }
    public string CoinName { get; set; }
    public double QuantityForSend { get; set; }

    public SendCryptoRequest(Guid receiverId, Guid senderId, string coinName, double quantityForSend)
    {
        ReceiverId = receiverId;
        SenderId = senderId;
        CoinName = coinName;
        QuantityForSend = quantityForSend;
    }
}