using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Library.Classes;
using Movie_Library.Data;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Library
{
    public class IndexModel : PageModel
    {
        private readonly MongoDbService _mongoDbService;

        /// <summary>
        /// Bibliothèque affichée dans la page.
        /// </summary>
        internal LibraryClass Library { get; private set; }

        /// <summary>
        /// Initialise le modèle de la page.
        /// MongoDbService est injecté par ASP.NET Core.
        /// </summary>
        public IndexModel(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            Library = new LibraryClass("Ma bibliothèque");
        }

        /// <summary>
        /// Charge les films depuis MongoDB à l'ouverture de la page.
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadLibraryAsync();
        }

        /// <summary>
        /// Recharge la bibliothèque depuis MongoDB.
        /// </summary>
        private async Task LoadLibraryAsync()
        {
            List<Movie> movies =
                await _mongoDbService.GetMoviesAsync();

            foreach (Movie movie in movies)
            {
                Library.addMovies(movie);
            }
        }

        /// <summary>
        /// Enregistre le statut et les données personnelles d'un film.
        /// Handler appelé par POST avec le nom SaveDetails.
        /// </summary>
        public async Task<IActionResult> OnPostSaveDetailsAsync(
            [FromForm] int id,
            [FromForm] string status,
            [FromForm] float personalRating,
            [FromForm] string? personalNote)
        {
            // Vérifie que le statut reçu correspond à une valeur valide.
            if (!Enum.TryParse(status, true, out Status parsedStatus)
                || !Enum.IsDefined(typeof(Status), parsedStatus))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Le statut sélectionné n'est pas valide."
                });
            }

            // Vérifie la note personnelle.
            if (float.IsNaN(personalRating)
                || float.IsInfinity(personalRating)
                || personalRating < 0
                || personalRating > 10)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "La note personnelle doit être comprise entre 0 et 10."
                });
            }

            // Enregistre les modifications dans MongoDB.
            bool updated =
                await _mongoDbService.UpdateMovieDetailsAsync(
                    id,
                    parsedStatus,
                    personalRating,
                    personalNote ?? string.Empty
                );

            return new JsonResult(new
            {
                success = updated,
                status = parsedStatus.ToString(),
                personalRating,
                personalNote = personalNote ?? string.Empty,
                message = updated
                    ? "Les modifications ont été enregistrées."
                    : "Ce film n'existe plus dans la bibliothèque."
            });
        }

        /// <summary>
        /// Supprime un film de la bibliothèque.
        /// Handler appelé par POST avec le nom Delete.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(
            [FromForm] int id)
        {
            bool deleted =
                await _mongoDbService.DeleteMovieAsync(id);

            return new JsonResult(new
            {
                success = deleted,
                id,
                message = deleted
                    ? "Le film a été supprimé de votre bibliothèque."
                    : "Ce film n'existe plus dans la bibliothèque."
            });
        }
    }
}