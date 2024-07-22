using AutoMapper;
using AutoMapper.QueryableExtensions;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.Models.ViewModels.ResponseStatus;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalAPI.Repositories
{
    public class UserRepository : IUserService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public UserRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }

        public async Task<ResponseStatus> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Users.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("User is not exist");
                _db.Users.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("User is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<UserViewModel>> GetAll(GetAllQueries queries)
        {
            var users = queries.SortOrder.ToLower() == "desc" ?
                _db.Users.Where(x => x.UserName.ToLower().Contains(queries.SearchTerm.ToLower())).OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Users.Where(x => x.UserName.ToLower().Contains(queries.SearchTerm.ToLower())).OrderBy(GetProperty(queries.SortColumn));
            var usersViewModel = users.ProjectTo<UserViewModel>(_mapper.ConfigurationProvider);
            return await PagedList<UserViewModel>.CreatePagedList(usersViewModel, queries.Page, queries.PageSize);
        }

        public async Task<UserViewModel?> GetById(int Id)
        {
            var user = await _db.Users.FindAsync(Id);
            var userViewModel = _mapper.Map<UserViewModel>(user);
            return userViewModel;
        }

        public Expression<Func<User, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "name" => d => d.UserName,
                "email" => d => d.Email,
                _ => d => d.Id
            };
        }
    }
}
