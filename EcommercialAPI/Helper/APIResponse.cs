namespace EcommercialAPI.Helper
{
    public class APIResponse
    {
        internal bool IsSuccess;
        public int ResponseCode { get; set; }
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Data { get; set; }
    }
    public class ErrorList
    {
        public string Error { get; set; }
        public string Detail { get; set;  }
    }
    public class LoginRequires2FAResponse
    {
        public string Message { get; set; }
        public bool Requires2FA { get; set; }
    }
}

