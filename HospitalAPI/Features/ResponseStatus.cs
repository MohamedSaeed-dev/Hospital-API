namespace HospitalAPI.Features
{
    public class ResponseStatus
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; } = string.Empty;
        public ResponseStatus(int code, string? message = null)
        {
            StatusCode = code;
            Message = message;
        }
    }
}
