using NMoneys;

namespace TransactionLoaderService.Core;

public class Transaction
{
    public Transaction(string id, decimal amount, CurrencyIsoCode currencyCode, DateTime transactionDate, TransactionStatus status)
    {
        Id = id;
        Amount = amount;
        CurrencyCode = currencyCode;
        TransactionDate = transactionDate;
        Status = status;
    }

    public string Id { get; set; }
    
    public decimal Amount { get; set; }
    
    public CurrencyIsoCode CurrencyCode { get; set; }
    
    public DateTime TransactionDate { get; set; }
    
    public TransactionStatus Status { get; set; }
}