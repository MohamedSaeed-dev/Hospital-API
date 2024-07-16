using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IDepartmentService : IServiceAPI<Department, DepartmentDTO>
    {

    }
    public class DepartmentService : IDepartmentService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public DepartmentService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> Add(DepartmentDTO entity)
        {
            try
            {
                var department = _mapper.Map<Department>(entity);
                await _db.Departments.AddAsync(department);
                return _db.SaveChanges();

            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Departments.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;
                _db.Departments.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Department>> GetAll(int skip, int take)
        {
            return await _db.Departments.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Department?> GetById(int Id)
        {

            return await _db.Departments.FindAsync(Id);
            
        }

        public async Task<int> Update(int Id, DepartmentDTO entity)
        {
            try
            {
                var record = await _db.Departments.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;
                
                if (entity.Name != null) record.Name = entity.Name;

                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
