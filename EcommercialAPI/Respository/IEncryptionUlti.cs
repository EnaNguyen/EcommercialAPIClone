namespace EcommercialAPI.Respository
{
    public interface IEncryptionUlti
    {
        Task<string> GenerateNewID(string TypeOf, List<string> id);
    }
}
