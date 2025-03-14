using Cinema.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ShowtimeSeat
{
    [Key]
    public int ShowtimeSeatID { get; set; }

    [Required]
    public int ShowtimeID { get; set; } // Foreign key

    [Required]
    public int SeatID { get; set; } // Foreign key

   

    [Required]
    [Range(0.00, 9999.99, ErrorMessage = "Price must be a positive value.")]
    public double Price { get; set; }  // Default price

    [Required]
    [EnumDataType(typeof(TicketType))]
    public TicketType SeatType { get; set; } = TicketType.Standard; // Default to Standard

    [Required]
    [EnumDataType(typeof(ShowtimeSeatStatus))]
    public ShowtimeSeatStatus Status { get; set; } = ShowtimeSeatStatus.Available;

    // Navigation properties
    [ForeignKey("ShowtimeID")]
    [ValidateNever]
    public virtual ShowTime Showtime { get; set; }

    [ForeignKey("SeatID")]
    [ValidateNever]
    public virtual Seat Seat { get; set; }
}



// Enum for different ticket types
public enum TicketType
{
   Vip,
   Standard,
   Double
}

// Enum for seat booking status
public enum ShowtimeSeatStatus
{
    Available,
    Booked,
    CheckedIn
}
