using JobPlatform.Data;
using JobPlatform.DTO.SearchCriteria;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class VacancyRepository : BaseRepository<Vacancy>

    {
        public VacancyRepository(JobplatformContext db) : base(db)
        { 
        }

        public async Task<IEnumerable<Vacancy>> FindVacanies(VacancySearchCriteria searchCriteria)
        {
            /**
             * The query filters the vacancies
             * a) that they are active
             * b) that they are for one of the specified regions
             * c) that they require one of the specified skill subcategories or in absence of them the specified skill categroies
             * for at most the specified duration and at most the specified skill level assessed by its grade
             * d) that have the specified scope defined by a skill subcategory or in absence of it a skill category
             * */
            var query = _db.Vacancies.Include(v => v.Business).Include(v => v.Region).Include(v => v.SkillSubcategory).AsQueryable();
            query = query.Where(v => v.Active != null && v.Active == true);
            query = query.Where(v => searchCriteria.Regions.Contains(v.Region.Id));
            foreach(var skill in searchCriteria.SkillSearchCriteria)
            {               
                    query = query.Where(v => v.VacancySkills.Any(vs => 
                    (vs.SkillSubcategory != null && skill.SubcategoryId != null &&
                    (vs.SkillSubcategory.Id == skill.SubcategoryId) && (vs.Duration <= skill.SkillDuration|| vs.Duration==null) &&
                    (vs.SkillLevel == null ||vs.SkillLevel.Grade== null|| vs.SkillLevel.Grade <= skill.SkillLevelGrade)
                 )
                    ||
                    (vs.SkillSubcategory != null && skill.SubcategoryId == null &&
                    (vs.SkillSubcategory.SkillCategoryId == skill.CategoryId) && (vs.Duration <= skill.SkillDuration || vs.Duration == null) &&
                    (vs.SkillLevel == null || vs.SkillLevel.Grade == null || vs.SkillLevel.Grade <= skill.SkillLevelGrade)
                    )

                    || (vs.SkillSubcategory == null && vs.SkillCategory !=null &&
                    vs.SkillCategory.Id == skill.CategoryId && (vs.Duration <= skill.SkillDuration ||vs.Duration ==null) &&
                    (vs.SkillLevel == null ||vs.SkillLevel.Grade == null || vs.SkillLevel.Grade <= skill.SkillLevelGrade)
                    )));
                
            }
            if(searchCriteria.JobSubCategory != null)
            {
               query =  query.Where(v => v.SkillSubcategory.Id == searchCriteria.JobSubCategory);
            }
            else if(searchCriteria.JobCategory != null)
            {
                query = query.Where(v => v.SkillSubcategory.SkillCategoryId == searchCriteria.JobCategory);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Vacancy>> GetBusinessVacancies(long businessId)
        {
            return await _db.Vacancies.Where(v => v.BusinessId == businessId).ToListAsync();
        }

        public async Task<int> FindAnnouncementsSinceDate(DateOnly? dateSince)
        {
            return await _db.Vacancies.Where(v => v.PublicationDate >= dateSince).Select(v =>v.Id).CountAsync();
        }

        public async Task<Vacancy?> UpdateVacancy(Vacancy vacancy)
        {
            var dbValue = await _db.Vacancies.FindAsync(vacancy.Id);
            if (dbValue is not null)
            {
                if(dbValue.BusinessId != vacancy.BusinessId) throw new AccessNotAllowedException();
                vacancy.PublicationDate = dbValue.PublicationDate;
                vacancy.Active = dbValue.Active;
                _table.Entry(dbValue).CurrentValues.SetValues(vacancy);
                _table.Entry(dbValue).State = EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<Vacancy?> DisactivateVacancy(long id)
        {
            var vacancy = await _db.Vacancies.FindAsync(id);
            if(vacancy is not null)
            {
                vacancy.Active = false;
                _table.Entry(vacancy).State = EntityState.Modified;
            }
            return vacancy;
        }
        public async Task<Vacancy?> DisactivateVacancy(long id, long businessId)
        {
            var vacancy = await _db.Vacancies.FindAsync(id);
            if (vacancy is not null)
            {
                if (vacancy.BusinessId != businessId) throw new AccessNotAllowedException();
                vacancy.Active = false;
                _table.Entry(vacancy).State = EntityState.Modified;
            }
            return vacancy;
        }

        public override async Task<bool> Delete(long id)
        {
            var vacancy =await  _db.Vacancies.FindAsync(id);
            if(vacancy is null)  return false;
            _db.VacancySkills.RemoveRange(vacancy.VacancySkills);
            _db.VacancyMerits.RemoveRange(vacancy.VacancyMerits);
            _db.Vacancies.Remove(vacancy);
            return true;

        }

        
    }
}
