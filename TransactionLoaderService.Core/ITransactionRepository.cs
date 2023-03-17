using NMoneys;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Core;

public interface ITransactionRepository
{
    Task<Result> SaveAsync(IEnumerable<Transaction> transactions, CancellationToken token);
    Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(DateTime dateBegin, DateTime dateEnd, CurrencyIsoCode? code, TransactionStatus? queryStatus, CancellationToken token);
}