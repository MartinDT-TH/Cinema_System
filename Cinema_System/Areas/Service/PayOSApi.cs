using Cinema_System.Areas.Request;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Threading.Tasks;
using Cinema_System.Areas.Types;

namespace Cinema_System.Areas.Service
{
    public class PayOSApi
    {
        public PayOSApi() { }

        public async Task<Response> CreatePayment(PaymentRequest paymentRequest, PayOS payOS)
        {
            try
            {
                PaymentData paymentData = new PaymentData(
                    paymentRequest.OrderCode,
                    paymentRequest.TotalAmount, // Sử dụng TotalAmount thay vì Amount
                    paymentRequest.Description,
                    paymentRequest.Items, // Sử dụng Foods thay vì Items
                    paymentRequest.CancelUrl,
                    paymentRequest.ReturnUrl
                );

                var response = await payOS.createPaymentLink(paymentData);
                return new Response(0, "success", response);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new Response(-1, "fail", null);
            }
        }
    }
}
