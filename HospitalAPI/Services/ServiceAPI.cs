namespace HospitalAPI.ServicesAPI
{
    public interface IServiceAPI<T, TDTO>
    {
        Task<IEnumerable<T>> GetAll(int? startIndex, int? endIndex);
        Task<T?> GetById(int Id);
        Task<int> Add(TDTO entity);
        Task<int> Update(int Id, TDTO entity);
        Task<int> DeleteById(int Id);
    }
}
