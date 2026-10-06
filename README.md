# ProjetPOO---Adriano-Nadia-et-Marc
## Movie library

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

- `Movie.cs` — Représente un film.
- `Library.cs` — Représente la bibliothèque personnelle.
- `Collection.cs` — Représente une collection de films.
- `WatchStatus.cs` — définit les différents statuts de visionnage.

### Classes/Tmdb/

Contient les classes utilisées pour représenter les données reçues depuis l'API TMDB :

- `MovieSearchResult.cs` - Représente le résultat d'une recherche de films.
- `TmdbMovieResult.cs` - Représente un film retourné par l'API TMDB.
- `TmdbSearchResponse.cs` - Représente la réponse globale retournée par TMDB.

### Services/

Contient la logique de l'application ainsi que la communication avec les services externes.

- `MoviesApi.cs` — gère les appels à l'API TMDB.

### Data/

Contient les éléments nécessaires à la configuration et à l'accès à MongoDB.

- `MongoDbService.cs` — gère la communication avec MongoDB.

### Controllers/

Contient les routes de manipulation des données de films

- `MoviesController.cs` - API REST permettant de manipuler les films de la bibliothèque.

### Pages/

Contient les pages Razor de l'application.

- `Movies/*` — recherche et affichage des films.
- `Library/*` — gestion de la bibliothèque personnelle.
- `Collections/*` — gestion des collections personnalisées.
- `Shared/*` — éléments communs aux différentes pages.

### wwwroot/

Contient les fichiers statiques utilisés par l'interface :

- `css/*` — fichiers CSS.
- `js/*` — fichiers JavaScript.
- `images/*` — images utilisées par l'application.
- `lib/*` — bibliothèques frontend.

### Properties/

Contient les fichiers de configuration liés au lancement de l'application, notamment `launchSettings.json`.

