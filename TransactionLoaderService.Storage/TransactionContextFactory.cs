using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TransactionLoaderService.Storage;

public class TransactionContextFactory : IDesignTimeDbContextFactory<TransactionContext>
{
    public TransactionContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationManager();
        config.AddJsonFile("appsettings.json");
        
        var optionsBuilder = new DbContextOptionsBuilder<TransactionContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        return new TransactionContext(optionsBuilder.Options);
    }
}