using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;

// Alias pour éviter les confusions entre
// le namespace Pages.Collections et la classe Collection.
using CollectionClass = Movie_Library.Classes.Collection;
using LibraryClass = Movie_Library.Classes.Library;

namespace Movie_Library.Pages.Collections
{
    /// <summary>
    /// Modèle de la page permettant de gérer
    /// les collections de films.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Bibliothèque principale de l'application.
        ///
        /// Elle est injectée par ASP.NET Core et correspond
        /// au singleton déclaré dans Program.cs.
        /// </summary>
        public LibraryClass Library { get; private set; }


        /// <summary>
        /// Nom de la nouvelle collection saisi
        /// dans le formulaire.
        ///
        /// BindProperty permet à ASP.NET Core de récupérer
        /// automatiquement la valeur envoyée par le formulaire.
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Le nom de la collection est obligatoire.")]
        public string CollectionName { get; set; } = string.Empty;


        /// <summary>
        /// Message affiché après la création
        /// réussie d'une collection.
        ///
        /// TempData permet de conserver le message
        /// après la redirection.
        /// </summary>
        [TempData]
        public string? Message { get; set; }


        /// <summary>
        /// Constructeur de la page.
        ///
        /// ASP.NET Core fournit automatiquement
        /// la Library enregistrée dans Program.cs.
        /// </summary>
        public IndexModel(LibraryClass library)
        {
            Library = library;
        }


        /// <summary>
        /// Affiche simplement la page.
        /// </summary>
        public void OnGet()
        {
        }


        /// <summary>
        /// Traite le formulaire de création
        /// d'une nouvelle collection.
        /// </summary>
        public IActionResult OnPostCreate()
        {
            /*
             * Vérifie les règles de validation.
             *
             * Dans notre cas, [Required] vérifie
             * principalement que CollectionName
             * contient une valeur.
             */
            if (!ModelState.IsValid)
            {
                return Page();
            }


            /*
             * Trim() supprime les espaces inutiles
             * avant et après le nom.
             *
             * Exemple :
             *
             * "   Science-fiction   "
             *
             * devient :
             *
             * "Science-fiction"
             */
            string collectionName =
                CollectionName.Trim();


            /*
             * Required ne considère pas forcément notre
             * nettoyage Trim(), on effectue donc également
             * une vérification explicite.
             */
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                ModelState.AddModelError(
                    nameof(CollectionName),
                    "Le nom de la collection est obligatoire."
                );

                return Page();
            }

            /*
             * Vérifie qu'une collection avec le même nom
             * n'existe pas déjà.
             */
            bool collectionAlreadyExists =
                Library.Collections.Any(
                    collection =>
                        string.Equals(
                            collection.Name,
                            collectionName,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            if (collectionAlreadyExists)
            {
                ModelState.AddModelError(
                    nameof(CollectionName),
                    "Une collection avec ce nom existe déjà."
                );

                return Page();
            }

            /*
             * Création de la collection.
             *
             * Pour le moment, aucune image/poster
             * n'est demandé dans le formulaire.
             *
             * On passe donc une chaîne vide.
             */
            CollectionClass collection =
                new CollectionClass(
                    collectionName,
                    string.Empty
                );


            /*
             * Ajoute la collection à notre bibliothèque.
             */
            Library.addCollection(collection);


            /*
             * Prépare le message de confirmation.
             */
            Message =
                $"La collection « {collectionName} » a été créée avec succès.";


            /*
             * Redirection vers la même page.
             *
             * Cela évite qu'un rafraîchissement du navigateur
             * renvoie le formulaire une deuxième fois.
             */
            return RedirectToPage();
        }

        /// <summary>
        /// Supprime une collection de la bibliothèque.
        ///
        /// Les films contenus dans la collection ne sont PAS
        /// supprimés de la bibliothèque.
        ///
        /// Seule la collection est supprimée.
        /// </summary>
        public IActionResult OnPostDelete(
            [FromForm] string collectionName)
        {
            /*
             * Recherche la collection grâce à son nom.
             *
             * Le nom étant unique, il permet d'identifier
             * la collection à supprimer.
             */
            CollectionClass? collection =
                Library.Collections.FirstOrDefault(
                    existingCollection =>
                        string.Equals(
                            existingCollection.Name,
                            collectionName,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            /*
             * Si la collection n'existe plus,
             * on affiche simplement un message.
             */
            if (collection == null)
            {
                Message =
                    "Cette collection n'existe plus.";

                return RedirectToPage();
            }


            /*
             * Suppression de la collection.
             *
             * Cette opération ne supprime PAS les films
             * de MongoDB ou de la bibliothèque.
             */
            Library.removeCollection(collection);


            Message =
                $"La collection « {collection.Name} » a été supprimée.";


            /*
             * Retour vers la page des collections.
             */
            return RedirectToPage();
        }
    }
}