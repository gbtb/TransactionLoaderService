using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TransactionLoaderService.Core;
using TransactionLoaderService.Web.Models;

namespace TransactionLoaderService.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile? file, [FromServices] ITransactionFileLoader fileLoader, CancellationToken token)
    {
        if (file == null)
            return BadRequest("Files were not submitted");

        if (file.ContentType != MediaTypeNames.Application.Xml && file.ContentType != MediaTypeNames.Text.Plain)
            return BadRequest(
                $"File content type: {file.ContentType} is not valid. Valid content types are: {MediaTypeNames.Application.Xml}, {MediaTypeNames.Text.Plain}");

        var result = await fileLoader.LoadFileAsync(file.OpenReadStream(), GuessFileFormat(file), token);
        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.Errors);
    }

    private static TransactionFileFormat GuessFileFormat(IFormFile file)
    {
        return file.ContentType switch
        {
            MediaTypeNames.Application.Xml => TransactionFileFormat.Xml,
            MediaTypeNames.Text.Plain => TransactionFileFormat.Csv,
            _ => TransactionFileFormat.Unknown
        };
    }
}