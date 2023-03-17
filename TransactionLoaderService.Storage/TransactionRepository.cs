using Microsoft.EntityFrameworkCore;
using NMoneys;
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

    public async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(DateTime dateBegin, DateTime dateEnd, CurrencyIsoCode? code, TransactionStatus? queryStatus,
        CancellationToken token)
    {
        return await _context.Transactions
            .Where(t => t.TransactionDate >= dateBegin
                        && (dateEnd == DateTime.MinValue || t.TransactionDate < dateEnd)
                        && (code == null || t.CurrencyCode == code)
                        && (queryStatus == null || t.Status == queryStatus)
                        )
            .OrderByDescending(t => t.TransactionDate)
            .Take(50)
            .ToListAsync(cancellationToken: token);
    }
}