using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NMoneys;
using TransactionLoaderService.Core;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Storage;

public class TransactionRepository: ITransactionRepository
{
    private readonly TransactionContext _context;
    private readonly ILogger _logger;

    public TransactionRepository(TransactionContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result> SaveAsync(IEnumerable<Transaction> transactions, CancellationToken token)
    {
        try
        {
            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync(token);
            return Result.Success;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException {Number: 2627})
        {
            _logger.LogError(ex,
                "Transactions save has been aborted. Attempted to insert duplicate transaction. {Message}",
                ex.InnerException.Message);
            return new Result($"Transactions save has been aborted. Attempted to insert duplicate transaction. {ex.InnerException.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected database error");
            return new Result("Unexpected database error. Please try again in a few minutes");
        }
    }

    public async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(DateTime dateBegin, DateTime dateEnd, CurrencyIsoCode? code, TransactionStatus? queryStatus,
        CancellationToken token)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected database error");
            throw;
        }
    }
}