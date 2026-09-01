using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public string MessageBienvenue { get; set; } = string.Empty;
    public string ModalMessage { get; set; } = string.Empty;

    public void OnGet()
    {
        // Logique métier ou appel de service en C#
        MessageBienvenue = "Bonjour, propulsé par C# .NET !";
        ModalMessage = "Hello World";
    }
}
