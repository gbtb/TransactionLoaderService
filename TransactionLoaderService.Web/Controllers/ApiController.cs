using System.Net;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NMoneys;
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
    public async Task<IActionResult> Index([FromQuery] TransactionsQuery query, CancellationToken token)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        CurrencyIsoCode? code = null;
        if (query.Currency is { Length: >0 } && !Currency.Code.TryParse(query.Currency, out code))
            return BadRequest($"Incorrect currency code: {query.Currency}. Code must be either empty or 3-char str in ISO4217 format");
        
        if (!TryParseDate(query.DateBegin, out var dateBegin))
            return BadRequest($"Incorrect begin date: {query.DateBegin}. Begin date must be parseable date str or empty");
        
        if (!TryParseDate(query.DateEnd, out var dateEnd))
            return BadRequest($"Incorrect end date: {query.DateEnd}. End date must be parseable date str or empty");

        if (query.Status == TransactionStatus.Unknown)
            return BadRequest("Incorrect transaction status. It should be an integer value in [1,2,3] range or empty");

        var transactions = await _transactionRepository.GetTransactionsAsync(dateBegin, dateEnd, code, query.Status, token);

        var response = transactions.Select(t => new TransactionResponse(t));
        
        return Json(response);
    }

    private bool TryParseDate(string? dateStr, out DateTime dateTime)
    {
        if (dateStr is {Length: > 0})
        {
            return DateTime.TryParse(dateStr, out dateTime);
        }

        dateTime = DateTime.MinValue;
        return true;
    }
}