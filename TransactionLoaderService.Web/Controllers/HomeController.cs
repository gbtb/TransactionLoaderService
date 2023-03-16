using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TransactionLoaderService.Core;
using TransactionLoaderService.Core.TransactionFileLoader;
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

        var fileFormat = GuessFileFormat(file);
        if (fileFormat == TransactionFileFormat.Unknown)
            return BadRequest(
                $"File content type: {file.ContentType} is not valid. Valid content types are: {MediaTypeNames.Application.Xml}, {MediaTypeNames.Text.Xml}, {MediaTypeNames.Text.Plain}, text/csv");

        var result = await fileLoader.LoadFileAsync(file.OpenReadStream(), fileFormat, token);
        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.Errors);
    }

    private static TransactionFileFormat GuessFileFormat(IFormFile file)
    {
        return file.ContentType switch
        {
            MediaTypeNames.Application.Xml => TransactionFileFormat.Xml,
            MediaTypeNames.Text.Xml => TransactionFileFormat.Xml,
            MediaTypeNames.Text.Plain => TransactionFileFormat.Csv,
            "text/csv" => TransactionFileFormat.Csv,
            _ => TransactionFileFormat.Unknown
        };
    }
}