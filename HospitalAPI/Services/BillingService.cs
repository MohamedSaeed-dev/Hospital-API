using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IBillingService : IServiceAPI<Billing, BillingDTO>
    {

    }
    public class BillingService : IBillingService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public BillingService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> Add(BillingDTO entity)
        {
            try
            {
                Billing billing = _mapper.Map<Billing>(entity);
                await _db.Billings.AddAsync(billing);
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
                var record = await _db.Billings.FindAsync(Id);
                if (record == null) return 0;
                _db.Billings.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Billing>> GetAll(int skip, int take)
        {
            return await _db.Billings.Include(x => x.Appointment).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Billing?> GetById(int Id)
        {
            return await _db.Billings.Include(x => x.Appointment).SingleOrDefaultAsync(x => x.AppointmentId == Id);
        }

        public async Task<int> Update(int Id, BillingDTO entity)
        {
            try
            {
                var record = await _db.Billings.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return 0;
                
                if(entity.Status.HasValue) record.Status = entity.Status.Value;
                if(entity.Amount.HasValue) record.Amount = entity.Amount.Value;
                if(entity.AppointmentId.HasValue) record.AppointmentId = entity.AppointmentId.Value;

                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
