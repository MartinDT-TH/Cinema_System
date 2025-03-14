using Cinema_System.Areas.Request;
using Cinema_System.Areas.Types;
using Cinema_System.Areas.Util;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_System.Areas.Service
{
    public class PayOSService
    {
        private PayOSApi payOSApi;

        public PayOSService()
        {
            payOSApi = new PayOSApi();
        }

        public async Task<Response> CreatePaymentAsync(int amount, long orderId, List<ItemData> foods, PayOS payOS)
        {
            var paymentRequest = new PaymentRequest
            {
                OrderCode = (int)orderId,
                TotalAmount = amount, // Sử dụng TotalAmount thay vì Amount
                Description = "Thanh toán " + RandomStringGenerator.GenerateRandomString(10),
                CancelUrl = "https://localhost:7115/Guest/Payment/CancelUrl",
                ReturnUrl = "https://localhost:7115/Guest/Payment/ReturnUrl",
                Items = foods // Sử dụng Foods thay vì Items
            };

            var response = await payOSApi.CreatePayment(paymentRequest, payOS);
            return response;
        }
    }
}
