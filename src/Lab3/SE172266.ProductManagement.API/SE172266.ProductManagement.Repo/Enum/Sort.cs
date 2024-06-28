namespace SE172266.ProductManagement.API.Enum
{
    public class Sort
    {
        public enum SortProductByEnum
        {
            ProductId = 1,
            ProductName = 2,
            CategoryId = 3,
            UnitsInStock = 4,
            UnitPrice = 5,
        }

        public enum SortProductTypeEnum
        {
            Ascending = 1,
            Descending = 2,
        }
    }
}
