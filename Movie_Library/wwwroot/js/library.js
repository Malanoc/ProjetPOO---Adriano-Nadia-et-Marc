document.addEventListener("DOMContentLoaded", () => {

    const sortSelect = document.getElementById("sort");
    const statusSelect = document.getElementById("status");
    const movieGrid = document.getElementById("movie-grid");

    if (!sortSelect || !statusSelect || !movieGrid) {
        return;
    }

    function getMovieCards() {
        return Array.from(movieGrid.querySelectorAll(".movie-item"));
    }

    function filterMovies() {
        const selectedStatus = statusSelect.value;

        getMovieCards().forEach(card => {
            const movieStatus = card.dataset.status;

            if (selectedStatus === "all" || movieStatus === selectedStatus) {
                card.style.display = "";
            } else {
                card.style.display = "none";
            }
        });
    }

    function sortMovies() {
        const cards = getMovieCards();

        cards.sort((a, b) => {
            const titleA = a.dataset.title.toLowerCase();
            const titleB = b.dataset.title.toLowerCase();

            if (sortSelect.value === "title") {
                return titleA.localeCompare(titleB);
            }

            return 0;
        });

        cards.forEach(card => movieGrid.appendChild(card));
    }

    sortSelect.addEventListener("change", () => {
        sortMovies();
        filterMovies();
    });

    statusSelect.addEventListener("change", filterMovies);

    sortMovies();
    filterMovies();
});