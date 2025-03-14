using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema.Models
{
    public class ShowTime
    {
        [Key]
        public int ShowTimeID { get; set; }

        [Required]
        //public DateTime ? ShowDate { get; set; } // SQL `DATE` maps to `DateTime`
        public string ShowDates { get; set; }
        [Required]
        //public TimeSpan ShowTimes { get; set; } // SQL `TIME` maps to `TimeSpan`
        public string ShowTimes { get; set; }

        

        public int CinemaID { get; set; }
        [ForeignKey(nameof(CinemaID))]
        [ValidateNever]
        public Theater ? Cinema { get; set; }


        public int RoomID { get; set; }
        [ForeignKey(nameof(RoomID))]
        [ValidateNever]
        public Room ? Room { get; set; }


        public int MovieID { get; set; }
        [ForeignKey(nameof(MovieID))]
        [ValidateNever]
        public Movie ? Movie { get; set; }

        // Navigation property for seats
        public virtual ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();



//        This is a navigation property that tells EF that one ShowTime can have multiple ShowtimeSeat records.
//Since ShowtimeSeat already has ShowtimeID as a foreign key, EF automatically understands this as a one-to-many relationship.
//Now, from ShowTime, you can access all seats that belong to it using ShowtimeSeats.

        // Computed property to get the available ticket quantity
        [NotMapped]
        public int AvailableTicketQuantity => ShowtimeSeats.Count(s => s.Status == ShowtimeSeatStatus.Available);

    }
}
