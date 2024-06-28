namespace SE172266.ProductManagement.API.Model.ProductModel
{
    public class CreateProductModel
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int UnitsInStock { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
