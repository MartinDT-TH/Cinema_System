using Cinema.Models.ViewModels;
using Cinema.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Cinema.DataAccess.Repository.IRepository;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public DetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int ? MovieID)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MovieDetailVM? Movie = new MovieDetailVM()
            {
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID)

            };
            ViewData["MovieID"] = MovieID; // Store MovieID using ViewData
            ViewData["UserId"] = userId;
            return View(Movie);
        }

        //C1
        public async Task<IActionResult> Details(int MovieID, string? targetDate = null, string? targetCity = null, string? targetTime = null)
        {
            // ✅ Get User ID from Claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ✅ Fetch Movie Details
            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            // ✅ Fetch Showtimes for the selected movie
            var showTimes = await _unitOfWork.showTime.GetAllAsync(
                u => u.MovieID == MovieID, includeProperties: "Cinema,Room"
            );

            // ✅ Fetch Showtime Seats
            var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
                includeProperties: "Seat"
            );

            // ✅ Get available cities
            var availableCities = showTimes
                .Select(show => show.Cinema.CinemaCity)
                .Distinct()
                .Select(city => new
                {
                    City = city,
                    Selected = (!string.IsNullOrEmpty(targetCity) && city.Equals(targetCity, StringComparison.OrdinalIgnoreCase))
                })
                .ToList();

            // ✅ Get available dates
            var availableDates = showTimes
                .Select(show => show.ShowDates)
                .Distinct()
                .Select(date => new
                {
                    Date = date,
                    Selected = (!string.IsNullOrEmpty(targetDate) && date == targetDate)
                })
                .ToList();

            // ✅ Get all showtimes (No filtering here, let JS filter)
            var showtimeList = showTimes.Select(show => new
            {
                date = show.ShowDates,
                CinemaName = show.Cinema.Name,
                CinemaId = show.CinemaID,
                CinemaAddress = show.Cinema.Address,
                City = show.Cinema.CinemaCity,
                RoomId = show.Room.RoomID,
                RoomName = show.Room.RoomNumber,
                Showtime = show.ShowTimes,

                Tickets = showtimeSeats
                    .Where(seat => seat.ShowtimeID == show.ShowTimeID)
                    .Select(seat => new
                    {
                        SeatId = seat.SeatID,
                        SeatNumber = seat.Seat.SeatName,

                        SeatType = seat.SeatType.ToString(),

                        Price = seat.Price
                    })
                    .ToList()

            }).ToList();


            // ✅ Get Food Items (No filtering)
            var foodItemsList = await _unitOfWork.Product.GetAllAsync();
            //var selectedFoodItems = foodItems
            //    .Take(1) // Dummy selection logic
            //    .Select(food => new
            //    {
            //        FoodId = food.ProductID,
            //        FoodName = food.Name,
            //        Quantity = 2,
            //        Price = food.Price
            //    })
            //    .ToList();

            // ✅ Calculate Total Price
            //double totalPrice = showtimeList.Sum(st => st.Tickets.Sum(t => t.Price)) + selectedFoodItems.Sum(f => f.Price * f.Quantity);

            // ✅ Return structured response
            var result = new
            {
                UserId = userId,
                MovieId = movie.MovieID,
                MovieName = movie.Title,
                AvailableDates = availableDates, // Now an array of { Date, Selected }
                SelectedDate = targetDate, // Can be removed if frontend uses `AvailableDates`
                AvailableCities = availableCities, // Now an array of { City, Selected }
                City = targetCity, // Can be removed if frontend uses `AvailableCities`
                Showtimes = showtimeList,
                FoodItems = foodItemsList,
                Selected = false,
                TotalPrice = 0
            };

            return Json(new { message = "Success", data = result });
        }



        //C2
        //    public async Task<IActionResult> Details(int MovieID, string? targetDate = null , string? targetCity = null, string? targetTime = null)
        //    {
        //        // ✅ Get User ID from Claims
        //        var claimsIdentity = (ClaimsIdentity)User.Identity;
        //        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        // ✅ Fetch Movie Details
        //        var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID);
        //        if (movie == null)
        //        {
        //            return NotFound("Movie not found.");
        //        }

        //        // ✅ Fetch Showtimes for the selected movie
        //        var showTimes = await _unitOfWork.showTime.GetAllAsync(
        //            u => u.MovieID == MovieID, includeProperties: "Cinema,Room"
        //        );

        //        // ✅ Fetch Showtime Seats
        //        var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
        //            includeProperties: "Seat"
        //        );

        //        // ✅ Filter showtimes based on user input
        //        var filteredShowtimes = showTimes
        //            .Where(show =>
        //                (string.IsNullOrEmpty(targetDate) || show.ShowDates == targetDate) &&
        //                (string.IsNullOrEmpty(targetCity) ||
        //                 string.Equals(show?.Cinema?.CinemaCity?.Trim(), targetCity.Trim(), StringComparison.OrdinalIgnoreCase)) &&
        //                (string.IsNullOrEmpty(targetTime) || show?.ShowTimes == targetTime)
        //            )
        //            .ToList();

        //        if (!filteredShowtimes.Any())
        //        {
        //            return NotFound("No matching showtime found.");
        //        }

        //        // ✅ Create a list of cinema, room, and showtime combinations
        //        var showtimeList = filteredShowtimes.Select(show => new
        //        {
        //            CinemaName = show.Cinema.Name,
        //            RoomId = show.Room.RoomID,
        //            RoomName = show.Room.RoomNumber, // or RoomName if available
        //            Showtime = show.ShowTimes,
        //            Tickets = showtimeSeats
        //                .Where(seat => seat.ShowtimeID == show.ShowTimeID)
        //                .Select(seat => new
        //                {
        //                    SeatId = seat.SeatID,
        //                    SeatNumber = seat.Seat.SeatName,
        //                    SeatType = seat.SeatType.ToString(),
        //                    Price = seat.Price
        //                })
        //                .ToList()
        //        }).ToList();

        //        // ✅ Get Food Items
        //        var foodItems = await _unitOfWork.Product.GetAllAsync(); // Assuming you have a Food entity
        //        var selectedFoodItems = foodItems
        //            .Take(1) // Dummy selection logic, modify as needed
        //            .Select(food => new
        //            {
        //                FoodId = food.ProductID,
        //                FoodName = food.Name,
        //                Quantity = 2, // Default quantity for example
        //                Price = food.Price
        //            })
        //            .ToList();

        //        // ✅ Calculate Total Price (Seats + Food)
        //        double totalPrice = showtimeList.Sum(st => st.Tickets.Sum(t => t.Price)) + selectedFoodItems.Sum(f => f.Price * f.Quantity);

        //        // Assume showtimeList is defined earlier, for example:


        //        var model = new MovieDetailVM
        //        {
        //            Movie = new Movie
        //            {
        //                MovieID = movie.MovieID,
        //                Title = movie.Title,
        //                MovieImage = movie.MovieImage,
        //                Genre = movie.Genre,
        //                Duration = movie.Duration,
        //                ReleaseDate = movie.ReleaseDate,
        //                Synopsis = movie.Synopsis,
        //                TrailerLink = movie.TrailerLink
        //            },
        //            // Build the ShowDates list – here we create one entry for the selected date.
        //            ShowDates = new List<ShowDateVM>
        //{
        //    new ShowDateVM
        //    {
        //        ShowDate = targetDate, // the selected date
        //        Cities = new List<CityVM>
        //        {
        //            new CityVM
        //            {
        //                CityName = targetCity,
        //                Cinemas = showtimeList.Select(show => new TimeScheduleVM
        //                {
        //                    CinemaID = show.RoomId, // or show.CinemaID if available
        //                    CinemaName = show.CinemaName,
        //                    ShowTimes = new List<string> { show.Showtime },
        //                    // Build the dictionary: Key is the showtime, Value is the mapped tickets list
        //                    SeatsByShowtime = new Dictionary<string, List<CinemaSeatVM>>
        //                    {
        //                        {
        //                            show.Showtime,
        //                            show.Tickets.Select(ticket => new CinemaSeatVM
        //                            {
        //                                SeatId = ticket.SeatId,
        //                                SeatName = ticket.SeatNumber,
        //                                // For simplicity, using the first character of the seat number as the Row.
        //                                Row = ticket.SeatNumber.Substring(0, 1),
        //                                // Parse the string value into the TicketType enum.
        //                                SeatType = (TicketType)Enum.Parse(typeof(TicketType), ticket.SeatType),
        //                                // Set the Price dynamically from the ticket's price.
        //                                Price = ticket.Price,
        //                                // Set status as available.
        //                                Status = ShowtimeSeatStatus.Available
        //                            }).ToList()
        //                        }
        //                    }
        //                }).ToList()
        //            }
        //        }
        //    }
        //},
        //            // Initialize shopping cart items as empty
        //            SelectedSeats = new List<TicketSelectionVM>(),
        //            SelectedFoodItems = new List<FoodSelectionVM>(),
        //            TotalPrice = 0
        //        };

        //        return View(model);



        //    }


        [HttpPost]
        public IActionResult  Order(int MovieID, int CinemaID, string ShowDate, string ShowTime)
        {
            // 1. Validate inputs
            // 2. Process booking (e.g., save to database)
            // 3. Redirect to confirmation page

            return RedirectToAction("Confirmation", new
            {
                MovieID = MovieID,
                CinemaID = CinemaID,
                ShowDate = ShowDate,
                ShowTime = ShowTime
            });
        }

        public IActionResult Confirmation(int MovieID, int CinemaID, string ShowDate, string ShowTime)
        {
            var bookingDetails = new
            {
                MovieID = MovieID,
                CinemaID = CinemaID,
                ShowDate = ShowDate,
                ShowTime = ShowTime
            };

            return View(bookingDetails);
        }
        [HttpPost]
        public IActionResult GenerateTicket(string ticketId)
        {
            string validationUrl = Url.Action("ValidAuthentication", "Home", new { ticketId = ticketId }, Request.Scheme);
            //string validationUrl = "https://localhost:7251/Home/ValidAuthentication";

            //string validationUrl = "https://www.facebook.com";
            //string validationUrl = "https://httpbin.org/anything";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator codeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = codeGenerator.CreateQrCode(validationUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCoder = new QRCode(qRCodeData);

                using (Bitmap bitMap = qRCoder.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            ViewBag.TicketId = ticketId;
            return View("TestQR");
        }
        //https://localhost:7115/Guest/Detail/GenerateTicket to test QRcode
        // enter staff login to use scan camera Staff account in DBInitializer username is account,,Staff@123 password
        public IActionResult TestQR()
        {
            return View();
        }
        public IActionResult ValidAuthentication(string ticketId)
        {
            if (ValidateTicket(ticketId))
            {
                ViewBag.Message = "Ticket is valid!";
            }
            else
            {
                ViewBag.Message = "Invalid or expired ticket.";
            }

            ViewBag.TicketId = ticketId;
            return View();
        }

        private bool ValidateTicket(string ticketId)
        {
            // Mock validation logic; replace with actual validation (e.g., check database)
            return !string.IsNullOrEmpty(ticketId) && ticketId.StartsWith("TICKET");
        }


    }
}
