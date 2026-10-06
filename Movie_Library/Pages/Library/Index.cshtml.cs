using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Library
{
    public class IndexModel : PageModel
    {
        internal LibraryClass Library { get; private set; }

        public IndexModel(IServiceProvider services)
        {
            Library =
                services.GetRequiredService<LibraryClass>();
        }

        public void OnGet()
        {
        }
    }
}