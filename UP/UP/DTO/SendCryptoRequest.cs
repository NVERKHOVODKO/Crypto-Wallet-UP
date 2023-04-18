namespace UP.DTO;

public class SendCryptoRequest
{
    public int ReceiverId { get; set; }
    public int SenderId { get; set; }
    public string CoinName { get; set; }
    public double QuantityForSend { get; set; }

    public SendCryptoRequest(int receiverId, int senderId, string coinName, double quantityForSend)
    {
        ReceiverId = receiverId;
        SenderId = senderId;
        CoinName = coinName;
        QuantityForSend = quantityForSend;
    }
}