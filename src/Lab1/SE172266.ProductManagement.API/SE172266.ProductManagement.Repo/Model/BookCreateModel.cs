namespace SE172266.BookStoreOData.API.Model
{
    public class BookCreateModel
    {
        public string Isbn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal Price { get; set; }
        public int? LocationId { get; set; }
        public int? PressId { get; set; }
    }
}
