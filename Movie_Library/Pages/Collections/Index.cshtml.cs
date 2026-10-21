using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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
    }
}