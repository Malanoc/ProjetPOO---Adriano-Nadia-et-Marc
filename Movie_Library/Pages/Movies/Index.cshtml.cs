using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Library.Classes;
using Movie_Library.Classes.Tmdb;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly LibraryClass _library;

        public List<Movie> FoundMovies { get; private set; } = new();

        public List<Movie> AlreadyInLibrary { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public string? Message { get; private set; }

        public IndexModel()
        {
            _library = new LibraryClass("Ma bibliothèque");
        }

        public async Task OnGetAsync()
        {
            await SearchMoviesAsync();
        }

        public async Task<IActionResult> OnPostAddAsync(
            int id,
            string title,
            string synopsis,
            string poster,
            float ratingTMDB,
            string? searchTerm)
        {
            SearchTerm = searchTerm;

            Movie movie = new Movie(
                id,
                title,
                synopsis,
                poster,
                ratingTMDB,
                Status.NotSeen
            );

            int movieCountBefore = _library.Movies.Count;

            _library.addMovies(movie);

            if (_library.Movies.Count > movieCountBefore)
            {
                Message = $"« {movie.Title} » a été ajouté à votre bibliothèque.";
            }
            else
            {
                Message = $"« {movie.Title} » est déjà dans votre bibliothèque.";
            }

            await SearchMoviesAsync();

            return Page();
        }

        private async Task SearchMoviesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FoundMovies = new List<Movie>();
                AlreadyInLibrary = new List<Movie>();
                return;
            }

            MovieSearchResult result =
                await _library.searchMovie(SearchTerm);

            FoundMovies = result.FoundMovies;
            AlreadyInLibrary = result.AlreadyInLibrary;
        }
    }
}