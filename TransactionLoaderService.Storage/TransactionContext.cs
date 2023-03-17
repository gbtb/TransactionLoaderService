using Microsoft.EntityFrameworkCore;
using TransactionLoaderService.Core;

namespace TransactionLoaderService.Storage;

public class TransactionContext: DbContext
{
    public TransactionContext(DbContextOptions<TransactionContext> options): base(options)
    {
        
    }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Transaction>();
        b.HasKey(t => t.Id);
        b.HasIndex(t => new { t.TransactionDate, t.CurrencyCode }).IsCreatedOnline();
        b.HasIndex(t => new { t.TransactionDate, t.Status }).IsCreatedOnline();
        b.Property(t => t.Id)
            .ValueGeneratedNever()
            .HasMaxLength(50)
            .IsFixedLength();
        b.Property(t => t.Amount).HasPrecision(18, 2);
    }
}