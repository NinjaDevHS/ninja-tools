using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeywordsGen.Pages;

public class IndexModel : PageModel
{
   private readonly ILogger<IndexModel> _logger;

   public IndexModel(ILogger<IndexModel> logger)
   {
      _logger = logger;
   }

   public void OnGet()
   {

   }

   public class Keyword
   {
      public string text { get; set; } = "";
      public string lang { get; set; } = "";
      public string category { get; set; } = "";
      public string bg { get; set; } = "";
      public string bgcol { get; set; } = "";
      public string template { get; set; } = "";
      public string font { get; set; } = "";
      public string color { get; set; } = "";
      public string border { get; set; } = "";
   }
}