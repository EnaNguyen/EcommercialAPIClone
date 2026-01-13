using EcommercialAPI.Helper;
using EcommercialAPI.Models.ViewModels.Admin;

namespace EcommercialAPI.Respository
{
    public interface IOrderServices
    {
        Task<List<GetOrderList>> GetOrderList(string username);
        Task<APIResponse> CancelOrder(int id);
        Task<APIResponse> AcceptOrder(int id);
        Task<APIResponse> PaidOrderCOD(int id);
        Task<APIResponse> ReceivedOrderVisa(int id);
        Task<APIResponse> RefundOrder(int id);
    }
}
