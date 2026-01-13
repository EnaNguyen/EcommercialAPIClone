using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
using EcommercialAPI.Models.ViewModels.User.Products;
using EcommercialAPI.Respository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EcommercialAPI.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionUlti _encryption;
        public ProductServices(ApplicationDbContext context, IEncryptionUlti encryption)
        {
            _context = context;
            _encryption = encryption;
        }
        public async Task<List<UserProductList>> UserViewProduct(string? name)
        {
            try
            {
                IQueryable<Products> query = _context.Products;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(c => c.Name.Contains(name));
                }
                var result = await query
                    .Select(p => new UserProductList
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ReleaseDate = p.ReleaseDate,
                        Quantity = p.Quantity,
                        Status = p.Status,
                        Brand = p.Brand,
                        Img = p.Img
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<APIResponse> AdminAddNewProduct(ProductCreateModel productCreateModel)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bool exists = await _context.Products
                    .AnyAsync(p => p.Name.Trim().ToLower() == productCreateModel.Name.Trim().ToLower());

                if (exists)
                {
                    return new APIResponse
                    {
                        ResponseCode = 409,
                        Result = "Product with this name already exists"
                    };
                }

                var newProduct = new Products
                {
                    Name = productCreateModel.Name,
                    Description = productCreateModel.Description,
                    Brand = productCreateModel.Brand,
                    Price = productCreateModel.Price,
                    Quantity = productCreateModel.Quantity,
                    Status = productCreateModel.Status,
                    ReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Img = productCreateModel.Img
                };
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();  

                await transaction.CommitAsync();

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = $"Product '{newProduct.Name}' added successfully",
                    Data = newProduct  
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed to create product",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> AdminUpdateProduct(int id, ProductEditModel productEditModel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productExisting = _context.Products.FirstOrDefault(p => p.Id == id);
                if (productExisting != null)
                {
                    productExisting.Name = productEditModel.Name;
                    productExisting.Description = productEditModel.Description;
                    productExisting.Price = productEditModel.Price;
                    productExisting.Status = productEditModel.Status;
                    productExisting.Brand = productEditModel.Brand;
                    productExisting.Quantity = productEditModel.Quantity;
                    productExisting.Img = productEditModel.Img;
                    _context.Products.Update(productExisting);
                    _context.SaveChanges();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 200,
                        Result = "Update Product Successfully",
                        Data = productExisting
                    };
                }
                transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Product not found",
                };
            }
            catch (Exception ex) 
            {
                transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't Edit Product",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> UpdateStatusProduct(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var UpdateStatusProduct = _context.Products.FirstOrDefault(p=>p.Id == id);
                if (UpdateStatusProduct != null)
                {
                    UpdateStatusProduct.Status = UpdateStatusProduct.Status==1?0:1;
                    _context.Products.Update(UpdateStatusProduct);
                    _context.SaveChanges();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 200,
                        Result = "Update Product with " + UpdateStatusProduct.Id + " successfully",
                        Data = UpdateStatusProduct
                    };
                }
                transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Can't Find this Product"
                };
            }
            catch(Exception ex)
            {
                transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't Update This Product Status",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> DeleteProduct(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productRemove = _context.Products.FirstOrDefault(p=>p.Id == id);
                if(productRemove != null)
                {
                    _context.Products.RemoveRange(productRemove);
                    _context.SaveChanges();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 204,
                        Result = "Remove product with " + productRemove.Id + " successfully"
                    };
                }    
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Can't find this product"
                };
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't Remove this product from database",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
