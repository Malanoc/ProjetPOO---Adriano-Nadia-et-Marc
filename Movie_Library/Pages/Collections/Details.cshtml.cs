using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Movie_Library.Repositories;
using System.Threading.Tasks;

// Alias pour éviter les conflits avec
// le namespace Pages.Collections.
using CollectionClass = Movie_Library.Classes.Collection;
using LibraryClass = Movie_Library.Classes.Library;
using MovieClass = Movie_Library.Classes.Movie;

namespace Movie_Library.Pages.Collections
{
    /// <summary>
    /// Page représentant le détail d'une collection.
    ///
    /// Elle permet :
    /// - d'afficher les films de la collection ;
    /// - d'afficher les films disponibles dans la bibliothèque ;
    /// - d'ajouter plusieurs films à la collection.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly MovieRepository _movieRepository;
        private readonly CollectionRepository _collectionRepository;

        /// <summary>
        /// Bibliothèque principale de l'application.
        ///
        /// Elle correspond au singleton déclaré
        /// dans Program.cs.
        /// </summary>
        internal LibraryClass Library { get; private set; }

        /// <summary>
        /// Liste de tous les films présents dans la bibliothèque.
        ///
        /// Cette liste est chargée depuis MongoDB.
        /// Elle permet notamment de savoir quels films peuvent
        /// être ajoutés à la collection courante.
        /// </summary>
        public List<MovieClass> LibraryMovies
        {
            get;
            private set;
        } = new();


        /// <summary>
        /// Collection actuellement affichée.
        /// </summary>
        internal CollectionClass? CurrentCollection
        {
            get;
            private set;
        }


        /// <summary>
        /// Films de la bibliothèque qui ne sont pas encore
        /// présents dans la collection.
        /// </summary>
        internal List<MovieClass> AvailableMovies
        {
            get;
            private set;
        } = new();


        /// <summary>
        /// Nom de la collection.
        ///
        /// Cette valeur sera également envoyée dans
        /// le formulaire sous forme de champ caché.
        /// </summary>
        [BindProperty]
        public string CollectionName { get; set; } =
            string.Empty;

        /// <summary>
        /// Nom original de la collection.
        ///
        /// Il est important de le conserver car le nom sert actuellement
        /// d'identifiant pour retrouver la collection.
        ///
        /// Si l'utilisateur modifie "Marvel" en "Super-héros",
        /// nous devons encore savoir que la collection à modifier
        /// s'appelait initialement "Marvel".
        /// </summary>
        [BindProperty]
        public string OriginalCollectionName { get; set; } = string.Empty;


        /// <summary>
        /// Nouveau nom saisi par l'utilisateur.
        /// </summary>
        [BindProperty]
        public string EditCollectionName { get; set; } = string.Empty;


        /// <summary>
        /// URL du poster de la collection.
        /// </summary>
        [BindProperty]
        public string EditCollectionPoster { get; set; } = string.Empty;

        /// <summary>
        /// Liste des identifiants TMDB des films
        /// sélectionnés dans le formulaire.
        ///
        /// Plusieurs checkboxes peuvent donc être
        /// envoyées en une seule fois.
        /// </summary>
        [BindProperty]
        public List<int> SelectedMovieIds
        {
            get;
            set;
        } = new();


        /// <summary>
        /// Message affiché après l'ajout de films.
        ///
        /// TempData permet de conserver le message
        /// pendant la redirection.
        /// </summary>
        [TempData]
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Message affiché s'il y a une erreur lors l'ajout de films.
        ///
        /// TempData permet de conserver le message
        /// pendant la redirection.
        /// </summary>
        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Constructeur de la page.
        /// </summary>
        public DetailsModel(LibraryClass library,MovieRepository movieRepository,CollectionRepository collectionRepository)
        {
            Library = library;
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
        }


        /// <summary>
        /// Affiche la collection correspondant
        /// au nom présent dans l'URL.
        /// </summary>
        /// <param name="name">
        /// Nom unique de la collection.
        /// </param>
        public async Task<IActionResult> OnGetAsync(string name)
        {
            /*
            * Charge d'abord tous les films réellement
            * enregistrés dans MongoDB.
            */
            await LoadLibraryMoviesAsync();


            /*
             * Recherche ensuite la collection demandée.
             */
            bool collectionFound =
                LoadCollection(name);


            if (!collectionFound)
            {
                return NotFound();
            }


            CollectionName =
                CurrentCollection!.Name;

            OriginalCollectionName =
                CurrentCollection!.Name;

            EditCollectionName =
                CurrentCollection.Name;

            EditCollectionPoster =
                CurrentCollection.Poster;


            return Page();
        }

