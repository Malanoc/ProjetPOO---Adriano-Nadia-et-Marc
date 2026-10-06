using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Library.Classes;
using Movie_Library.Classes.Tmdb;
using Movie_Library.Data;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly LibraryClass _library;

        private readonly MongoDbService _mongoDbService;

        public List<Movie> FoundMovies { get; private set; } = new();

        public List<Movie> AlreadyInLibrary { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public string? Message { get; private set; }


        public IndexModel(
            MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;

            _library =
                new LibraryClass("Ma bibliothèque");
        }


        public async Task OnGetAsync()
        {
            await LoadLibraryAsync();

            await SearchMoviesAsync();
        }


        /// <summary>
        /// Ajoute un film à la bibliothèque et à MongoDB.
        /// </summary>
        public async Task<IActionResult> OnPostAddAsync(
            int id,
            string title,
            string synopsis,
            string poster,
            float ratingTMDB,
            string? searchTerm)
        {
            SearchTerm = searchTerm;

            await LoadLibraryAsync();

            Movie movie =
                new Movie(
                    id,
                    title,
                    synopsis,
                    poster,
                    ratingTMDB,
                    Status.NotSeen
                );


            // Vérification en mémoire.
            bool alreadyInLibrary =
                _library.Movies.Any(
                    existingMovie =>
                        existingMovie.Id == movie.Id
                );


            if (alreadyInLibrary)
            {
                Message =
                    $"« {movie.Title} » est déjà dans votre bibliothèque.";
            }
            else
            {
                // Sauvegarde dans MongoDB.
                bool added =
                    await _mongoDbService.AddMovieAsync(movie);

                if (added)
                {
                    // Ajout à la bibliothèque en mémoire.
                    _library.addMovies(movie);

                    Message =
                        $"« {movie.Title} » a été ajouté à votre bibliothèque.";
                }
                else
                {
                    // Le film existait déjà dans MongoDB.
                    Message =
                        $"« {movie.Title} » est déjà dans votre bibliothèque.";

                    // Recharge la bibliothèque.
                    await LoadLibraryAsync();
                }
            }


            await SearchMoviesAsync();

            return Page();
        }


        /// <summary>
        /// Recharge les films depuis MongoDB.
        /// </summary>
        private async Task LoadLibraryAsync()
        {
            List<Movie> movies =
                await _mongoDbService.GetMoviesAsync();

            foreach (Movie movie in movies)
            {
                _library.addMovies(movie);
            }
        }


        /// <summary>
        /// Recherche les films sur TMDB
        /// et sépare ceux déjà enregistrés.
        /// </summary>
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

            FoundMovies =
                result.FoundMovies;

            AlreadyInLibrary =
                result.AlreadyInLibrary;
        }
    }
}