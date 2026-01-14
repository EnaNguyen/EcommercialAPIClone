using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.DTO;
using EcommercialAPI.Respository;
using Microsoft.EntityFrameworkCore;
namespace EcommercialAPI.Services
{
    public class CheckoutServices : ICheckoutServices
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CheckoutServices> _logger;
        private readonly VnPayConfig _vnpayConfig;
        private readonly IVnPayServices _vnPayService;
        public CheckoutServices(ApplicationDbContext context, ILogger<CheckoutServices> logger, VnPayConfig vnpayConfig, IVnPayServices vnPayService)
        {
            _context = context;
            _logger = logger;
            _vnpayConfig = vnpayConfig;
            _vnPayService = vnPayService;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDto request, HttpContext httpContext)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (request == null || request.CartId <= 0)
                {
                    return new PaymentResponse { Success = false, Message = "Yêu cầu thanh toán không hợp lệ" };
                }
                if (string.IsNullOrEmpty(request.PaymentMethod) || !new[] { "cash", "cod", "vnpay" }.Contains(request.PaymentMethod.ToLower()))
                {
                    return new PaymentResponse { Success = false, Message = "Phương thức thanh toán không hợp lệ" };
                }
                if (request.TotalPrice <= 0)
                {
                    return new PaymentResponse { Success = false, Message = "Số tiền cuối cùng không hợp lệ" };
                }

              
                _logger.LogInformation($"Processing payment: CartId={request.CartId}, PaymentMethod={request.PaymentMethod}, TotalPrice={request.TotalPrice}");
                var cart =_context.Carts.FirstOrDefault(g=>g.Id==request.CartId);
                var userPaid = _context.Users.FirstOrDefault(g => g.Id == cart.UserId);
                var cartDetail = _context.CartDetails.Where(g=>g.CartId==request.CartId).ToList();
                if(cart==null)
                {
                    return new PaymentResponse { Success = false, Message = "Giỏ hàng không tồn tại" };
                }
                if (cartDetail == null)
                {
                    return new PaymentResponse { Success = false, Message = "Giỏ hàng không chứa sản phầm nào" };
                }
                Orders newOrder = new Orders()
                {
                    UserId = userPaid.Id,
                    TotalPrice = cart.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    isPaid = false,
                    Status = 0,
                    Receiver = string.IsNullOrWhiteSpace(request.Receiver) ? request.Receiver : userPaid.FullName,
                    Phone = string.IsNullOrWhiteSpace(request.Phone) ? request.Phone : userPaid.Phone,
                    Address = request.Address,
                    PayMethod = request.PaymentMethod
                };
                _context.Orders.Add(newOrder);
                _context.CartDetails.RemoveRange(cartDetail);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                List<OrderDetails> ordersDetail = new List<OrderDetails>();
                foreach(var item in cartDetail)
                {
                    ordersDetail.Add(new OrderDetails()
                    {
                        OrderId = newOrder.Id,
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                    });
                }    
                _context.OrderDetails.AddRangeAsync(ordersDetail);
                await _context.SaveChangesAsync();
                if (request.PaymentMethod =="VnPay")
                {
                    var vnPayRequest = new VnPaymentRequest
                    {
                        OrderId = newOrder.Id.ToString(),
                        FullName = string.IsNullOrWhiteSpace(request.Receiver)?request.Receiver : userPaid.FullName,
                        Description = $"Thanh toán đơn hàng #{newOrder.Id.ToString()}",
                        Amount = Convert.ToDouble(Math.Ceiling(newOrder.TotalPrice)),
                        CreatedDate = DateTime.Now
                    };
                    var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, vnPayRequest);
                    transaction.Commit();
                    return new PaymentResponse
                    {

                        Success = true,
                        TotalPrice = Convert.ToDecimal(newOrder.TotalPrice),
                        Message = paymentUrl
                    };
                }
                else 
                {
                    transaction.Commit();
                    return new PaymentResponse
                    {
                        Success = true,
                        TotalPrice = Convert.ToDecimal(newOrder.TotalPrice),
                        Message = "Đã đặt hàng theo hình thức " +request.PaymentMethod + " thành công",
                        OrderId = newOrder.Id
                    };
                } 
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình đặt hàng"
                };
            }
        }

        public async Task<PaymentResponse> ProcessVnPayCallbackAsync(IQueryCollection query, HttpContext httpContext)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                Console.WriteLine("[DEBUG] Starting VNPay callback processing...");
                var vnPayResponse = _vnPayService.PaymentExecute(query);
                Console.WriteLine($"[DEBUG] VNPay response: Success={vnPayResponse.Success}, Code={vnPayResponse.VnPayResponseCode}");
                var vnp_TxnRef = vnPayResponse.OrderId;
                var orderTemp = _context.Orders.FirstOrDefault(g => g.Id.ToString() == vnp_TxnRef.Split('_')[0]);
                if (!vnPayResponse.Success)
                {
                    Carts rebackCart = new Carts
                    {
                        UserId = orderTemp.UserId,
                        TotalPrice = orderTemp.TotalPrice,
                        CreatedAt= orderTemp.CreatedAt,
                    };
                    _context.Carts.Add(rebackCart);
                    await _context.SaveChangesAsync();
                    var listDetail = _context.OrderDetails.Where(g => g.OrderId == orderTemp.Id).ToList();
                    foreach(var item in listDetail)
                    {
                        _context.CartDetails.Add(
                            new CartDetails
                            {
                                CartId = rebackCart.Id,
                                ProductId = item.ProductId,
                                Price = item.Price,
                                Quantity = item.Quantity
                            });
                    } 
                    await _context.OrderDetails.AddRangeAsync(listDetail);
                    _context.Orders.RemoveRange(orderTemp);
                    await _context.SaveChangesAsync();
                    string message = vnPayResponse.VnPayResponseCode switch
                    {
                        "01" => "Giao dich chua hoan tat (nguoi dung huy)",
                        "02" => "Giao dich bi loi",
                        "24" => "Giao dich bi huy boi nguoi dung",
                        _ => "Thanh toan VNPay khong thanh cong"
                    };

                    message = System.Text.RegularExpressions.Regex.Replace(message, "[^\\x00-\\x7F]", "");
                    Console.WriteLine($"[ERROR] VNPay payment failed: {message}");

                    httpContext.Response.Redirect(
                        $"http://localhost:8080/PaymentFail?status=failed&message={Uri.EscapeDataString(message)}"
                    );
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = message,
                        TransactionId = vnPayResponse.TransactionId
                    };
                }
                orderTemp.Status = 1;

                _context.Orders.RemoveRange(orderTemp);
                await _context.SaveChangesAsync();
                var redirectUrl = $"http://localhost:4200/PaymentSuccess?status=success&orderId={orderTemp.Id}&transactionId={vnPayResponse.TransactionId}";
                Console.WriteLine($"[DEBUG] Redirecting to: {redirectUrl}");
                httpContext.Response.Redirect(redirectUrl);
                transaction.Commit();
                return new PaymentResponse
                {
                    Success = true,
                    OrderId = orderTemp.Id,
                    Message = "VNPay payment successful",
                    TransactionId = vnPayResponse.TransactionId,
                    TotalPrice = Convert.ToDecimal(orderTemp.TotalPrice)
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new PaymentResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
