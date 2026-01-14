using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
using EcommercialAPI.Models.ViewModels.Admin;
using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
namespace EcommercialAPI.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IEmailServices _emailServices;
        public UserServices(ApplicationDbContext context, IAuthenticationServices authenticationServices, IEmailServices emailServices)
        {
            _context = context;
            _authenticationServices = authenticationServices;
            _emailServices = emailServices;
        }
        public async Task<List<GetUserList>> GetListUser(string? usernameOrEmail)
        {
            try
            {        
                IQueryable<Users> query = _context.Users;
                if (!string.IsNullOrWhiteSpace(usernameOrEmail))
                {
                    query = query.Where(g => g.Username == usernameOrEmail || g.Email == usernameOrEmail);
                }
                var result = await query.Select(p => new GetUserList
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    UserName = p.Username,
                    Email = p.Email,
                    Phone = p.Phone,
                    Gender = p.Gender,
                    CreatedAt = p.CreatedAd,
                    DateOfBirth = p.DayOfBirth,
                    Role = p.Role,
                    Status = p.Status,
                    Img = p.Img != null ? p.Img : "",
                }).ToListAsync();
                var test = result;
                int i = 0;
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<APIResponse> CreateUser(UserCreateModal model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var errors = new Dictionary<string, string>();
                if (model.ReEnterPassword != model.Password)
                {
                    errors["ReEnterPassword"] = "Mật khẩu nhập lại không trùng khớp";
                }
                var checkUsernameOrEmail = _context.Users.FirstOrDefault(p => p.Username == model.Username || p.Email == model.Email);

                if (checkUsernameOrEmail != null)
                {
                    errors["AlreadyExist"] = "Tài Khoản hoặc Gmail đã được đăng ký";
                }
                var emailValidate = new EmailAddressAttribute();
                if (!emailValidate.IsValid(model.Email))
                {
                    errors["Email"] = "Email không hợp lệ";
                }
                //if (model.Phone==null||model.Phone.Length>=12 || model.Phone.Length<=9 || !long.TryParse(model.Phone, out _))
                //{
                //    errors["Phone"] = "Số điện thoại không hợp lệ";
                //}
                if (errors.Count > 0)
                {
                    var errorLists = errors.Select(kvp => new ErrorList
                    {
                        Error = kvp.Key,
                        Detail = kvp.Value
                    }).ToArray();

                    return new APIResponse
                    {
                        ResponseCode = 400, 
                        Result = "Validation Failed",
                        ErrorMessage = "Dữ liệu đầu vào không hợp lệ",
                        Data = errorLists
                    };
                }
                string First2Digit = "";
                if (model.Role.Trim() == "admin")
                    First2Digit = "AD";
                else if (model.Role.Trim() == "customer")
                    First2Digit = "US";
                else
                    First2Digit = "MN";
                var MaxUserId = _context.Users.Where(g => g.Role.Trim() == (model.Role.Trim())).OrderByDescending(p => p.Id).FirstOrDefault();
                string newId;
                if (MaxUserId == null || string.IsNullOrEmpty(MaxUserId.Id))
                {
                    newId = First2Digit+ "000001";
                }
                else
                {
                    string numberPart = MaxUserId.Id.Substring(2); 
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        int nextNumber = currentNumber + 1;
                        newId = First2Digit + nextNumber.ToString("D6"); 
                    }
                    else
                    {
                        throw new Exception("Định dạng ID hiện tại không hợp lệ");
                    }
                }    
                Users newUser = new Users()
                {
                    Id = newId,
                    Username = model.Username,
                    FullName = model.Fullname,
                    Email = model.Email,
                    Phone = model.Phone??"",
                    DayOfBirth = model.DayOfBirth??DateOnly.FromDateTime(DateTime.Now),
                    Password = _authenticationServices.HashCode(model.Password),
                    CreatedAd = DateOnly.FromDateTime(DateTime.Now),
                    Role = "Customer",
                    Gender = model.Gender??0,
                    Status = 1,
                    TwoFA = false
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                transaction.Commit();
                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Created Account Successfully",
                    Data = newUser
                };
            }
            catch (Exception ex) {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't not create new account",
                    ErrorMessage = ex.Message
                };
            }
           
        }
        public async Task<APIResponse> UpdateInfoUser(UserInfoChangeModel model,string username)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var currentUserInfo  =  _context.Users.Where(g=>g.Username == username|| g.Email==username).FirstOrDefault();
                if (currentUserInfo != null)
                {
                    string currentEmail = currentUserInfo.Email;
                    var ExistedUsername = _context.Users.Where(g => g.Email == model.Email && g.Email != currentEmail).FirstOrDefault();
                    if (ExistedUsername != null)
                    {
                        return new APIResponse
                        {
                            ResponseCode = 400,
                            Result = "This Email has been Registered for another Account",
                        };
                    }
                    currentUserInfo.Email = model.Email;
                    currentUserInfo.Phone = model.Phone;
                    currentUserInfo.FullName = model.FullName;
                    currentUserInfo.DayOfBirth = model.DateOfBirth;
                    currentUserInfo.Gender = model.Gender;
                    currentUserInfo.Status= model.Status;
                    currentUserInfo.TwoFA = model.TwoFA;
                    currentUserInfo.Img = model.Img;
                    _context.Users.Update(currentUserInfo);
                    _context.SaveChanges();
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 202,
                        Result = "Updated Account " + username + " successfully",
                    };
                }
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Can't find this account",
                };
            }
            catch (Exception ex) 
            {
                transaction.Rollback();
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Can't not update this account",
                    ErrorMessage = ex.Message
                };
            }
        }
        public Task<APIResponse> UpdatePasswordUser(UserChangePasswordModel model, string username)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse> ResetPasswordUser(string username)
        {
            try
            {
                var currentAccount = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                if (currentAccount != null) 
                {
                    string otpCode =  _authenticationServices.GenerateOtp();
                    currentAccount.CurrentOtpCode = _authenticationServices.HashCode(otpCode);
                    currentAccount.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);
                    await _context.SaveChangesAsync();
                    var sendEmail = await _emailServices.SendEmail(currentAccount.Email, otpCode, "Thiết lập lại mật mật khẩu (Reset Password )", "Password Reset");
                    if (sendEmail.ResponseCode != 200)
                    {
                        return new APIResponse { ResponseCode = 500, Result = "Failed to send OTP" };
                    }
                    return new APIResponse
                    {
                        ResponseCode = 200,
                        Result = "Password Reset OTP Sent",
                        Data = new LoginRequires2FAResponse
                        {
                            Message = "OTP sent to your email",
                            Requires2FA = true
                        }
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 400,
                    Result = "Can't find this account"
                };
            }
            catch(Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 400,
                    Result = "Can't find this account",
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<APIResponse> ResetPasswordUserOTP(string username, string otp)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var userReset = _context.Users.FirstOrDefault(g => g.Username == username || g.Email == username);
                if (userReset == null)
                {
                    return new APIResponse { ResponseCode = 404, Result = "User not found" };
                }
                if (userReset.OtpExpiryTime < DateTime.UtcNow ||
                    string.IsNullOrEmpty(userReset.CurrentOtpCode) ||
                    userReset.CurrentOtpCode != _authenticationServices.HashCode(otp))
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Invalid or expired OTP"
                    };
                }
                string NewPass = _authenticationServices.GenerateOtp();
                string HashPass = _authenticationServices.HashCode (NewPass);
                
                userReset.Password= HashPass;
                userReset.CurrentOtpCode = null;
                userReset.OtpExpiryTime = null;
                var sendEmail = await _emailServices.SendEmail(userReset.Email, NewPass, "Mật Khẩu của bạn đã được thay đổi ( Your Password Has Been Reset )", "Password Reset");
                if (sendEmail.ResponseCode != 200)
                {
                    return new APIResponse { ResponseCode = 500, Result = "Failed to send email" };
                }
                _context.Users.Update(userReset);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return new APIResponse()
                {
                    ResponseCode = 200,
                    Result = "Password Resetted, Please Check your Email for it"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new APIResponse()
                {
                    ResponseCode = 200,
                    Result = "Password Resetted, Please Check your Email for it"
                };
            }
        }
    }
}