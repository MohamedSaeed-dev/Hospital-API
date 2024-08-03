namespace HospitalAPI.Features.Utils.IServices
{
    public interface IResponseStatus
    {
        ResponseStatus Ok(string message);
        ResponseStatus InternalServerError(string message, string InnerError);
        ResponseStatus Forbidden(string message);
        ResponseStatus UnAuthorized(string message);
        ResponseStatus NotFound(string message);
        ResponseStatus BadRequest(string message);
        ResponseStatus Created(string message);
        ResponseStatus NoContent();
    }
}
