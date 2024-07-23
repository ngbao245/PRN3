using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SE172266.ProductManagement.Repo.Entities
{
    public class Press
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
    }
}
