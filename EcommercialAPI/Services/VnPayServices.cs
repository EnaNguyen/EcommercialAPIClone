using EcommercialAPI.Helper;
using EcommercialAPI.Respository;
using EcommercialAPI.Ultilities;
using System.Net.WebSockets;

namespace EcommercialAPI.Services
{
    public class VnPayService : IVnPayServices
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequest model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", RouteConfig.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh Toan Cho Don Hang:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); 
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_ExpireDate", model.CreatedDate.AddMinutes(30).ToString("yyyyMMddHHmmss"));
           
            vnpay.AddRequestData("vnp_TxnRef", model.OrderId + "_"+ DateTime.Now.Ticks.ToString()); 

            var paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentResponse model)
        {
            throw new NotImplementedException();
        }

        public VnPaymentResponse PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_OrderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponse
                {
                    Success = false,
                    OrderId = vnp_OrderId,
                    TransactionId = vnp_TransactionId.ToString(),
                    VnPayResponseCode = vnp_ResponseCode,
                    OrderDescription = vnp_OrderInfo
                };
            }

            bool isSuccess = !string.IsNullOrEmpty(vnp_TransactionStatus) && vnp_TransactionStatus == "00";

            return new VnPaymentResponse
            {
                Success = isSuccess,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_OrderId,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
