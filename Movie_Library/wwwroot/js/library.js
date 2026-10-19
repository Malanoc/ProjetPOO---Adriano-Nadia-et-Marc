document.addEventListener("DOMContentLoaded", () => {
    const sortSelect = document.getElementById("sort");
    const statusSelect = document.getElementById("status");
    const movieGrid = document.getElementById("movie-grid");

    const movieModalElement = document.getElementById("movie-details-modal");
    const movieCount = document.getElementById("library-count");
    const emptyState = document.getElementById("library-empty-state");

    // Le script peut être chargé sur une page sans bibliothèque.
    if (!movieGrid) {
        return;
    }

    // Ordre utilisé pour le tri des statuts.
    const statusOrder = {
        notseen: 0,
        inprogress: 1,
        seen: 2
    };

    // Correspondance entre les statuts et leur libellé affiché.
    const statusLabels = {
        NotSeen: "Non vu",
        InProgress: "En cours",
        Seen: "Vu"
    };

    let selectedCard = null;

    // Initialise Bootstrap Modal si Bootstrap est disponible.
    const modal = movieModalElement && window.bootstrap
        ? bootstrap.Modal.getOrCreateInstance(movieModalElement)
        : null;


    // =================
    // TRI ET FILTRAGE
    // =================

    // Récupère les cartes présentes dans la grille.
    function getMovieCards() {
        return Array.from(
            movieGrid.querySelectorAll(".movie-item")
        );
    }

    // Trie les films selon l'option sélectionnée.
    function sortMovies() {
        if (!sortSelect) {
            return;
        }

        const cards = getMovieCards();
        const selectedSort = sortSelect.value;

        cards.sort((a, b) => {
            const titleA = (a.dataset.title || "").trim();
            const titleB = (b.dataset.title || "").trim();

            // Tri alphabétique par titre.
            if (selectedSort === "title") {
                return titleA.localeCompare(
                    titleB,
                    "fr",
                    { sensitivity: "base" }
                );
            }

            // Tri par note TMDB décroissante.
            if (selectedSort === "rating") {
                const ratingA = Number.parseFloat(
                    a.dataset.rating || "0"
                );

                const ratingB = Number.parseFloat(
                    b.dataset.rating || "0"
                );

                return ratingB - ratingA ||
                    titleA.localeCompare(titleB, "fr");
            }

            // Tri par statut, puis par titre.
            if (selectedSort === "status") {
                const statusA =
                    statusOrder[a.dataset.status] ?? 99;

                const statusB =
                    statusOrder[b.dataset.status] ?? 99;

                return statusA - statusB ||
                    titleA.localeCompare(titleB, "fr");
            }

            return 0;
        });

        // Réinsère les cartes dans l'ordre calculé.
        cards.forEach(card => {
            movieGrid.appendChild(card);
        });
    }

    // Affiche les films correspondant au filtre de statut.
    function filterMovies() {
        if (!statusSelect) {
            return;
        }

        const selectedStatus = statusSelect.value;

        getMovieCards().forEach(card => {
            const shouldShow =
                selectedStatus === "all" ||
                card.dataset.status === selectedStatus;

            card.style.display = shouldShow ? "" : "none";
        });
    }

    // Événement de changement du tri.
    if (sortSelect) {
        sortSelect.addEventListener("change", () => {
            sortMovies();
            filterMovies();
        });
    }

    // Événement de changement du filtre.
    if (statusSelect) {
        statusSelect.addEventListener("change", filterMovies);
    }


    // ======================
    // MESSAGES DE LA MODALE
    // ======================

    function showModalMessage(message, type = "success") {
        const messageElement = document.getElementById(
            "movie-modal-message"
        );

        messageElement.textContent = message;
        messageElement.className = `alert alert-${type}`;
    }

    function hideModalMessage() {
        const messageElement = document.getElementById(
            "movie-modal-message"
        );

        messageElement.textContent = "";
        messageElement.className = "alert d-none";
    }


    // =========================
    // OUVERTURE DE LA MODALE
    // =========================

    function openMovieModal(card) {
        if (!modal || !card) {
            return;
        }

        selectedCard = card;
        hideModalMessage();

        const title = card.dataset.title || "";
        const synopsis = card.dataset.synopsis || "";
        const poster = card.dataset.poster || "";
        const status = card.dataset.status || "notseen";

        // Informations principales.
        document.getElementById("movie-modal-title").textContent =
            title;

        document.getElementById("movie-modal-synopsis").textContent =
            synopsis || "Aucun synopsis disponible.";

        const tmdbRating = Number.parseFloat(
            card.dataset.rating || "0"
        );

        document.getElementById("movie-modal-tmdb-rating").textContent =
            `${tmdbRating.toFixed(1)} / 10`;

        // Statut de visionnage.
        document.getElementById("movie-modal-status").value =
            status === "inprogress"
                ? "InProgress"
                : status === "seen"
                    ? "Seen"
                    : "NotSeen";

        // Note et commentaire personnels.
        document.getElementById("movie-modal-personal-rating").value =
            card.dataset.personalRating || "0";

        document.getElementById("movie-modal-personal-note").value =
            card.dataset.personalNote || "";

        // Affiche le poster ou le remplacement correspondant.
        const posterImage = document.getElementById(
            "movie-modal-poster"
        );

        const noPoster = document.getElementById(
            "movie-modal-no-poster"
        );

        if (poster) {
            posterImage.src = poster;
            posterImage.alt = title;

            posterImage.classList.remove("d-none");
            noPoster.classList.add("d-none");
        } else {
            posterImage.removeAttribute("src");
            posterImage.classList.add("d-none");
            noPoster.classList.remove("d-none");
        }

        modal.show();
    }

    // Un clic sur une carte ouvre la modale.
    movieGrid.addEventListener("click", event => {
        const card = event.target.closest(".movie-item");

        if (card) {
            openMovieModal(card);
        }
    });

    // L'ouverture est également possible au clavier.
    movieGrid.addEventListener("keydown", event => {
        const card = event.target.closest(".movie-item");

        if (
            card &&
            (event.key === "Enter" || event.key === " ")
        ) {
            event.preventDefault();
            openMovieModal(card);
        }
    });


    // ==========================
    // ENVOI DES REQUÊTES POST
    // ==========================

    async function postForm(url, values) {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        );

        const formData = new URLSearchParams();

        Object.entries(values).forEach(([key, value]) => {
            formData.append(key, value ?? "");
        });

        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type":
                    "application/x-www-form-urlencoded; charset=UTF-8",

                "RequestVerificationToken": token?.value || ""
            },
            body: formData.toString()
        });

        if (!response.ok) {
            throw new Error(
                `La requête a échoué (HTTP ${response.status}).`
            );
        }

        return await response.json();
    }


    // ==============================
    // NOTE PERSONNELLE SUR LA CARTE
    // ==============================

    function updatePersonalRatingDisplay(card, rating) {
        const movieInfo = card.querySelector(".movie-info");

        if (!movieInfo) {
            return;
        }

        let ratingLine = card.querySelector(
            ".movie-personal-rating"
        );

        const personalRating = Number(rating);

        // Aucun élément à afficher si le film n'a pas de note.
        if (personalRating <= 0) {
            if (ratingLine) {
                ratingLine.remove();
            }

            return;
        }

        // Crée la ligne lors de la première note attribuée.
        if (!ratingLine) {
            ratingLine = document.createElement("p");
            ratingLine.className = "movie-personal-rating";

            movieInfo.appendChild(ratingLine);
        }

        // Met à jour le libellé et la valeur.
        ratingLine.replaceChildren(
            document.createTextNode("Note personnelle : ")
        );

        const ratingValue = document.createElement("span");

        ratingValue.textContent =
            `${personalRating.toLocaleString("fr-FR", {
                minimumFractionDigits: 1,
                maximumFractionDigits: 1
            })} / 10`;

        ratingLine.appendChild(ratingValue);
    }


    // =======================================
    // ENREGISTREMENT DU STATUT ET DES NOTES
    // =======================================

    document.getElementById("movie-modal-save")
        .addEventListener("click", async () => {
            if (!selectedCard) {
                return;
            }

            const saveButton = document.getElementById(
                "movie-modal-save"
            );

            saveButton.disabled = true;
            hideModalMessage();

            try {
                const result = await postForm(
                    movieModalElement.dataset.saveUrl,
                    {
                        id: selectedCard.dataset.movieId,

                        status: document.getElementById(
                            "movie-modal-status"
                        ).value,

                        personalRating: document.getElementById(
                            "movie-modal-personal-rating"
                        ).value,

                        personalNote: document.getElementById(
                            "movie-modal-personal-note"
                        ).value
                    }
                );

                if (!result.success) {
                    throw new Error(
                        result.message ||
                        "Impossible d'enregistrer les modifications."
                    );
                }

                // Met à jour les données de la carte.
                selectedCard.dataset.status =
                    result.status.toLowerCase();

                selectedCard.dataset.personalRating =
                    String(result.personalRating);

                selectedCard.dataset.personalNote =
                    result.personalNote || "";

                // Actualise le statut visible.
                const statusLabel = selectedCard.querySelector(
                    ".movie-status-label"
                );

                if (statusLabel) {
                    statusLabel.textContent =
                        statusLabels[result.status] ||
                        result.status;
                }

                // Actualise ou retire la note personnelle.
                updatePersonalRatingDisplay(
                    selectedCard,
                    result.personalRating
                );

                // Réapplique le tri et le filtre.
                sortMovies();
                filterMovies();

                showModalMessage(result.message);
            } catch (error) {
                showModalMessage(error.message, "danger");
            } finally {
                saveButton.disabled = false;
            }
        });


    // =====================
    // SUPPRESSION DU FILM
    // =====================

    document.getElementById("movie-modal-delete")
        .addEventListener("click", async () => {
            if (!selectedCard) {
                return;
            }

            const title = selectedCard.dataset.title || "ce film";

            // Confirmation avant suppression.
            if (!window.confirm(
                `Voulez-vous vraiment supprimer « ${title} » de votre bibliothèque ?`
            )) {
                return;
            }

            const deleteButton = document.getElementById(
                "movie-modal-delete"
            );

            deleteButton.disabled = true;

            try {
                const result = await postForm(
                    movieModalElement.dataset.deleteUrl,
                    {
                        id: selectedCard.dataset.movieId
                    }
                );

                if (!result.success) {
                    throw new Error(
                        result.message || "La suppression a échoué."
                    );
                }

                // Retire la carte de la grille.
                selectedCard.remove();
                selectedCard = null;

                const remainingMovies = getMovieCards().length;

                // Met à jour le compteur de films.
                if (movieCount) {
                    movieCount.textContent =
                        `${remainingMovies} film${remainingMovies > 1 ? "s" : ""}`;
                }

                // Affiche l'état vide si nécessaire.
                if (remainingMovies === 0) {
                    movieGrid.classList.add("d-none");

                    if (emptyState) {
                        emptyState.classList.remove("d-none");
                    }
                }

                // Applique de nouveau le tri et le filtre.
                sortMovies();
                filterMovies();

                // Ferme la modale après la suppression.
                if (modal) {
                    modal.hide();
                }
            } catch (error) {
                showModalMessage(error.message, "danger");
            } finally {
                deleteButton.disabled = false;
            }
        });


    // ===================
    // INITIALISATION
    // ===================

    sortMovies();
    filterMovies();
});