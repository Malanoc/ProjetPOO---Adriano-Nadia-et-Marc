document.addEventListener("DOMContentLoaded", () => {
    const resultsGrid = document.getElementById("movies-results-grid");
    const modalElement = document.getElementById("search-movie-modal");

    // La page peut être affichée sans résultat de recherche.
    if (!resultsGrid || !modalElement || !window.bootstrap) {
        return;
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    const modalTitle = document.getElementById(
        "search-movie-modal-title"
    );

    const modalSynopsis = document.getElementById(
        "search-movie-modal-synopsis"
    );

    const modalRating = document.getElementById(
        "search-movie-modal-rating"
    );

    const modalPoster = document.getElementById(
        "search-movie-modal-poster"
    );

    const noPoster = document.getElementById(
        "search-movie-modal-no-poster"
    );

    const addForm = document.getElementById(
        "search-add-movie-form"
    );

    const addButton = document.getElementById(
        "search-add-movie-button"
    );

    const alreadyAddedMessage = document.getElementById(
        "search-movie-already-added"
    );

    // Remplit les champs cachés du formulaire d'ajout.
    function populateAddForm(movie) {
        document.getElementById("search-add-movie-id").value =
            movie.id;

        document.getElementById("search-add-movie-title").value =
            movie.title;

        document.getElementById("search-add-movie-synopsis").value =
            movie.synopsis;

        document.getElementById("search-add-movie-poster").value =
            movie.poster;

        // Utilise le point comme séparateur décimal dans le champ caché.
        document.getElementById("search-add-movie-rating").value =
            String(movie.rating);

        document.getElementById("search-add-movie-search-term").value =
            movie.searchTerm;
    }

    // Ouvre la modale avec les informations de la carte sélectionnée.
    function openMovieModal(card) {
        const movie = {
            id: card.dataset.movieId || "",
            title: card.dataset.title || "",
            synopsis: card.dataset.synopsis || "",
            poster: card.dataset.poster || "",
            rating: Number.parseFloat(card.dataset.rating || "0"),
            searchTerm: card.dataset.searchTerm || ""
        };

        const alreadyInLibrary =
            card.dataset.inLibrary === "true";

        modalTitle.textContent = movie.title;

        modalSynopsis.textContent =
            movie.synopsis || "Aucun synopsis disponible.";

        modalRating.textContent =
            `${movie.rating.toLocaleString("fr-CH", {
                minimumFractionDigits: 1,
                maximumFractionDigits: 1
            })} / 10`;

        // Affiche l'affiche ou le remplacement.
        if (movie.poster) {
            modalPoster.src = movie.poster;
            modalPoster.alt = movie.title;

            modalPoster.classList.remove("d-none");
            noPoster.classList.add("d-none");
        } else {
            modalPoster.removeAttribute("src");
            modalPoster.alt = "";

            modalPoster.classList.add("d-none");
            noPoster.classList.remove("d-none");
        }

        // Un film déjà enregistré ne peut pas être ajouté de nouveau.
        if (alreadyInLibrary) {
            addForm.classList.add("d-none");
            alreadyAddedMessage.classList.remove("d-none");
        } else {
            populateAddForm(movie);

            addForm.classList.remove("d-none");
            alreadyAddedMessage.classList.add("d-none");
        }

        modal.show();
    }

    // Le clic sur une carte ouvre la modale.
    resultsGrid.addEventListener("click", event => {
        const card = event.target.closest(".movie-search-item");

        if (!card) {
            return;
        }

        openMovieModal(card);
    });

    // Permet aussi d'ouvrir une carte au clavier.
    resultsGrid.addEventListener("keydown", event => {
        const card = event.target.closest(".movie-search-item");

        if (
            !card ||
            (event.key !== "Enter" && event.key !== " ")
        ) {
            return;
        }

        event.preventDefault();
        openMovieModal(card);
    });

    // Évite les doubles clics sur le bouton pendant l'envoi.
    addForm.addEventListener("submit", () => {
        addButton.disabled = true;
        addButton.textContent = "Ajout en cours...";
    });
});