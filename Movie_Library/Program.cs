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

app.Run();