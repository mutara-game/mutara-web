namespace Mutara.Web.Api
{
    public class ApiResponse
    {
        public int StatusCode { get; }
        public string? Message { get;  }

        public ApiResponse(int statusCode, string message = null)
        {
            this.StatusCode = statusCode;
            this.Message = message ?? GetDefaultMessage(statusCode);
        }

        private static string? GetDefaultMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 403: return "Forbidden";
                case 404: return "Page Not Found";
                default: return null;
            }
        }
    }
    
}