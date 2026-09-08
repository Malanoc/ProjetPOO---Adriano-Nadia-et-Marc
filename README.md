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
Properties/	Configuration de lancement de l'application^
```

## Description des dossiers

### Classes/

Contient les classes principales de l'application :

- `Movie.cs` — représente un film.
- `Library.cs` — représente la bibliothèque personnelle.
- `Collection.cs` — représente une collection de films.
- `WatchStatus.cs` — définit les différents statuts de visionnage.

### Classes/Tmdb/

Contient les classes utilisées pour représenter les données reçues depuis l'API TMDB :

- `MovieSearchResult.cs`
- `TmdbMovieResult.cs`
- `TmdbSearchResponse.cs`

### Services/

Contient la logique de l'application ainsi que la communication avec les services externes.

- `MoviesApi.cs` — gère les appels à l'API TMDB.

### Data/

Contient les éléments nécessaires à la configuration et à l'accès à MongoDB.

- `MongoDbService.cs` — gère la communication avec MongoDB.

### Pages/

Contient les pages Razor de l'application.

- `Pages/Movies/` — recherche et affichage des films.
- `Pages/Library/` — gestion de la bibliothèque personnelle.
- `Pages/Collections/` — gestion des collections personnalisées.
- `Pages/Shared/` — éléments communs aux différentes pages.

### wwwroot/

Contient les fichiers statiques utilisés par l'interface :

- `css/` — fichiers CSS.
- `js/` — fichiers JavaScript.
- `images/` — images utilisées par l'application.
- `lib/` — bibliothèques frontend.

### Properties/

Contient les fichiers de configuration liés au lancement de l'application, notamment `launchSettings.json`.
