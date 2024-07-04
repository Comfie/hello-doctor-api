namespace ApiBaseTemplate.Application.Common.Models
{
    public class CustomResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }

        public CustomResponse(string message, bool success)
        {
            Message = message;
            Success = success;
        }
    }
}
