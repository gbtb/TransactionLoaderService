using TransactionLoaderService.Core;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Storage;

public class TransactionRepository: ITransactionRepository
{
    private readonly TransactionContext _context;

    public TransactionRepository(TransactionContext context)
    {
        _context = context;
    }
    
    public async Task SaveAsync(IEnumerable<Transaction> transactions, CancellationToken token)
    {
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync(token);
    }
}