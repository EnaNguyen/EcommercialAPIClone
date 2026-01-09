using EcommercialAPI.Data;
using EcommercialAPI.Respository;
using System.Threading.Tasks;
namespace EcommercialAPI.Ultilities
{
    public class EncryptionUlti  : IEncryptionUlti
    {
        public EncryptionUlti()
        {
        }
        public async Task<string> GenerateNewID (string TypeOf, List<string> ListId)
        {
            if(TypeOf == "Products")
            {
                var maxId = ListId.Select(p => int.Parse(p)).Max();
                return (maxId + 1).ToString();
            }
            return "3";
        }
    }
}
