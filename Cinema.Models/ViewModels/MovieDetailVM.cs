using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
    public class MovieDetailVM
    {
        public Movie Movie { get; set; }
        public List<ShowDateVM> ShowDates { get; set; }

        // Shopping Cart Items
        public List<TicketSelectionVM> SelectedSeats { get; set; } = new List<TicketSelectionVM>();
        public List<FoodSelectionVM> SelectedFoodItems { get; set; } = new List<FoodSelectionVM>();

        public double TotalPrice { get; set; } // Automatically calculated
    }
}
