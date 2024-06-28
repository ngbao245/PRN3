using System.ComponentModel.DataAnnotations;
using static SE172266.ProductManagement.API.Enum.Sort;

namespace SE172266.ProductManagement.API.Model.ProductModel
{
    public class SearchProductModel
    {
        public string? ProductName { get; set; }

        public int? CategoryId { get; set; }

        public decimal? FromUnitPrice { get; set; } = decimal.Zero;

        public decimal? ToUnitPrice { get; set; } = null;

        public SortContent? SortContent { get; set; }

        public int _pageIndex { get; set; } = 1;

        [Range(0, int.MaxValue)]
        public int pageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value <= 0 ? 1 : value;
        }


        [Range(1, 200)]
        public int pageSize { get; set; } = 50;

    }

    public class SortContent
    {
        public SortProductByEnum? sortProductBy { get; set; }
        public SortProductTypeEnum? sortProductType { get; set; }
    }

}
