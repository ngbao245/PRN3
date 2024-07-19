using System;
using System.Collections.Generic;

namespace SE172266.BookStoreOData.API.Entities
{
    public partial class Category
    {
        public Category()
        {
            Presses = new HashSet<Press>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Press> Presses { get; set; }
    }
}
