using NMoneys;

namespace TransactionLoaderService.Core;

/// <summary>
/// Domain object representing valid transaction
/// </summary>
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

    /// <summary>
    /// Transaction Id
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public CurrencyIsoCode CurrencyCode { get; set; }
    
    /// <summary>
    /// Transaction date
    /// </summary>
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Transaction status
    /// </summary>
    public TransactionStatus Status { get; set; }
}