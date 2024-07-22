using AutoMapper;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels.ResponseStatus;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalAPI.Repositories
{
    public class DepartmentRepository : IDepartmentService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public DepartmentRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }

        public async Task<ResponseStatus> Add(DepartmentDTO entity)
        {
            try
            {
                var isExist = _db.Departments.Any(x => x.Name == entity.Name);
                if (isExist) return _response.BadRequest("Department is already Exist");
                var department = _mapper.Map<Department>(entity);
                await _db.Departments.AddAsync(department);
                await _db.SaveChangesAsync();
                return _response.Created("Department is Created Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseStatus> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Departments.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Department is not exist");
                _db.Departments.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Department is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Department>> GetAll(GetAllQueries queries)
        {
            var departments = queries.SortOrder.ToLower() == "desc" ?
                _db.Departments.Where(x => x.Name.ToLower().Contains(queries.SearchTerm.ToLower())).OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Departments.Where(x => x.Name.ToLower().Contains(queries.SearchTerm.ToLower())).OrderBy(GetProperty(queries.SortColumn));
            return await PagedList<Department>.CreatePagedList(departments, queries.Page, queries.PageSize);
        }

        public Expression<Func<Department, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "name" => d => d.Name!,
                _ => d => d.Id
            };
        }

        public async Task<Department?> GetById(int Id)
        {
            return await _db.Departments.FindAsync(Id);
        }

        public async Task<ResponseStatus> Update(int Id, DepartmentDTO entity)
        {
            try
            {
                var record = await _db.Departments.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Department is not exist");

                if (!string.IsNullOrEmpty(entity.Name)) record.Name = entity.Name;

                await _db.SaveChangesAsync();
                return _response.Ok("Department is Updated Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
