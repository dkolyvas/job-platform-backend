namespace JobPlatform.DTO.Applicant.ApplicantSkills
{
    public class ApplicantSkillViewDTO
    {
        public long Id { get; set; }

        //    public long ApplicantId { get; set; }

        

        public long SkillSubcategoryId { get; set; }
        public string? SkillSubcategoryName { get; set; } 
        
        public long SkillCategoryId { get; set; }
        public string? SkillCategoryName {  get; set; }


        public int? SkillLevelId { get; set; }
        public string? SkillLevelName { get; set; }
     //   public int? SkillGrade {  get; set; }

        public string? Institution { get; set; }

        public string? Description { get; set; }

        public DateOnly? DateFrom { get; set; }

        public DateOnly? DateTo { get; set; }

        public int? DurationMonths { get; set; }
    }
}
