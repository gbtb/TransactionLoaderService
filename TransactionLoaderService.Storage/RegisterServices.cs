using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransactionLoaderService.Core;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Storage;

public static class RegisterServices
{
    public static IServiceCollection RegisterStorageServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddDbContext<TransactionContext>(c =>
            {
                c.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                if (configuration.GetValue<bool>("EnableEFCoreLogging"))
                    c.LogTo(Console.WriteLine, LogLevel.Information);

            });
    }
}