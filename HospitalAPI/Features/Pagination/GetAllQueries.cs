namespace HospitalAPI.Features.Pagination
{
    public class GetAllQueries
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 1;
        public string SearchTerm { get; set; } = "";
        public string SortColumn { get; set; } = "";
        public string SortOrder { get; set; } = "";

    }
}
