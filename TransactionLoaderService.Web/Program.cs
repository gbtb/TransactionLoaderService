using Microsoft.EntityFrameworkCore;
using TransactionLoaderService.Core.TransactionFileLoader;
using TransactionLoaderService.Core.TransactionStreamReaders;
using TransactionLoaderService.Storage;

namespace TransactionLoaderService.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services
            .RegisterStorageServices()
            .AddLogging(c => c.AddSimpleConsole())
            .AddTransient<ITransactionFileLoader, TransactionFileLoader>()
            .AddTransient<ITransactionStreamReader, XmlTransactionStreamReader>()
            .AddTransient<ITransactionStreamReader, CsvTransactionStreamReader>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TransactionContext>();
            db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        app.Run();
    }
}