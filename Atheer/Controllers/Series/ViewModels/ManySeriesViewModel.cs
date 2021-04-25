using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.Series.ViewModels
{
    public class ManySeriesViewModel
    {
        public ICollection<ArticleSeries> Series { get; set; }
    }
}