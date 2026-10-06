using Movie_Library.Classes;

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
}
else
{
    Console.WriteLine(
        "Echec de la connexion avec MongoDB. " +
        "Verifiez que votre serveur local est demarre."
    );
}

app.Run();