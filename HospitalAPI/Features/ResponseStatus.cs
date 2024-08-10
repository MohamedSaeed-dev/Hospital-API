namespace HospitalAPI.Features
{
    public class ResponseStatus
    {
        public int StatusCode { get; set; }
        public object? Message { get; set; }
        public ResponseStatus(int code, object? message = null)
        {
            StatusCode = code;
            Message = message;
        }
    }
}
