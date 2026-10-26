using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
 
    public string MongoStatus { get; set; } = "";

    public void OnGet()
    {
        // La méthode reste vide, le test de démarrage est géré par Program.cs
    }
}

