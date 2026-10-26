using Movie_Library.Classes;
using Movie_Library.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Backend API REST.
builder.Services.AddControllers();

// Une seule bibliothèque partagée dans toute l'application.
builder.Services.AddSingleton<Library>(
    _ => new Library("Ma bibliothèque")
);

// Service MongoDB partagé dans toute l'application.
builder.Services.AddSingleton<Movie_Library.Data.MongoDbService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    return new Movie_Library.Data.MongoDbService(configuration);
});

builder.Services.AddSingleton<MovieRepository>();
builder.Services.AddSingleton<CollectionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Active les routes /api/...
app.MapControllers();

// Test de connexion MongoDB au démarrage.
var mongoService =
    app.Services.GetRequiredService<Movie_Library.Data.MongoDbService>();

if (mongoService.TestConnection())
{
    Console.WriteLine("Connexion avec MongoDB réussie.");

    // Récupère la bibliothèque partagée de l'application.
    var library = app.Services.GetRequiredService<Library>();

    try
    {
        // Récupère les collections et leurs films depuis MongoDB.
        var collections = await mongoService.LoadCollectionsAsync();

        // Charge les collections récupérées dans la bibliothèque en mémoire.
        library.LoadCollections(collections);

        Console.WriteLine(
            $"{collections.Count} collection(s) chargée(s) depuis MongoDB."
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Erreur lors du chargement des collections : {ex.Message}"
        );
    }
}

else
{
    Console.WriteLine(
        "Echec de la connexion avec MongoDB. " +
        "Verifiez que votre serveur local est demarre."
    );
}

app.Run();