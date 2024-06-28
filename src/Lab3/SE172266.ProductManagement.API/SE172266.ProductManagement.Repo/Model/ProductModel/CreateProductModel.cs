using System.ComponentModel.DataAnnotations;

namespace SE172266.ProductManagement.API.Model.ProductModel
{
    public class CreateProductModel
    {
        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        [Range(0, int.MaxValue)]
        public int UnitsInStock { get; set; }

        [Range(0, int.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