        /// <summary>
        /// Charge depuis MongoDB tous les films
        /// appartenant à la bibliothèque.
        /// </summary>
        private async Task LoadLibraryMoviesAsync()
        {
            try
            {
                LibraryMovies = await _movieRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Erreur lors du chargement des films : {ex.Message}"
        );

                // Utilise les films déjà présents en mémoire si disponibles.
                LibraryMovies = Library.Movies.ToList();

                ErrorMessage =
                    "Impossible de charger les films depuis la base de données. " +
                    "Les données disponibles en mémoire peuvent rester accessibles.";
            }
        }


        /// <summary>
        /// Ajoute un ou plusieurs films
        /// à la collection.
        /// </summary>
        public async Task<IActionResult> OnPostAddMoviesAsync()
        {
            /*
             * Recharge les films de la bibliothèque depuis MongoDB.
             *
             * Une requête POST est une nouvelle requête HTTP,
             * donc on recharge les données nécessaires.
             */
            await LoadLibraryMoviesAsync();


            /*
             * Recharge ensuite la collection.
             */
            bool collectionFound =
                LoadCollection(CollectionName);


            if (!collectionFound)
            {
                return NotFound();
            }


            /*
             * Vérifie que l'utilisateur a sélectionné
             * au moins un film.
             */
            if (SelectedMovieIds.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(SelectedMovieIds),
                    "Veuillez sélectionner au moins un film."
                );

                return Page();
            }


            /*
             * Compteur utilisé pour le message
             * de confirmation.
             */
            int addedMoviesCount = 0;
            List<MovieClass> newlyAddedMovies = new();


            /*
             * Parcourt les identifiants envoyés
             * par le formulaire.
             *
             * Distinct() empêche également qu'un même ID
             * envoyé plusieurs fois soit traité plusieurs fois.
             */
            foreach (int movieId in
                     SelectedMovieIds.Distinct())
            {
                /*
                 * On accepte uniquement un film qui existe
                 * réellement dans la bibliothèque.
                 */
                MovieClass? movie =
                    LibraryMovies.FirstOrDefault(
                        libraryMovie =>
                            libraryMovie.Id == movieId
                    );


                if (movie == null)
                {
                    continue;
                }


                /*
                 * Vérifie que le film n'existe pas déjà
                 * dans cette collection.
                 *
                 * Même si le formulaire n'affiche normalement
                 * que les films disponibles, cette vérification
                 * côté serveur reste importante.
                 */
                bool alreadyExists =
                    CurrentCollection!.Movies.Any(
                        collectionMovie =>
                            collectionMovie.Id == movie.Id
                    );


                /*
                 * Film déjà présent :
                 * on ne l'ajoute pas.
                 */
                if (alreadyExists)
                {
                    continue;
                }


                /*
                 * Utilisation de la méthode existante
                 * de Collection.
                 *
                 * Aucune modification de Collection.cs
                 * n'est nécessaire.
                 */
                CurrentCollection.addMovie(movie);
                newlyAddedMovies.Add(movie);

                addedMoviesCount++;
            }


            /*
             * Aucun nouveau film n'a finalement
             * pu être ajouté.
             */
            if (addedMoviesCount == 0)
            {
                ModelState.AddModelError(
                    nameof(SelectedMovieIds),
                    "Les films sélectionnés sont déjà présents dans cette collection."
                );

                /*
                 * On recalcule la liste des films disponibles
                 * avant de réafficher la page.
                 */
                LoadAvailableMovies();

                return Page();
            }

            try
            {
                await _collectionRepository.SaveAsync(CurrentCollection!);
            }
            catch (Exception ex)
            {
                // Annule les ajouts en mémoire si MongoDB refuse la sauvegarde.
                foreach (MovieClass addedMovie in newlyAddedMovies)
                {
                    CurrentCollection!.RemoveMovie(addedMovie);
                }

                Console.Error.WriteLine(
                    $"Erreur lors de la sauvegarde de la collection : {ex}"
                );

                ErrorMessage =
                    "Impossible d'enregistrer la collection. " +
                    "Vérifiez votre connexion à la base de données, puis réessayez.";

                return RedirectToPage(
                    "/Collections/Details",
                    new { name = CollectionName }
                );
            }

            /*
             * Message de confirmation.
             */
            if (addedMoviesCount == 1)
            {
                SuccessMessage =
                    "Le film a été ajouté à la collection.";
            }
            else
            {
                SuccessMessage =
                    $"{addedMoviesCount} films ont été ajoutés à la collection.";
            }


            /*
             * Redirection vers la page de la collection.
             *
             * Cette redirection évite qu'un rafraîchissement
             * du navigateur renvoie le formulaire.
             */
            return RedirectToPage(
                "/Collections/Details",
                new
                {
                    name = CollectionName
                }
            );
        }

        /// <summary>
        /// Retire un film de la collection actuellement affichée.
        ///
        /// ATTENTION :
        /// le film n'est PAS supprimé de la bibliothèque
        /// et n'est PAS supprimé de MongoDB.
        ///
        /// Seule l'association entre le film et la collection
        /// est supprimée.
        /// </summary>
        public async Task <IActionResult> OnPostRemoveMovieAsync(
            [FromRoute] string name,
            [FromForm] int id)
        {
            /*
             * Recherche la collection correspondant
             * au nom présent dans l'URL.
             *
             * Exemple :
             *
             * /Collections/Marvel
             *
             * name = "Marvel"
             */
            CollectionClass? collection =
                Library.Collections.FirstOrDefault(
                    existingCollection =>
                        string.Equals(
                            existingCollection.Name,
                            name,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            /*
             * La collection n'existe plus.
             */
            if (collection == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Cette collection n'existe plus."
                });
            }


            /*
             * Recherche le film uniquement
             * dans cette collection.
             */
            MovieClass? movie =
                collection.Movies.FirstOrDefault(
                    collectionMovie =>
                        collectionMovie.Id == id
                );


            /*
             * Le film n'est pas présent
             * dans cette collection.
             */
            if (movie == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ce film n'est plus présent dans cette collection."
                });
            }


            /*
             * Retire uniquement le film de la collection.
             *
             * MongoDB n'est pas touché.
             * Le film reste donc dans la bibliothèque.
             */
            collection.RemoveMovie(movie);
            try
            {
                await _collectionRepository.SaveAsync(collection);
            }
            catch (Exception ex)
            {
                // Restaure l'association si la sauvegarde échoue.
                collection.addMovie(movie);

                Console.Error.WriteLine(
                    $"Erreur lors du retrait d'un film de la collection : {ex}"
                );

                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Impossible d'enregistrer le retrait du film. " +
                        "Vérifiez la connexion à la base de données, puis réessayez."
                });
            }


            return new JsonResult(new
            {
                success = true,
                id = movie.Id,
                message =
                    $"« {movie.Title} » a été retiré de la collection."
            });
        }

        /// <summary>
        /// Modifie le nom et le poster de la collection.
        /// </summary>
        public async Task<IActionResult> OnPostUpdateCollectionAsync()
        {
            /*
             * Recharge les films depuis MongoDB.
             *
             * Cela est nécessaire si nous devons réafficher
             * la page à cause d'une erreur de validation.
             */
            await LoadLibraryMoviesAsync();


            /*
             * Recherche la collection grâce à son ANCIEN nom.
             *
             * On ne peut pas utiliser le nouveau nom puisque
             * la modification n'a pas encore été effectuée.
             */
            bool collectionFound =
                LoadCollection(OriginalCollectionName);


            if (!collectionFound)
            {
                return NotFound();
            }


            /*
             * Nettoyage du nouveau nom.
             *
             * Exemple :
             *
             * "   Marvel   "
             *
             * devient :
             *
             * "Marvel"
             */
            string newName =
                EditCollectionName.Trim();


            /*
             * Le nom d'une collection est obligatoire.
             */
            if (string.IsNullOrWhiteSpace(newName))
            {
                ModelState.AddModelError(
                    nameof(EditCollectionName),
                    "Le nom de la collection est obligatoire."
                );

                return Page();
            }


            /*
             * Vérifie qu'aucune AUTRE collection
             * ne possède déjà ce nom.
             *
             * Le nom d'une collection doit être unique.
             */
            bool nameAlreadyExists =
                Library.Collections.Any(
                    collection =>
                        !ReferenceEquals(
                            collection,
                            CurrentCollection
                        )
                        &&
                        string.Equals(
                            collection.Name,
                            newName,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            if (nameAlreadyExists)
            {
                ModelState.AddModelError(
                    nameof(EditCollectionName),
                    "Une collection avec ce nom existe déjà."
                );

                return Page();
            }


            /*
             * Modification du nom.
             *
             * Collection.Name possède déjà un setter,
             * donc aucune modification de Collection.cs
             * n'est nécessaire.
             */

            string oldName = CurrentCollection!.Name;
            string oldPoster = CurrentCollection.Poster;

            // Modifie la collection en mémoire.
            CurrentCollection.Name = newName;
            CurrentCollection.Poster =
                EditCollectionPoster?.Trim() ?? string.Empty;

            // Enregistre la nouvelle version dans MongoDB.
            try
            {
                await _collectionRepository.SaveAsync(CurrentCollection);
            }
            catch (Exception ex)
            {
                // Restaure les anciennes valeurs si la sauvegarde échoue.
                CurrentCollection.Name = oldName;
                CurrentCollection.Poster = oldPoster;

                Console.Error.WriteLine(
                    $"Erreur lors de la sauvegarde de la collection : {ex.Message}"
                );

                ErrorMessage =
                    "Impossible d'enregistrer la collection. " +
                    "Vérifiez la connexion à la base de données, puis réessayez.";

                return RedirectToPage(
                    "/Collections/Details",
                    new { name = oldName }
                );
            }

            // Supprime l'ancien document uniquement si le nom a changé.
            if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                try
                {
                    bool deleted =
                        await _collectionRepository.DeleteAsync(oldName);

                    if (!deleted)
                    {
                        ErrorMessage =
                            "La nouvelle collection a été enregistrée, " +
                            "mais l'ancien enregistrement n'a pas été supprimé.";

                        return RedirectToPage(
                            "/Collections/Details",
                            new { name = newName }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(
                        $"Erreur lors de la suppression de l'ancienne collection : {ex.Message}"
                    );

                    ErrorMessage =
                        "La nouvelle collection a été enregistrée, mais la suppression " +
                        "de l'ancien enregistrement a échoué. Vérifiez la base de données.";

                    return RedirectToPage(
                        "/Collections/Details",
                        new { name = newName }
                    );
                }
            }

            /*
             * Message affiché après redirection.
             */
            SuccessMessage =
                "La collection a été modifiée avec succès.";


            /*
             * IMPORTANT :
             *
             * L'URL de la page utilise le nom de la collection :
             *
             * /Collections/Marvel
             *
             * Si le nom devient "Super-héros",
             * il faut donc rediriger vers :
             *
             * /Collections/Super-héros
             */
            return RedirectToPage(
                "/Collections/Details",
                new
                {
                    name = newName
                }
            );
        }

        /// <summary>
        /// Recherche une collection par son nom
        /// et prépare ses films disponibles.
        /// </summary>
        /// <param name="name">
        /// Nom unique de la collection.
        /// </param>
        /// <returns>
        /// true si la collection existe,
        /// sinon false.
        /// </returns>
        private bool LoadCollection(string name)
        {
            /*
             * Recherche de la collection grâce à son nom.
             *
             * OrdinalIgnoreCase permet par exemple de considérer
             * "Marvel" et "marvel" comme le même nom.
             */
            CurrentCollection =
                Library.Collections.FirstOrDefault(
                    collection =>
                        string.Equals(
                            collection.Name,
                            name,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            if (CurrentCollection == null)
            {
                return false;
            }


            CollectionName =
                CurrentCollection.Name;


            /*
             * Prépare la liste des films encore disponibles.
             */
            LoadAvailableMovies();


            return true;
        }


        /// <summary>
        /// Recherche tous les films de la bibliothèque
        /// qui ne sont pas encore dans la collection.
        /// </summary>
        private void LoadAvailableMovies()
        {
            if (CurrentCollection == null)
            {
                AvailableMovies =
                    new List<MovieClass>();

                return;
            }


            /*
             * On utilise les films chargés depuis MongoDB.
             *
             * On conserve uniquement ceux qui ne sont
             * pas encore présents dans la collection.
             */
            AvailableMovies =
                LibraryMovies
                    .Where(
                        libraryMovie =>
                            !CurrentCollection.Movies.Any(
                                collectionMovie =>
                                    collectionMovie.Id ==
                                    libraryMovie.Id
                            )
                    )
                    .OrderBy(
                        movie =>
                            movie.Title
                    )
                    .ToList();
        }
    }
}