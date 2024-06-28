using SE172266.ProductManagement.Repo.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SE172266.ProductManagement.Repo.Entities
{
    [Table(nameof(Product))]
    public class Product : ISoftDelete
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required, StringLength(40)]
        public string ProductName { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int UnitsInStock { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }


        [ForeignKey(nameof(CategoryId))]
        [JsonIgnore]
        public Category Category { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
