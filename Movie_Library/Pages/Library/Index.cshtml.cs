using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Library
{
    public class IndexModel : PageModel
    {
        internal LibraryClass Library { get; private set; }

        public IndexModel()
        {
            Library = new LibraryClass("Ma bibliothèque");
        }

        public void OnGet()
        {
        }
    }
}