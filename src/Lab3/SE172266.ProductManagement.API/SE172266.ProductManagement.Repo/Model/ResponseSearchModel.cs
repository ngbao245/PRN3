using SE172266.ProductManagement.Repo.Entities;

namespace SE172266.ProductManagement.Repo.Model
{
    public class ResponseSearchModel<T> 
    {
        public IEnumerable<T>? Entities { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPages { get; set; }
        public int? TotalProducts { get; set; }
    }
}
