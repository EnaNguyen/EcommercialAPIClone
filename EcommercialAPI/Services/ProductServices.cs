using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
using EcommercialAPI.Models.ViewModels.User.Products;
using EcommercialAPI.Respository;

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
        public async Task<APIResponse> UserViewProduct()
        {
            try
            {
                var ListProduct = _context.Products.ToList();
                var Product = new List<UserProductList>();
                {
                    foreach (var item in ListProduct)
                    {
                        Product.Add(new UserProductList()
                        {

                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            Price = item.Price,
                            ReleaseDate = item.ReleaseDate,
                            Quantity = item.Quantity,
                            Status = item.Status,
                            Brand = item.Brand,
                        });
                    }
                }
                if (Product.Count() > 0)
                {
                                        
                    return new APIResponse
                    {
                        ResponseCode = 200,
                        Result = "Received Data",
                        Data = Product,
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Received Data with no Product",
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't connect to DB",
                    ErrorMessage = ex.Message
                };
            }

        }
        public async Task<APIResponse> AdminAddNewProduct(ProductCreateModel productCreateModel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ListProduct =  _context.Products.ToList();
                var existingProduct = ListProduct.FirstOrDefault(g => g.Name.ToLower().Trim() == productCreateModel.Name.ToLower().Trim());  
                if (existingProduct!=null) {
                    return new APIResponse
                    {
                        ResponseCode = 409,
                        Result = "Product already exists",
                    };
                }
                var ListIdProduct = ListProduct.Select(p=>p.Id).ToList();
                Products ProductToCreate = new Products()
                {
                    Id = await _encryption.GenerateNewID("Products", ListIdProduct),
                    Name = productCreateModel.Name,
                    ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
                    Description = productCreateModel.Description,
                    Quantity = productCreateModel.Quantity,
                    Brand = productCreateModel.Brand,
                    Price = productCreateModel.Price,
                    Status = productCreateModel.Status,
                };
                _context.Products.Add(ProductToCreate);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Adding new Product: " + productCreateModel.Name,
                    Data = productCreateModel
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't Create New Product",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<APIResponse> AdminUpdateProduct(string id, ProductEditModel productEditModel)
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

        public async Task<APIResponse> UpdateStatusProduct(string id)
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

        public async Task<APIResponse> DeleteProduct(string id)
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
