namespace CollegeApp.Data.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student> GetByIdAsync(int id,bool useNoTracking);
        Task<Student> GetByNameAsync(string name);
        Task<int> CreateAsync(Student student);
        Task<int> UpdateAsync(Student student);
        Task<bool> DeleteAsync(Student student);

    }
}
