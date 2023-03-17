using TransactionLoaderService.Core;

namespace TransactionLoaderService.Web.Controllers;

public class TransactionsQuery
{
    public string? Currency { get; set; }
    
    public TransactionStatus? Status { get; set; }
    
    public string? DateBegin { get; set; }
    
    public string? DateEnd { get; set; }
}