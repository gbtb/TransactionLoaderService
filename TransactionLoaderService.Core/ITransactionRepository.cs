using NMoneys;

namespace TransactionLoaderService.Core.TransactionFileLoader;

public interface ITransactionRepository
{
    Task<Result> SaveAsync(IEnumerable<Transaction> transactions, CancellationToken token);
    Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(DateTime dateBegin, DateTime dateEnd, CurrencyIsoCode? code, TransactionStatus? queryStatus, CancellationToken token);
}