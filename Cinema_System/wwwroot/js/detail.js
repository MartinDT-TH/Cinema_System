$(document).ready(function () {
    // Initialize core variables
    const movieID = $("#movieID").val();
    const IdUser = $("#targetTime").val();
    let allShowtimes = [];

    // Main AJAX request
    $.ajax({
        url: `/Guest/Detail/Details?userId=${IdUser}&MovieID=${movieID}`,
        method: "GET",
        success: function (response) {
            handleSuccessResponse(response);
        },
        error: handleErrorResponse
    });

    function handleSuccessResponse(response) {
        console.log("Response received:", response);

        if (response.message !== "Success") {
            showErrorMessage("#movieDetails", "Failed to load movie details.");
            return;
        }

        const data = response.data;
        allShowtimes = data.showtimes || [];

        initializeFilters(
            data.availableDates.map(d => d.date),
            data.availableCities.map(c => c.city)
        );
        renderFoodItems(data.foodItems);
    }

    function initializeFilters(dates, cities) {
        createDateFilters("#filterDates", dates);
        createCityFilter("#filterCities", cities);
        applyFilters(); // Initial filter application
    }

    function createDateFilters(container, dates) {
        const buttonsHtml = dates.map((date, index) => `
            <button class="btn btn-outline-warning px-3 py-2 mt-3 date-toggle ${index === 0 ? 'active' : ''}" 
                    data-value="${date}" 
                    style="width: 120px; height: 120px;">
                ${date}
            </button>
        `).join(" ");

        $(container).html(buttonsHtml)
            .find(".date-toggle").click(function () {
                $(".date-toggle").removeClass("active");
                $(this).addClass("active");
                applyFilters();
            });
    }

    function createCityFilter(container, cities) {
        const dropdownHtml = `
            <select id="citySelect" class="form-select form-select-sm mb-2">
                <option value="">All Cities</option>
                ${cities.map(c => `<option value="${c}">${c}</option>`).join("")}
            </select>`;

        $(container).html(dropdownHtml)
            .find("select").on("change", applyFilters);
    }

    function applyFilters() {
        const filters = {
            date: getActiveDateFilter(),
            city: $("#citySelect").val()
        };

        const filtered = allShowtimes.filter(show =>
            (!filters.date.length || filters.date.includes(show.date)) &&
            (!filters.city || show.city === filters.city)
        );

        updateShowtimesDisplay(filtered);
    }

    function getActiveDateFilter() {
        return $("#filterDates .date-toggle.active").map(function () {
            return $(this).data("value");
        }).get();
    }

    function updateShowtimesDisplay(showtimes) {
        const cinemaMap = showtimes.reduce((acc, show) => {
            acc[show.cinemaName] = acc[show.cinemaName] || [];
            acc[show.cinemaName].push(show);
            return acc;
        }, {});

        const displayHtml = Object.entries(cinemaMap).map(([cinema, shows]) => `
        <div class="card sci-fi-card mb-3 w-100">
            <h5 class="card-title">${cinema}</h5>
            <p>${shows[0].cinemaAddress}</p> 
            <div class="d-flex flex-wrap gap-2">
                ${shows.map(show => `
                    <button class="btn sci-fi-btn showtime-btn" 
                            data-showtime='${JSON.stringify(show)}'>
                        ${show.showtime}
                    </button>
                `).join("")}
            </div>
        </div>
    `).join("") || "<p class='mb-3 text-white'>No showtimes available</p>";

        $("#movieDetails").html(displayHtml)
            .find(".showtime-btn").click(handleShowtimeSelection);
    }


    function handleShowtimeSelection() {
        $(".showtime-btn").removeClass("btn-primary").addClass("btn-outline-primary");
        const $btn = $(this).removeClass("btn-outline-primary").addClass("btn-primary");

        const showtime = JSON.parse($btn.attr("data-showtime"));

        renderSeats(showtime.seatList);
        renderTicketList(showtime.ticketList);  // ✅ Call function to render ticket list
       
    }

    function renderTicketList(ticketList) {
        const ticketContainer = $("#ticketContainer").empty(); // Clear previous tickets

        const ticketRow = $('<div class="d-flex justify-content-center flex-wrap gap-3"></div>'); // Center align and allow wrapping

        ticketList.forEach(ticket => {
            const ticketHtml = `
            <div class="product-card d-flex flex-column align-items-center col-4 text-center">
                <img src="/images/ticketCinema.jpg" alt="${ticket.ticketType}" class="product-img">
                <div class="product-info">
                    <h4>${ticket.ticketType} Ticket</h4>
                    <span class="price" >Price: ${ticket.price.toFixed(2) } VND</span>
                    <div class="quantity d-flex align-items-center justify-content-center">
                        <button class="btn minus">-</button>
                        <span class="count mx-2" data-ticket-type="${ticket.ticketType}">${ticket.selectedQuantity}</span>
                        <button class="btn plus">+</button>
                    </div>
                </div>
            </div>
        `;
            ticketRow.append(ticketHtml);
        });

        ticketContainer.append(ticketRow);
    }



    function renderSeats(seats) {
        const seatsContainer = $("#seatsContainer")
            .empty()
            .addClass("seat-grid d-flex flex-wrap justify-content-center mx-auto col-6");

        seats.forEach(seat => {
            const seatClass = `seat ${seat.selected ? 'booked' : ''} ${seat.maintenance ? 'maintenance' : ''}`;
            const $seat = $(`<div class="${seatClass}" data-seat-id="${seat.seatId}" data-seat-type="${seat.seatType}">${seat.seatNumber}</div>`)
                .click(function () {
                    if (!$(this).hasClass("booked") && !$(this).hasClass("maintenance")) {
                        $(this).toggleClass("selected");
                        updateBookingSummary(); // Ensure booking bar updates when seat is clicked
                    }
                });

            seatsContainer.append($seat);
        });
        updateBookingSummary(); // Ensure booking bar updates when seat is clicked
    }

    // Function to render food items
    function renderFoodItems(foodItems) {
        const foodContainer = $("#foodContainer").empty(); // Clear previous food items

        foodItems.forEach(food => {
            const foodHtml = `
            <div class="col-md-4 d-flex justify-content-center food-item">
                <div class="product-card d-flex align-items-center">
                    <img src="${food.productImage || '/css/images/default-food.png'}" alt="${food.name}" class="product-img">
                    <div class="product-info">
                        <h4 class="food-name">${food.name}</h4>
                        <p>${food.description}</p>
                        <span class="price">${food.price.toFixed(2)} VND</span>
                        <div class="quantity">
                            <button class="btn minus">-</button>
                            <span class="count" data-product-id="${food.productID}">${food.countProduct}</span>
                            <button class="btn plus">+</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

            foodContainer.append(foodHtml);
        });
    }

    function updateBookingSummary() {
        let selectedTickets = {};
        $(".count").each(function () {
            let ticketType = $(this).data("ticket-type");
            let quantity = parseInt($(this).text());
            selectedTickets[ticketType] = quantity;
        });

        let selectedSeats = $(".seat.selected").map(function () {
            return {
                seatId: $(this).data("seat-id"),
                seatType: $(this).data("seat-type")
            };
        }).get();

        let selectedSeatTypes = {};
        selectedSeats.forEach(seat => {
            selectedSeatTypes[seat.seatType] = (selectedSeatTypes[seat.seatType] || 0) + 1;
        });

        let totalTickets = Object.values(selectedTickets).reduce((a, b) => a + b, 0);
        let totalSelectedSeats = selectedSeats.length;

        // **Show/Hide booking summary**
        if (totalSelectedSeats > 0) {
            $('#booking-summary').removeClass('d-none'); // Show booking bar
        } else {
            $('#booking-summary').addClass('d-none'); // Hide when no seats are selected
        }

        // **Validation: Ensure selected seats match ticket purchase**
        if (totalSelectedSeats > totalTickets) {
            alert("You have selected more seats than the tickets you purchased.");
            $(".seat.selected").last().removeClass("selected"); // Remove the last selected seat
            return;
        }

        for (let seatType in selectedSeatTypes) {
            if (selectedSeatTypes[seatType] > (selectedTickets[seatType] || 0)) {
                alert(`Seat type mismatch! You selected more ${seatType} seats than the tickets you purchased.`);
                $(".seat.selected").last().removeClass("selected"); // Remove the last selected seat
                return;
            }
        }
    }


    function handleErrorResponse() {
        showErrorMessage("#movieDetails", "Error loading movie details.");
    }

    function showErrorMessage(container, message) {
        $(container).html(`<p class="text-danger mb-3">${message}</p>`);
    }



 

});
// ĐẾM NGƯỢC 5 PHÚT GIỮ VÉ
let timeLeft = 300;
const countdown = setInterval(function () {
    timeLeft--;
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;
    $('#countdown').text(`${minutes}:${seconds < 10 ? '0' : ''}${seconds}`);
    if (timeLeft <= 0) {
        clearInterval(countdown);
        alert('Hết thời gian giữ vé!');
        location.reload();
    }
}, 1000);




$(document).ready(function () {
    // Function to update selected food items in summary
    function updateSelectedFood() {
        // Collect selected food items
        let selectedFoodText = $(".count[data-product-id]").map(function () {
            let quantity = parseInt($(this).text());
            let foodName = $(this).closest('.food-item').find('.food-name').text();
            return quantity > 0 ? `${quantity}x ${foodName}` : null;
        }).get().join(", ") || "None";

        // Collect selected tickets
        let selectedTicketsText = $(".count[data-ticket-type]").map(function () {
            let quantity = parseInt($(this).text());
            let ticketType = $(this).data("ticket-type");
            return quantity > 0 ? `${quantity}x ${ticketType}` : null;
        }).get().join(", ") || "None";

        // Combine food and tickets into a single summary
        let combinedSummary = [selectedTicketsText, selectedFoodText].filter(Boolean).join(", ");

        // Update the display
        $("#displayFood").text(combinedSummary);
    }

    // Event listener for plus button
    $(document).on("click", ".plus", function () {
        let countSpan = $(this).siblings(".count");
        let currentCount = parseInt(countSpan.text());
        countSpan.text(currentCount + 1);
        updateSelectedFood(); // Call the function to update the food summary
        calculateTotalPrice();
    });

    // Event listener for minus button
    $(document).on("click", ".minus", function () {
        let countSpan = $(this).siblings(".count");
        let currentCount = parseInt(countSpan.text());
        if (currentCount > 0) {
            countSpan.text(currentCount - 1);
            updateSelectedFood(); // Call the function to update the food summary
            calculateTotalPrice();
        }
    });

    function calculateTotalPrice() {
        let totalPrice = 0;

        // Calculate total for food items
        $(".count[data-product-id]").each(function () {
            let quantity = parseInt($(this).text());
            console.log(quantity);
            let price = parseFloat($(this).closest('.food-item').find('.price').text().replace(' VND', ''));
            console.log(price);
            totalPrice += quantity * price;
            console.log(totalPrice);
        });

        // Calculate total for tickets
        $(".count[data-ticket-type]").each(function () {
            let quantity = parseInt($(this).text());
            console.log("Quantity:", quantity);

            // Extract the price text and remove non-numeric characters (e.g., "USD", commas)
            let priceText = $(this).closest('.product-card').find('.price').text();
            let price = parseFloat(priceText.replace(/[^0-9.]/g, '')); // Remove all non-numeric characters
            console.log("Price Text:", priceText);
            console.log("Parsed Price:", price);

            // Add to the total price
            if (!isNaN(price)) {
                totalPrice += quantity * price;
            }
            console.log("Total Price:", totalPrice);
        });

        // Update the total price display
        $("#displayTotalPrice").text(totalPrice.toFixed(2) + " VND");
    }
});