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

// Récupère la bibliothèque partagée de l'application.
    var library = app.Services.GetRequiredService<Library>();

if (mongoService.TestConnection())
{
    Console.WriteLine("Connexion avec MongoDB réussie.");

    

    try
    { 
        // Récupère tous les films sauvegardés dans MongoDB.
        var movies = await mongoService.GetMoviesAsync();

        // Charge les films dans la bibliothèque en mémoire.
        library.LoadMovies(movies);

        Console.WriteLine(
            $"{movies.Count} film(s) chargé(s) depuis MongoDB."
        );


        // Récupère les collections et leurs films depuis MongoDB.
        var collections = await mongoService.LoadCollectionsAsync(movies);

        // Charge les collections récupérées dans la bibliothèque en mémoire.
        library.LoadCollections(collections);

        Console.WriteLine(
            $"{collections.Count} collection(s) chargée(s) depuis MongoDB."
        );
    }
    catch (Exception ex)
    {
        library.StartupDataWarning =
            "Les données sauvegardées n'ont pas pu être chargées.";

        Console.Error.WriteLine(
            $"Erreur lors du chargement des données MongoDB : {ex}"
        );
    }
}

else
{
    library.StartupDataWarning =
        "Impossible de se connecter à MongoDB. " +
        "Les données sauvegardées peuvent être indisponibles.";

    Console.Error.WriteLine(
        "Échec de la connexion à MongoDB. Vérifiez que le serveur est démarré."
    );
}

app.Run();