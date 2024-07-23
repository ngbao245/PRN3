using SE172266.ProductManagement.Repo.Entities;

namespace SE172266.BookStoreOData.API.Model
{
    public class BookUpdateModel
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public Address Location { get; set; }
        public int? PressId { get; set; }
    }
}
