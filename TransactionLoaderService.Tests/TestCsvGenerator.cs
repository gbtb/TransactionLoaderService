using System.Text;
using CsCheck;
using NMoneys;
using NUnit.Framework;
using TransactionLoaderService.Core;

namespace TransactionLoaderService.Tests;

[TestFixture]
public class TestCsvGenerator
{
    [Test]
    public void Generate()
    {
        var filename = "../../../RandomTransactions.csv";

        var chars = Gen.Char['a', 'z'];
        var id = Gen.String[chars, 50, 50];
        var date = Gen.DateTime[new DateTime(2022, 1, 1), DateTime.Now];
        var status = Gen.Enum<TransactionStatus>().Where(t => t != TransactionStatus.Unknown);
        var amount = Gen.Decimal[-2000, 2000000];
        var currency = Gen.Enum<CurrencyIsoCode>();

        var transactions = Gen.Select(id, date, status, amount, currency, BuildTransaction);
        transactions.Enumerable[100].SampleOne(e => 
            {
                var file = File.OpenWrite(filename);
                using var writer = new StreamWriter(file);
                foreach (var tran in e)
                {
                    var line =
                        $"\"{tran.Id}\",\"{tran.Amount:0.##}\",\"{tran.CurrencyCode}\",\"{tran.TransactionDate:dd/MM/yyyy HH:mm:ss}\",\"{MapStatus(tran.Status)}\"";
                    writer.WriteLine(line);
                }
            }
            );
    }

    private string MapStatus(TransactionStatus tranStatus)
    {
        return tranStatus switch
        {
            TransactionStatus.Approved => "Approved",
            TransactionStatus.Done => "Finished",
            TransactionStatus.Rejected => "Failed"
        };
    }

    private Transaction BuildTransaction(string arg1, DateTime arg2, TransactionStatus arg3, decimal arg4, CurrencyIsoCode arg5)
    {
        return new Transaction(arg1, arg4, arg5, arg2, arg3);
    }
}