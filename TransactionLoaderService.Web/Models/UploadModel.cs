using System.ComponentModel.DataAnnotations;

namespace TransactionLoaderService.Web.Models;

public class UploadModel
{
    [Required(ErrorMessage = "The {0} field is required")]
    [Display(Name = "Files")]
    [DataType(DataType.Upload)]
    public IFormFileCollection Files { get; set; }
}