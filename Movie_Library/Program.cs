using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Movie_Library; // S'aligne directement sur le namespace simplifié

var builder = WebApplication.CreateBuilder(args);

// C# trouve MongoDbService tout seul car ils partagent le même espace
builder.Services.AddSingleton<MongoDbService>();

// --- CODE DE TEST DE CONNEXION RAPIDE ---
var tempProvider = builder.Services.BuildServiceProvider();
var mongoService = tempProvider.GetService<MongoDbService>();

if (mongoService != null && mongoService.FastTest())
{
    Console.WriteLine("=================================");
    Console.WriteLine("✅ MONGODB EST CONNECTÉ AVEC SUCCÈS !");
    Console.WriteLine("=================================");
}
else
{
    Console.WriteLine("=================================");
    Console.WriteLine("❌ IMPOSSIBLE DE SE CONNECTER À MONGODB.");
    Console.WriteLine("=================================");
}

builder.Services.AddRazorPages();

var app = builder.Build();

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

app.Run();
