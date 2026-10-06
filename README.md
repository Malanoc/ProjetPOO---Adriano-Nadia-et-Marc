# ProjetPOO---Adriano-Nadia-et-Marc - Movie library

# Introduction

Créer une application web pour aider un utilisateur à gérer sa filmographie personnelle. L’application permet de rechercher et catégoriser des films pour garder une trace des films visionnés.

## Fonctionnalités:

- Rechercher un film.
- Récupérer les films via une API.
- Films classés dans des bibliothèques.
- Chaque film : synopsis, affiche et une note imdb.
- Gérer le statut de visionnage (vu, en cours , non vu) sélectionné par l'user.
- Films triés dans cette bibliothèque avec un système de tri/filtre.
- Créer des collections personnalisées pour agréger des films ensembles accessibles avec un bouton encart dédié pour chaque collection dans l'interface de l'application.
- Interface web responsive.
- Notes personnelles.
- En bonus: mode sombre, comptes utilisateurs, films favoris, etc.

# User Story

| ID  | User Story | Valeur |
| :---: | :---: | :---: |
| US01 | En tant qu’utilisateur, je veux créer ma propre bibliothèque de film | 5 |
| US02 | En tant qu’utilisateur, je veux rechercher un film | 5 |
| US03 | En tant qu’utilisateur, Je veux ajouter un film à ma bibliothèque | 5 |
| US04 | En tant qu’utilisateur, je veux supprimer un film de ma bibliothèque | 5 |
| US05 | En tant qu’utilisateur, je veux voir l’affiche et les détails de mes films | 4 |
| US06 | En tant qu’utilisateur, je veux noter si le film est vu, en cours ou non vu | 3 |
| US07 | En tant qu’utilisateur, je veux trier mes films | 3 |
| US08 | En tant qu’utilisateur, je veux créer des collections personnelles | 2 |
| US09 | En tant qu’utilisateur, je veux que ma bibliothèque sauvegarde automatiquement | 5 |
| US10 | En tant qu’utilisateur, je veux consulter ma bibliothèque sur mon PC et Téléphone | 2 |
| US11 | En tant qu’utilisateur, je veux pouvoir prendre des notes sur les films que j’ai visionnés | 3 |

# Technologie

| Frontend  | Backend | Base de données |
| :---: | :---: | :---: |
| HTML / CSS / JS + Bootstrap | C# / ASP.NET Core | MongoDB |

# Diagramme de classes

<img src="/docs/Movie_Library_UML.png"  alt="Diagramme de classes UML - Movie Library"/>

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
├── Controllers/
│   └── MoviesController.cs
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

