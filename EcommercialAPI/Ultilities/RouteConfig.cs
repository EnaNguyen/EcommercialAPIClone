namespace EcommercialAPI.Ultilities
{
    public static class RouteConfig
    {
        public static string GetIpAddress(HttpContext context)
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1") 
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = "127.0.0.1"; 
            }

            return ipAddress;
        }
    }
}
