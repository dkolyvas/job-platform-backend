using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Applicant.ApplicantSkills
{
    public class ApplicantSkillInsertDTO
    {

        //public long ApplicantId { get; set; }

        [Required(ErrorMessage = "You must provide a valid subcategory for all skills")]
        public long SkillSubcategoryId { get; set; }


        public int? SkillLevelId { get; set; }


        [StringLength(100, ErrorMessage = "The field 'institution' cannot exceed 100 characters")]
        public string? Institution { get; set; }

        [StringLength(255, ErrorMessage = "The field skill descriptions cannot exceed 255 characters")]
        public string? Description { get; set; }

        [DataType(DataType.Date, ErrorMessage = "You must provide a valid start date for all skills or none at all")]
        public DateOnly? DateFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "You must provide a valid end date for all skills or none at all")]
        public DateOnly? DateTo { get; set; }

        



    }
}
