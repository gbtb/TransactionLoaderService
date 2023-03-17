using Microsoft.Extensions.DependencyInjection;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Storage;

public static class RegisterServices
{
    public static IServiceCollection RegisterStorageServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddDbContext<TransactionContext>();
    }
}