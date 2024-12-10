using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Region
{
    public class RegionInsertDTO
    {
        [Required(ErrorMessage = "You must provide a name")]
        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string? Name { get; set; }
    }
}
