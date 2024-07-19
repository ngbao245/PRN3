using System;
using System.Collections.Generic;

namespace SE172266.BookStoreOData.API.Entities
{
    public partial class Book
    {
        public int Id { get; set; }
        public string Isbn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal Price { get; set; }
        public int? LocationId { get; set; }
        public int? PressId { get; set; }

        public virtual Address? Location { get; set; }
        public virtual Press? Press { get; set; }
    }
}
