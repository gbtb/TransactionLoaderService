using System.Net;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TransactionLoaderService.Core;
using TransactionLoaderService.Core.TransactionFileLoader;

namespace TransactionLoaderService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController: Controller
{
    private readonly ITransactionRepository _transactionRepository;

    public ApiController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [HttpGet("/transactions")]
    public IActionResult Index([FromQuery] TransactionsQuery query, CancellationToken token)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        //var currencyCode = Currency.Code.TryParse(query.Currency);
        
        //await _transactionRepository.GetTransactionsAsync()

        return Json("");
    }
}

public class TransactionResponse
{
    public string Id { get; }
    
    public string Payment { get; }
    
    public string Status { get; }

    public TransactionResponse(Transaction tran)
    {
        Id = tran.Id;
        Payment = $"{tran.Amount} {tran.CurrencyCode}";
        Status = tran.Status switch
        {
            TransactionStatus.Approved => "A",
            TransactionStatus.Rejected => "R",
            TransactionStatus.Done => "D",
            _ => throw new ArgumentException($"Transaction {tran.Id} with unknown status retrieved from storage. Possible data validation error / data corruption")
        };
    }
}

public class TransactionsQuery
{
    public string? Currency { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionStatus Status { get; set; }
    
    public string? DateBegin { get; set; }
    
    public string? DateEnd { get; set; }
}