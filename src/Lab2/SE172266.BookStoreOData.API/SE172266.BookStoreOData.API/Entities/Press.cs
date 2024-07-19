using System;
using System.Collections.Generic;

namespace SE172266.BookStoreOData.API.Entities
{
    public partial class Press
    {
        public Press()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Book> Books { get; set; }
    }
}
