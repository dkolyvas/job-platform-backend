namespace JobPlatform.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        public  Task<T?> FindById(long id);

        public Task<IEnumerable<T>> FindAll();

        public Task<T> AddOne(T entity);

        public Task<T?> UpdateOne(T entity, long id);

        public Task<IEnumerable<T>> AddRange(IEnumerable<T> entities);

        public  Task<bool> Delete(long id);
    }
}
