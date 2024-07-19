using System;
using System.Collections.Generic;

namespace SE172266.BookStoreOData.API.Entities
{
    public partial class Address
    {
        public Address()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }
}
