using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.ViewModels.ResponseStatus;
using System.Linq.Expressions;

namespace HospitalAPI.ServicesAPI
{
    public interface IServiceAPI<T, TDTO>
    {
        Task<PagedList<T>> GetAll(GetAllQueries queries);
        Expression<Func<T, object>> GetProperty(string sortColumn);
        Task<T?> GetById(int Id);
        Task<ResponseStatus> Add(TDTO entity);
        Task<ResponseStatus> Update(int Id, TDTO entity);
        Task<ResponseStatus> DeleteById(int Id);
    }
}
