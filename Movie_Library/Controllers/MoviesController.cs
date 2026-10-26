using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Movie_Library.Classes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Movie_Library.Controllers
{
    /// <summary>
    /// API REST permettant de tester et manipuler les films de la bibliothèque.
    /// La même instance de Library est utilisée par cette API et par le frontend Razor.
    /// </summary>
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IServiceProvider _services;

        public MoviesController(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Récupère la bibliothèque partagée enregistrée dans Program.cs.
        /// </summary>
        private Library GetLibrary()
        {
            return _services.GetRequiredService<Library>();
        }

        /// <summary>
        /// Retourne tous les films actuellement présents dans la bibliothèque.
        /// GET /api/movies
        /// </summary>
        [HttpGet]
        public IActionResult GetMovies()
        {
            Library library = GetLibrary();

            return Ok(
                library.Movies.Select(movie => ToResponse(movie))
            );
        }

        /// <summary>
        /// Retourne un film de la bibliothèque grâce à son identifiant TMDB.
        /// GET /api/movies/27205
        /// </summary>
        [HttpGet("{id:int}")]
        public IActionResult GetMovie(int id)
        {
            Library library = GetLibrary();

            Movie? movie = library.Movies.FirstOrDefault(
                existingMovie => existingMovie.Id == id
            );

            if (movie == null)
            {
                return NotFound(new
                {
                    message = $"Aucun film avec l'identifiant TMDB {id} n'est présent dans la bibliothèque."
                });
            }

            return Ok(ToResponse(movie));
        }

        /// <summary>
        /// Recherche des films sur TMDB.
        /// Les résultats sont séparés entre les nouveaux films
        /// et ceux déjà présents dans la bibliothèque.
        /// GET /api/movies/search?title=batman
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new
                {
                    message = "Le paramètre 'title' est obligatoire."
                });
            }

            Library library = GetLibrary();
            var result = await library.searchMovie(title);

            return Ok(new
            {
                foundMovies = result.FoundMovies.Select(movie => ToResponse(movie)),
                alreadyInLibrary = result.AlreadyInLibrary.Select(movie => ToResponse(movie))
            });
        }

        /// <summary>
        /// Ajoute manuellement un film à la bibliothèque.
        /// Cette route est pratique pour les premiers tests Postman.
        /// POST /api/movies
        /// </summary>
        [HttpPost]
        public IActionResult AddMovie([FromBody] AddMovieRequest request)
        {
            Library library = GetLibrary();

            if (request.Id <= 0)
            {
                return BadRequest(new
                {
                    message = "L'identifiant TMDB doit être supérieur à 0."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new
                {
                    message = "Le titre du film est obligatoire."
                });
            }

            bool alreadyExists = library.Movies.Any(
                movie => movie.Id == request.Id
            );

            if (alreadyExists)
            {
                return Conflict(new
                {
                    message = $"Le film avec l'identifiant TMDB {request.Id} est déjà dans la bibliothèque."
                });
            }

            Status status = Status.NotSeen;

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !Enum.TryParse(request.Status, true, out status))
            {
                return BadRequest(new
                {
                    message = "Statut invalide. Valeurs possibles : Seen, NotSeen, InProgress."
                });
            }

            Movie movie = new Movie(
                request.Id,
                request.Title,
                request.Synopsis ?? string.Empty,
                request.Poster ?? string.Empty,
                request.RatingTMDB,
                status
            );

            if (request.PersonalRating.HasValue)
            {
                movie.PersonalRating = request.PersonalRating.Value;
            }

            if (request.PersonalNote != null)
            {
                movie.PersonalNote = request.PersonalNote;
            }

            library.addMovies(movie);

            return CreatedAtAction(
                nameof(GetMovie),
                new { id = movie.Id },
                ToResponse(movie)
            );
        }

        /// <summary>
        /// Supprime un film de la bibliothèque grâce à son identifiant TMDB.
        /// DELETE /api/movies/27205
        /// </summary>
        [HttpDelete("{id:int}")]
        public IActionResult DeleteMovie(int id)
        {
            Library library = GetLibrary();

            Movie? movie = library.Movies.FirstOrDefault(
                existingMovie => existingMovie.Id == id
            );

            if (movie == null)
            {
                return NotFound(new
                {
                    message = $"Aucun film avec l'identifiant TMDB {id} n'est présent dans la bibliothèque."
                });
            }

            library.removeMovie(movie);

            return NoContent();
        }

        /// <summary>
        /// Convertit un Movie en objet JSON simple pour les réponses HTTP.
        /// </summary>
        private static object ToResponse(Movie movie)
        {
            return new
            {
                id = movie.Id,
                title = movie.Title,
                synopsis = movie.Synopsis,
                poster = movie.Poster,
                ratingTMDB = movie.RatingTMDB,
                personalRating = movie.PersonalRating,
                personalNote = movie.PersonalNote,
                status = movie.Status.ToString()
            };
        }
    }

    /// <summary>
    /// Corps JSON attendu par POST /api/movies.
    /// </summary>
    public class AddMovieRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Synopsis { get; set; }
        public string? Poster { get; set; }
        public float RatingTMDB { get; set; }
        public float? PersonalRating { get; set; }
        public string? PersonalNote { get; set; }
        public string? Status { get; set; }
    }
}
