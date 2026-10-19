document.addEventListener("DOMContentLoaded", () => {
    const sortSelect = document.getElementById("sort");
    const statusSelect = document.getElementById("status");
    const movieGrid = document.getElementById("movie-grid");

    const movieModalElement = document.getElementById("movie-details-modal");
    const movieCount = document.getElementById("library-count");
    const emptyState = document.getElementById("library-empty-state");

    // Le script peut aussi être chargé sur une page sans bibliothèque.
    if (!movieGrid) {
        return;
    }

    const statusOrder = {
        notseen: 0,
        inprogress: 1,
        seen: 2
    };

    const statusLabels = {
        NotSeen: "Non vu",
        InProgress: "En cours",
        Seen: "Vu"
    };

    let selectedCard = null;

    const modal = movieModalElement && window.bootstrap
        ? bootstrap.Modal.getOrCreateInstance(movieModalElement)
        : null;

    // -----------------------------------------------------
    // TRI ET FILTRAGE
    // -----------------------------------------------------

    function getMovieCards() {
        return Array.from(
            movieGrid.querySelectorAll(".movie-item")
        );
    }

    function sortMovies() {
        if (!sortSelect) {
            return;
        }

        const cards = getMovieCards();
        const selectedSort = sortSelect.value;

        cards.sort((a, b) => {
            const titleA = (a.dataset.title || "").trim();
            const titleB = (b.dataset.title || "").trim();

            if (selectedSort === "title") {
                return titleA.localeCompare(
                    titleB,
                    "fr",
                    { sensitivity: "base" }
                );
            }

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

            if (selectedSort === "status") {
                const statusA = statusOrder[a.dataset.status] ?? 99;
                const statusB = statusOrder[b.dataset.status] ?? 99;

                return statusA - statusB ||
                    titleA.localeCompare(titleB, "fr");
            }

            return 0;
        });

        cards.forEach(card => movieGrid.appendChild(card));
    }

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

    if (sortSelect) {
        sortSelect.addEventListener("change", () => {
            sortMovies();
            filterMovies();
        });
    }

    if (statusSelect) {
        statusSelect.addEventListener("change", filterMovies);
    }

    // -----------------------------------------------------
    // MODALE : AFFICHAGE DES DÉTAILS
    // -----------------------------------------------------

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

        document.getElementById("movie-modal-title").textContent = title;
        document.getElementById("movie-modal-synopsis").textContent =
            synopsis || "Aucun synopsis disponible.";

        document.getElementById("movie-modal-tmdb-rating").textContent =
            `${Number.parseFloat(card.dataset.rating || "0").toFixed(1)} / 10`;

        document.getElementById("movie-modal-status").value =
            status === "inprogress" ? "InProgress" :
                status === "seen" ? "Seen" : "NotSeen";

        document.getElementById("movie-modal-personal-rating").value =
            card.dataset.personalRating || "0";

        document.getElementById("movie-modal-personal-note").value =
            card.dataset.personalNote || "";

        const posterImage = document.getElementById("movie-modal-poster");
        const noPoster = document.getElementById("movie-modal-no-poster");

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

    // Cliquer sur une carte ouvre la modale.
    movieGrid.addEventListener("click", event => {
        const card = event.target.closest(".movie-item");

        if (card) {
            openMovieModal(card);
        }
    });

    // Permet aussi d'ouvrir une carte au clavier.
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

    // -----------------------------------------------------
    // REQUÊTES POST AVEC PROTECTION ANTIFORGERY
    // -----------------------------------------------------

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

    // -----------------------------------------------------
    // ENREGISTREMENT DU STATUT ET DES NOTES
    // -----------------------------------------------------

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
                        result.message || "Impossible d'enregistrer les modifications."
                    );
                }

                selectedCard.dataset.status =
                    result.status.toLowerCase();

                selectedCard.dataset.personalRating =
                    String(result.personalRating);

                selectedCard.dataset.personalNote =
                    result.personalNote;

                const statusLabel =
                    selectedCard.querySelector(".movie-status-label");

                if (statusLabel) {
                    statusLabel.textContent =
                        statusLabels[result.status] || result.status;
                }

                sortMovies();
                filterMovies();

                showModalMessage(result.message);
            } catch (error) {
                showModalMessage(error.message, "danger");
            } finally {
                saveButton.disabled = false;
            }
        });

    // -----------------------------------------------------
    // SUPPRESSION DU FILM
    // -----------------------------------------------------

    document.getElementById("movie-modal-delete")
        .addEventListener("click", async () => {
            if (!selectedCard) {
                return;
            }

            const title = selectedCard.dataset.title || "ce film";

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

                selectedCard.remove();
                selectedCard = null;

                const remainingMovies = getMovieCards().length;

                if (movieCount) {
                    movieCount.textContent =
                        `${remainingMovies} film${remainingMovies > 1 ? "s" : ""}`;
                }

                if (remainingMovies === 0) {
                    movieGrid.classList.add("d-none");

                    if (emptyState) {
                        emptyState.classList.remove("d-none");
                    }
                }

                sortMovies();
                filterMovies();

                modal.hide();
            } catch (error) {
                showModalMessage(error.message, "danger");
            } finally {
                deleteButton.disabled = false;
            }
        });

    // Applique le tri et le filtre initiaux.
    sortMovies();
    filterMovies();
});