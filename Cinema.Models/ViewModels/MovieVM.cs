using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using Microsoft.AspNetCore.Mvc.Rendering;
namespace Cinema.Models.ViewModels
{
    public class MovieVM
    {
        // dung de hien thi trang chinh nhu ben cinestar

        public IEnumerable<Movie> ShowingMovies { get; set; } // film dang chieu,    // film sap chieu
                                                              // phan biet qua isUpcommingMOvie

        public IEnumerable<Movie> UpcommingMovies { get; set; }

        public IEnumerable<Coupon> CouponMovies { get; set; } // ma giam gia

        public int ShowingMoviesCount { get; set; } 
        public int UpcommingMoviesCount { get; set; }
        public int CouponCount { get; set; }

        public int PageSize { get; set; }

      


    }
}
