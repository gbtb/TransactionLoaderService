using Microsoft.EntityFrameworkCore;
using TransactionLoaderService.Core;

namespace TransactionLoaderService.Storage;

public class TransactionContext: DbContext
{
    public TransactionContext()
    {
        
    }
    
    // public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
    // {
    //     
    // }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Transaction>();
        b.HasKey(t => t.Id);
        b.Property(t => t.Id)
            .ValueGeneratedNever()
            .HasMaxLength(50)
            .IsFixedLength();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=localhost;Initial Catalog=Main;Encrypt=false;Integrated Security=false;User Id=SA;Password=Passw0rd");
    }
}