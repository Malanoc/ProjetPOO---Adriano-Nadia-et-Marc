using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Library.Data;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Library
{
    public class IndexModel : PageModel
    {
        private readonly MongoDbService _mongoDbService;

        internal LibraryClass Library { get; private set; }

        public IndexModel(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;

            Library = new LibraryClass("Ma bibliothèque");
        }

        public async Task OnGetAsync()
        {
            List<Movie_Library.Classes.Movie> movies =
                await _mongoDbService.GetMoviesAsync();

            foreach (Movie_Library.Classes.Movie movie in movies)
            {
                Library.addMovies(movie);
            }
        }
    }
}