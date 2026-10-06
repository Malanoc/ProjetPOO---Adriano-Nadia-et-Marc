using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    // Utilisation directe du chemin complet pour s'assurer que .NET trouve la classe sans erreur
    private readonly Movie_Library.Data.MongoDbService _mongoDbService;

    public string MongoStatus { get; set; } = "";

    // Utilisation du chemin complet également dans le constructeur
    public IndexModel(Movie_Library.Data.MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public void OnGet()
    {
        // La méthode reste vide, le test de démarrage est géré par Program.cs
    }
}

