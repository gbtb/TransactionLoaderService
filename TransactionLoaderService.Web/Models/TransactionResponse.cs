using TransactionLoaderService.Core;

namespace TransactionLoaderService.Web.Models;

public class TransactionResponse
{
    public string Id { get; }
    
    public string Payment { get; }
    
    public string Status { get; }

    public TransactionResponse(Transaction tran)
    {
        Id = tran.Id;
        Payment = $"{tran.Amount} {tran.CurrencyCode}";
        Status = tran.Status switch
        {
            TransactionStatus.Approved => "A",
            TransactionStatus.Rejected => "R",
            TransactionStatus.Done => "D",
            _ => throw new ArgumentException($"Transaction {tran.Id} with unknown status retrieved from storage. Possible data validation error / data corruption")
        };
    }
}