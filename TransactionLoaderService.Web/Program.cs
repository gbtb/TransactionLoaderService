using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using TransactionLoaderService.Core.TransactionFileLoader;
using TransactionLoaderService.Core.TransactionStreamReaders;
using TransactionLoaderService.Storage;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace TransactionLoaderService.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services
            .RegisterStorageServices(builder.Configuration)
            .AddLogging(c => c.AddSimpleConsole())
            .AddSwaggerGen(c => c.AddEnumsWithValuesFixFilters())
            .AddTransient<ITransactionFileLoader, TransactionFileLoader>()
            .AddTransient<ITransactionStreamReader, XmlTransactionStreamReader>()
            .AddTransient<ITransactionStreamReader, CsvTransactionStreamReader>();

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024; //1 MB
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        
        
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TransactionContext>();
            //db.Database.EnsureDeleted();
            db.Database.Migrate();
        }

        app.Run();
    }
}