# ProjetPOO---Adriano-Nadia-et-Marc
Movie library

# Structure du projet

```text
Movie_Library/
│
├── Classes/
│   ├── Movie.cs
│   ├── Library.cs
│   ├── Collection.cs
│   ├── WatchStatus.cs
│   │
│   └── Tmdb/
│       ├── MovieSearchResult.cs
│       ├── TmdbMovieResult.cs
│       └── TmdbSearchResponse.cs
│
├── Services/
│   └── MoviesApi.cs
│
├── Data/
│   └── MongoDbService.cs
│
├── Pages/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   └── _ValidationScriptsPartial.cshtml
│   │
│   ├── Movies/
│   │   ├── Index.cshtml
│   │   ├── Index.cshtml.cs
│   │   ├── Details.cshtml
│   │   └── Details.cshtml.cs
│   │
│   ├── Library/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   │
│   ├── Collections/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Create.cshtml.cs
│   │   ├── Details.cshtml
│   │   └── Details.cshtml.cs
│   │
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   ├── Privacy.cshtml
│   ├── Privacy.cshtml.cs
│   ├── Error.cshtml
│   └── Error.cshtml.cs
│
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   │
│   ├── js/
│   │   └── site.js
│   │
│   ├── images/
│   │
│   └── lib/
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── Movie_Library.csproj

Pages/Shared/	Éléments communs aux différentes pages
wwwroot/	Fichiers statiques : CSS, JavaScript, images et bibliothèques
Properties/	Configuration de lancement de l'application
