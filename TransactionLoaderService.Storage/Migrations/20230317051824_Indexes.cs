using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionLoaderService.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate_CurrencyCode",
                table: "Transactions",
                columns: new[] { "TransactionDate", "CurrencyCode" })
                .Annotation("SqlServer:Online", true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate_Status",
                table: "Transactions",
                columns: new[] { "TransactionDate", "Status" })
                .Annotation("SqlServer:Online", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionDate_CurrencyCode",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionDate_Status",
                table: "Transactions");
        }
    }
}
