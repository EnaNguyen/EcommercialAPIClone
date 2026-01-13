using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.ViewModels.Admin;
using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
namespace EcommercialAPI.Services
{
    public class CartServices : ICartServices
    {   
        private readonly ApplicationDbContext _context;
        public CartServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<APIResponse> AddItemToCart(string userId, int productId, int quantity)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);                   
                if (cart == null)
                {
                    cart = new Carts
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        TotalPrice = 0
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartId == cart.Id && cd.ProductId == productId);

                if (cartDetail != null)
                {
                    cartDetail.Quantity += quantity;                
                    var productAdd = _context.Products.FirstOrDefault(g => g.Id == productId);
                    cart.TotalPrice += quantity * productAdd.Price;
                    _context.CartDetails.Update(cartDetail);
                    _context.Carts.Update(cart);

                }
                else
                {
                    var productAdd = _context.Products.FirstOrDefault(g => g.Id == productId);
                    cartDetail = new CartDetails
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = productAdd.Price
                    };
                    cart.TotalPrice += cartDetail.Quantity * productAdd.Price;
                    _context.CartDetails.Add(cartDetail);
                    _context.Carts.Update(cart);
                }
                 
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new APIResponse { ResponseCode = 200, Result = "Item added to cart successfully" };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't connect to server",
                    ErrorMessage = ex.Message,
                };
            }
        }


        public async Task<List<GetCartList>> GetCartList(string? userId)
        {
            try
            {
                IQueryable<Carts> query = _context.Carts;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    query = query.Where(c => c.UserId == userId);                  
                }
                var result = await query.Select(g => new GetCartList
                {
                    Id = g.Id,
                    UserId = g.UserId,
                    TotalPrice = g.TotalPrice,
                    CreatedAt = g.CreatedAt,
                }).ToListAsync();
                foreach (var item in result)
                {
                    List<GetCartDetailList> listDetail = new List<GetCartDetailList>();
                    var ListDetail = _context.CartDetails.Where(g => g.CartId == item.Id).ToList();
                    foreach (var dt in ListDetail)
                    {
                        var NameProduct = _context.Products.FirstOrDefault(g => g.Id == dt.ProductId);
                        listDetail.Add(new GetCartDetailList
                        {
                            Id = dt.ProductId,
                            ProductId = dt.ProductId,
                            ProductName = NameProduct.Name,
                            Price = dt.Price,
                            Quantity = dt.Quantity,
                        });
                    }
                    item.Details = listDetail;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<APIResponse> RemoveItemFromCart(string userId, int productId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var userCart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
                var CartDetail = _context.CartDetails.FirstOrDefault(c => c.ProductId == productId && c.CartId == userCart.Id);
                userCart.TotalPrice -= CartDetail.Quantity*CartDetail.Price;
                _context.CartDetails.Remove(CartDetail);
                _context.Carts.Update(userCart);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 204,
                    Result = "Remove items from cart successfully"
                };
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode =500,
                    Result = "Can't connect to DB",
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<APIResponse> IncreaseQuantity(string userId, int productId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var product = _context.Products.FirstOrDefault(g => g.Id == productId);
                var cart = _context.Carts.FirstOrDefault(g=>g.UserId== userId);
                var cartDetail = _context.CartDetails.FirstOrDefault(g=>g.ProductId== productId && g.CartId==cart.Id);
                if(cartDetail == null || cartDetail.Quantity >= product.Quantity) 
                {
                    transaction.Rollback();
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "The quantity of this product in your cart reach the limit",
                        Data = "Max quantity is " + product.Quantity
                    };
                }
                cartDetail.Quantity++;
                cart.TotalPrice += cartDetail.Price;
                _context.CartDetails.Update(cartDetail);
                _context.Carts.Update(cart);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 202,
                    Result = "Increase Quantity of product in cart successfully",
                    Data =""
                }; 
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't connect to DB",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> DecreaseQuantity(string userId, int productId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var product = _context.Products.FirstOrDefault(g => g.Id == productId);
                var cart = _context.Carts.FirstOrDefault(g => g.UserId == userId);
                var cartDetail = _context.CartDetails.FirstOrDefault(g => g.ProductId == productId && g.CartId == cart.Id);
                if (cartDetail == null || cartDetail.Quantity ==1)
                {
                    transaction.Rollback();
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Can't reduce the quantity of this product in cart anymore",
                        Data = "Must have at lease 1 products"
                    };
                }
                cartDetail.Quantity--;
                cart.TotalPrice -= cartDetail.Price;
                _context.CartDetails.Update(cartDetail);
                _context.Carts.Update(cart);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 202,
                    Result = "Increase Quantity of product in cart successfully",
                    Data = ""
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't connect to DB",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> ChangeAmountDirect(string userId, int productId, int quantity)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var product = _context.Products.FirstOrDefault(g => g.Id == productId);
                var cart = _context.Carts.FirstOrDefault(g => g.UserId == userId);
                var cartDetail = _context.CartDetails.FirstOrDefault(g => g.ProductId == productId && g.CartId == cart.Id);
                if (quantity >= product.Quantity || quantity <= 0)
                {
                    transaction.Rollback();
                    return new APIResponse
                    {
                        ResponseCode= 404,
                        Result= "The quantity of this product in cart is invalid"

                    };
                }
                cart.TotalPrice -= cartDetail.Price * cartDetail.Quantity;
                cartDetail.Quantity = quantity;
                cart.TotalPrice += cartDetail.Price * quantity;
                _context.CartDetails.Update(cartDetail);
                _context.Carts.Update(cart);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 202,
                    Result = "Adjust the quantity of product successfully",
                    Data = ""
                };
            }
            catch
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode= 500,
                    Result ="Can't conmect to Db",
                    Data = ""
                };
                
            }
        }
    }
}