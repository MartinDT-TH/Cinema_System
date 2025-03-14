using Microsoft.AspNetCore.Mvc;
using Cinema_System.Areas.Service;
using Cinema_System.Areas.Request;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_System.Areas
{
    [Area("Guest")]
    [Route("Guest/[controller]/[action]")]
    public class PaymentController : Controller
    {
        private readonly PayOSService _payOSService;
        private readonly PayOS _payOS;

        public PaymentController(PayOSService payOSService, PayOS payOS)
        {
            _payOSService = payOSService;
            _payOS = payOS;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromForm] PaymentRequest request)
        {
            if (request == null || request.TotalAmount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            Random random = new Random();
            var orderId = random.Next(1, int.MaxValue);

            // Chuẩn bị danh sách sản phẩm từ Seats & Foods
            var items = new List<ItemData>();

            // Thêm ghế vào danh sách
            foreach (var seat in request.Seats)
            {
                items.Add(new ItemData($"Seat {seat}", 80000, 1));

            }

            // Thêm thức ăn vào danh sách
            foreach (var food in request.Items)
            {
                items.Add(new ItemData(food.name, food.price, food.quantity));

            }

            // Gọi dịch vụ PayOS để tạo thanh toán
            var response = await _payOSService.CreatePaymentAsync(request.TotalAmount, orderId, items, _payOS);

            if (response.error == 0)
            {
                return Redirect(url: ((CreatePaymentResult)response.data).checkoutUrl);
            }

            return BadRequest("Payment failed.");
        }


        // Trang hủy
        [HttpGet]
        public IActionResult CancelUrl()
        {
            return View();
        }

        // Trang thành công
        [HttpGet]
        public IActionResult ReturnUrl()
        {
            return View();
        }
    }
}
