using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SE172266.ProductManagement.Repo.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public int UnitsInStock { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
