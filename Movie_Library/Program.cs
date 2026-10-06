
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Connect to MongoDB service and enable it across the entire web application

//builder.Services.AddSingleton<Movie_Library.Data.MongoDbService>();

//Configuration de la base MongoDB
//Enregistre le service en mode "Singleton" (une seule instance unique pour tout le site).
//On passe la configuration('sp.GetRequiredService') pour que le service lise l'adresse de connexion et le nom de la base dans le fichier appsettings.json.
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

// TEST DE CONNEXION BD
// On récupère le service pour exécuter la vérification
// CORRECTION : On ajoute "Movie_Library.Data." devant le nom du service
var mongoService = app.Services.GetRequiredService<Movie_Library.Data.MongoDbService>();


// On écrit un texte simple et épuré dans la console selon le résultat
if (mongoService.TestConnection())
{
    Console.WriteLine("Connexion avec MongoDB réussie.");
}
else
{
    Console.WriteLine("Echec de la connexion avec MongoDB. Verifiez que votre serveur local est demarre.");
}


app.Run();