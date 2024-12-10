using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Region
{
    public class RegionUpdateDTO
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string? Name { get; set; }
    }
}
