using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.ViewModels.Admin;
using EcommercialAPI.Respository;

namespace EcommercialAPI.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly ApplicationDbContext _context;
        public OrderServices(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<List<GetOrderList>> GetOrderList(string username)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                IQueryable<Orders> query = _context.Orders;
                if (!string.IsNullOrWhiteSpace(username))
                {
                    query = query.Where(c => c.UserId == user.Id);
                }
                var result = query.Select(c => new GetOrderList
                {
                    Id = c.Id,
                    UserFullName = user != null ? user.FullName : c.Receiver,
                    UserId = c.UserId,
                    Receiver = c.Receiver,
                    Phone = c.Phone,
                    Address = c.Address,
                    isPaid = c.isPaid,
                    CreateAt = c.CreatedAt,
                    PayMethod = c.PayMethod,
                    TotalPrice = c.TotalPrice,
                    PaidAt = c.PaidAt != null ? c.PaidAt : null,
                    Status = c.Status
                }).ToList();
                foreach (var order in result)
                {
                    List<GetOrderDetailsList> details = new List<GetOrderDetailsList>();
                    var ListDetail = _context.OrderDetails.Where(od => od.OrderId == order.Id).ToList();
                    foreach (var product in ListDetail)
                    {
                        var productInfo = _context.Products.FirstOrDefault(p => p.Id == product.ProductId);
                        GetOrderDetailsList Detail = new GetOrderDetailsList
                        {
                            ProductId = product.ProductId,
                            ProductName = productInfo != null ? productInfo.Name : "Unknown",
                            Price = product.Price,
                            Quantity = product.Quantity,
                            ProductImg = productInfo != null ? productInfo.Img : null
                        };
                        details.Add(Detail);
                    }
                    order.DetailsLists = details;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<APIResponse> AcceptOrder(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order != null)
                {
                    if(order.Status != 0)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Only pending orders can be accepted"
                        };
                    }
                    order.Status= 1;
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 204,
                        Result = "Accept Order Successfully"
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Failure",
                    ErrorMessage = "Order not found"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> CancelOrder(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order != null)
                {
                    if (order.isPaid == true)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Cannot cancel a paid order"
                        };
                    }
                    if (order.Status == 1)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Order is being delivered"
                        };
                    }
                    else if (order.Status == 2)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Order had been delivered"
                        };
                    }
                    else if (order.Status == -2)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Order had been refund"
                        };
                    }
                    else if (order.Status == -1)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "Failure",
                            ErrorMessage = "Order had been cancelled"
                        };
                    }
                    else
                    {
                        order.Status = -1;
                        _context.Orders.Update(order);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        return new APIResponse
                        {
                            ResponseCode = 204,
                            Result = "Cancel Order Successfully"
                        };
                    }             
                }
                else
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failure",
                        ErrorMessage = "Order not found"
                    };
                }
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<APIResponse> PaidOrderCOD(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if(order == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failure",
                        ErrorMessage = "Order not found"
                    };
                }
                if(order.isPaid == true)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        ErrorMessage = "Order had been paid"
                    };
                }
                if(order.Status!=1)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        ErrorMessage = "Only orders being delivered can be paid by using COD"
                    };
                }    
                order.isPaid = true;
                order.Status = 2;
                order.PaidAt = DateTime.Now;
                var detail = _context.OrderDetails.Where(od => od.OrderId == id).ToList();
                foreach(var item in detail)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if(product != null)
                    {
                        product.Quantity -= item.Quantity;
                        _context.Products.Update(product);
                    }    
                }
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 204,
                    Result = "Paid Order Successfully"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> ReceivedOrderVisa(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order != null)
                {
                    if(order.isPaid==true&&order.Status==1)
                    {
                        order.Status = 2;
                        _context.Orders.Update(order);
                        transaction.Commit();
                        return new APIResponse
                        {
                            ResponseCode = 204,
                            Result = "Customer has been received order Successfully"
                        };

                    }    
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        ErrorMessage = "Only paid orders being delivered can be marked as received"
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Failure",
                    ErrorMessage = "Can't find this order"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> RefundOrder(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order.Status == 2 && order.isPaid == true)
                {
                    order.Status = -2;
                    _context.Orders.Update(order);
                    var detail = _context.OrderDetails.Where(od => od.OrderId == id).ToList();
                    foreach (var item in detail)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Quantity += item.Quantity;
                            _context.Products.Update(product);
                        }
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 204,
                        Result = "Refund Order Successfully"
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Failure",
                    ErrorMessage = "Can't find this order"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
