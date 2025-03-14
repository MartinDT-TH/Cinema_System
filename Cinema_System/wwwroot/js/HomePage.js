

  

$(document).ready(function () {
    // Load movies for all categories on page load.
    loadMovies(1, 1, 1);

    // Event Listener for Pagination Clicks (delegated since buttons are generated dynamically)
    $(document).on("click", ".page-number, .page-prev, .page-next", function (e) {
        e.preventDefault();
        let page = parseInt($(this).attr("data-page"));
        let category = $(this).attr("data-category");

        // Prevent clicking if the button is disabled.
        if (!$(this).parent().hasClass("disabled")) {
            updateMovies(category, page);
        }
    });
});

function loadMovies(showingPage, upcomingPage, couponPage) {
    // Use the provided page numbers or read from hidden elements if null.
    showingPage = showingPage || parseInt($("#showing-page").attr("data-page"));
    upcomingPage = upcomingPage || parseInt($("#upcoming-page").attr("data-page"));
    couponPage = couponPage || parseInt($("#coupon-page").attr("data-page"));

    $.ajax({
        url: `/Guest/Home/GetMovies?Showingpage=${showingPage}&Upcommingpage=${upcomingPage}&CouponPage=${couponPage}`,
        type: "GET",
        success: function (response) {
            if (response.message === "Success") {
                let data = response.data || {};

                //let showingId = data.showingMovies.movieID;
                //let upComingId = data.upcommingMovies.movieID;
                //let couponId = data.couponMovies.couponID;

                let CountShowingMovies = data.showingMoviesCount;
                let CountUpcomingMovies = data.upcommingMoviesCount;
                let CountCouponMovies = data.couponCount;
                let pageSize = data.pageSize;

                let totalPagesShowing = Math.ceil(CountShowingMovies / pageSize);
                let totalPagesUpcoming = Math.ceil(CountUpcomingMovies / pageSize);
                let totalPagesCoupons = Math.ceil(CountCouponMovies / pageSize);

                // Update Movie Sections for each category.
                updateMovieSection("showing", data.showingMovies, totalPagesShowing, showingPage);
                updateMovieSection("upcoming", data.upcommingMovies, totalPagesUpcoming, upcomingPage);
                updateMovieSection("coupon", data.couponMovies, totalPagesCoupons, couponPage);
            } else {
                alert("Failed to load movies.");
            }
        },
        error: function () {
            alert("Error fetching data from API.");
        }
    });
}

function updateMovieSection(category, movies, totalPages, currentPage) {
    let container = $(`#${category}-movies`);
    let paginationContainer = $(`#${category}-pagination`);

    // Clear old data.
    container.html("");
    paginationContainer.html("");

    // Add section heading.
    container.append(`
        <h5 class="text-center">
            ${category === "showing" ? "Now Showing" : category === "upcoming" ? "Upcoming Movies" : "Coupons"}
        </h5>
    `);

    // Create a row to hold cards.
    let row = $('<div class="row pb-3"></div>');

    // Append movies or coupons
    movies.forEach(movie => {
        let card = "";

        if (category === "coupon") {
            // Coupon Card
            card = `
                <div class="col-lg-3 col-sm-6">
                    <div class="row p-2">
                        <div class="col-12 p-1">
                           
                               <img src="${movie.CouponImage ? movie.CouponImage : 'https://placehold.co/500x300/png'}"   class="card-img-top img-fluid rounded"  />
                           
                        </div>
                    </div>
                </div>
            `;
        } else {
            // Movie Card
            card = `
                <div class="col-lg-3 col-sm-6">
                    <div class="row p-2">
                        <div class="col-12 p-1">
                            <div class="card rounded">
                                <img src="${movie.movieImage ? movie.movieImage : 'https://placehold.co/500x700/png'}" 
                                    class="card-img-top img-fluid rounded" 
                                    alt="${movie.title}" />
                            </div>
                        </div>
                        <div class="col-12">
                            <p class="pt-2 h5 text-dark opacity-75 text-uppercase text-center mb-3">${movie.title}</p>
                        </div>
                        <div class="col-6">
                            <a href="${movie.trailerLink}" class="text-dark" target="_blank" style="font-size:20px">See Trailer</a>
                        </div>
                        <div class="col-6">
                        <a href="/Guest/Detail/Index?MovieID=${movie.movieID}" class="btn btn-outline-warning">
                          ${movie.isUpcomingMovie ? "Detail" : "Book Ticket"}
</a>                    </a>

                        </div>
                    </div>
                </div>
            `;
        }

        row.append(card);
    });

    // Append the row to the container.
    container.append(row);

    // Update Pagination for this category.
    updatePagination(category, totalPages, currentPage);
}

function updatePagination(category, totalPages, currentPage) {
    let paginationHtml = `<nav><ul class="pagination justify-content-center">`;

    // Previous button.
    //paginationHtml += `
    //    <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
    //        <a class="page-link page-prev" href="#" data-category="${category}" data-page="${currentPage - 1}">Previous</a>
    //    </li>
    //`;

    // Page numbers.
    for (let i = 1; i <= totalPages; i++) {
        let activeClass = i === currentPage ? "active" : "";
        paginationHtml += `
            <li class="page-item ${activeClass}">
                <a class="page-link page-number" href="#" data-category="${category}" data-page="${i}">${i}</a>
            </li>
        `;
    }

    //// Next button.
    //paginationHtml += `
    //    <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
    //        <a class="page-link page-next" href="#" data-category="${category}" data-page="${currentPage + 1}">Next</a>
    //    </li>
    //`;
    paginationHtml += `</ul></nav>`;

    // Update the pagination container for the category.
    $(`#${category}-pagination`).html(paginationHtml);

    // Update the hidden element that tracks the current page.
    $(`#${category}-page`).attr("data-page", currentPage);
}

function updateMovies(category, page) {
    // Call loadMovies() with the proper parameter based on category.
    if (category === "showing") loadMovies(page, null, null);
    if (category === "upcoming") loadMovies(null, page, null);
    if (category === "coupon") loadMovies(null, null, page);
}