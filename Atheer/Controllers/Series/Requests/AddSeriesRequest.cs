using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Series.Requests
{
    public class AddSeriesRequest
    {
        [MinLength(1), MaxLength(64)] public string Title { get; set; }
        [MinLength(1), MaxLength(256)] public string Description { get; set; }
    }
}