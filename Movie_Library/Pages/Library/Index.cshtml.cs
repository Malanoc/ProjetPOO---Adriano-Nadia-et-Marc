using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Library.Classes;
using Movie_Library.Repositories;
using System.Linq;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Library
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Repository responsable de la gestion des collections.
        /// Il permet d'accéder aux collections sans appeler
        /// directement le service de base de données.
        /// </summary>
        private readonly CollectionRepository _collectionRepository;
        /// <summary>
        /// Repository responsable de la gestion des films.
        /// Il permet d'accéder aux données des films dans MongoDB
        /// sans appeler directement le service de base de données.
        /// </summary>
        /// 
        private readonly MovieRepository _movieRepository;

        /// <summary>
        /// Bibliothèque partagée de l'application.
        ///
        /// Cette instance correspond au singleton déclaré dans Program.cs.
        /// Elle contient notamment les collections créées par l'utilisateur.
        /// </summary>
        private readonly LibraryClass _sharedLibrary;

        /// <summary>
        /// Bibliothèque affichée dans la page.
        /// </summary>
        internal LibraryClass Library { get; private set; }

        /// <summary>
        /// Initialise le modèle de la page.
        /// Les repositories et la bibliothèque partagée
        /// sont injectés par ASP.NET Core.
        /// </summary>
        public IndexModel(MovieRepository movieRepository,CollectionRepository collectionRepository,LibraryClass sharedLibrary)
        {
            _movieRepository = movieRepository;

            _collectionRepository = collectionRepository;
            /*
             * Bibliothèque singleton partagée dans toute l'application.
             *
             * C'est ici que sont actuellement stockées
             * les collections.
             */
            _sharedLibrary = sharedLibrary;
            /*
             * Cette bibliothèque reste utilisée uniquement
             * pour afficher les films récupérés depuis MongoDB
             * sur cette page.
             */
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
                await _movieRepository.GetAllAsync();

            Library = new LibraryClass("Ma bibliothèque");

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
                await _movieRepository.UpdateDetailsAsync(
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
        ///
        /// Lorsqu'un film est supprimé de la bibliothèque,
        /// il est également retiré automatiquement
        /// de toutes les collections qui le contiennent.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(
            [FromForm] int id)
        {
            /*
             * Suppression du film dans MongoDB.
             *
             * MongoDB représente actuellement la source principale
             * des films présents dans la bibliothèque.
             */
            bool deleted =
                await _movieRepository.DeleteAsync(id);


            /*
             * Si le film a bien été supprimé de la bibliothèque,
             * on doit également le retirer de toutes les collections.
             */
            if (deleted)
            {
                /*
                 * Parcourt toutes les collections.
                 */
                foreach (var collection in _sharedLibrary.Collections)
                {
                    /*
                     * Recherche dans cette collection le film
                     * possédant le même identifiant TMDB.
                     */
                    Movie? movieInCollection =
                        collection.Movies.FirstOrDefault(
                            movie =>
                                movie.Id == id
                        );


                    /*
                     * Si le film est présent dans cette collection,
                     * on le retire.
                     */
                    if (movieInCollection != null)
                    {
                        collection.RemoveMovie(
                            movieInCollection
                        );

                        // Sauvegarde la collection après sa modification dans MongoDB.
                        await _collectionRepository.SaveAsync(collection);
                    }
                }
            }


            return new JsonResult(new
            {
                success = deleted,
                id,

                message = deleted
                    ? "Le film a été supprimé de votre bibliothèque et de vos collections."
                    : "Ce film n'existe plus dans la bibliothèque."
            });
        }
    }
}