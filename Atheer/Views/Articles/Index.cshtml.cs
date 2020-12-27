using System.Collections.Generic;
using AtheerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atheer.Views.Articles
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)] 
        public List<BlogPost> Posts { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}