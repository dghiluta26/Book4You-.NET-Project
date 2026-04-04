using System.ComponentModel.DataAnnotations;

namespace Project.Models

{
    public class Amenity
    {
        public int Id  { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
    }
}
