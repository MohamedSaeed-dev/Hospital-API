using HospitalAPI.Features;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.ServicesAPI;
using System.Linq.Expressions;

namespace HospitalAPI.Services
{
    public interface IUserService
    {
        Task<PagedList<UserViewModel>> GetAll(GetAllQueries queries);
        Expression<Func<User, object>> GetProperty(string sortColumn);
        Task<UserViewModel?> GetById(int Id);
        Task<User> GetByProperty(Func<User, bool> predicate);
        Task<ResponseStatus> DeleteById(int Id);
    }
}
