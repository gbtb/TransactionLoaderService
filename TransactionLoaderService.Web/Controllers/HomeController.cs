using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TransactionLoaderService.Core.TransactionFileLoader;
using TransactionLoaderService.Web.Models;

namespace TransactionLoaderService.Web.Controllers;

public class HomeController : Controller
{
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (file == null)
            return BadRequest("Files were not submitted");

        var fileFormat = GuessFileFormat(file);
        if (fileFormat == TransactionFileFormat.Unknown)
            return BadRequest(
                $"Unknown format. File content type: {file.ContentType} is not valid. Valid content types are: {MediaTypeNames.Application.Xml}, {MediaTypeNames.Text.Xml}, {MediaTypeNames.Text.Plain}, text/csv");

        var stream = file.OpenReadStream(); //dispose of this stream is not necessary, since it wraps request stream which will be disposed by Asp.Net Core itself
        var result = await fileLoader.LoadFileAsync(stream, fileFormat, token);
        if (result.IsSuccess)
        {
            ViewData["Message"] = $"Successfully loaded transactions from {file.Name}";
            return View();
        }

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