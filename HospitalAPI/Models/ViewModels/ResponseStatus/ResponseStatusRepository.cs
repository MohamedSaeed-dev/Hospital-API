namespace HospitalAPI.Models.ViewModels.ResponseStatus
{
    public class ResponseStatusRepository : IResponseStatus
    {
        public ResponseStatus BadRequest(string message)
        {
            return new ResponseStatus(400, message);
        }

        public ResponseStatus Created(string message)
        {
            return new ResponseStatus(201, message);
        }

        public ResponseStatus Forbidden(string message)
        {
            return new ResponseStatus(403, message);
        }

        public ResponseStatus InternalServerError(string message, string InnerError)
        {
            var msg = $"Error = {message} \nInnerError = {InnerError}";
            return new ResponseStatus(500, msg);
        }

        public ResponseStatus NoContent()
        {
            return new ResponseStatus(204);
        }

        public ResponseStatus NotFound(string message)
        {
            return new ResponseStatus(404, message);
        }

        public ResponseStatus Ok(string message)
        {
            return new ResponseStatus(200, message);
        }

        public ResponseStatus UnAuthorized(string message)
        {
            return new ResponseStatus(401, message);
        }
    }
}
