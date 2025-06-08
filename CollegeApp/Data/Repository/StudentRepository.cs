
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CollegeApp.Data.Repository
{
    public class StudentRepository : IStudentRepository
    {
        CollegeDbContext _dbContext;
        public StudentRepository(CollegeDbContext dbContext)
        {
            _dbContext = dbContext;   
        }
        public async Task<int> CreateAsync(Student student)
        {
            _dbContext.students.Add(student);
            await _dbContext.SaveChangesAsync();
            return student.Id;
        }

        public async Task<bool> DeleteAsync(Student student)
        {
            _dbContext.students.Remove(student);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetAllAsync()
        {
           return await _dbContext.students.ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id,bool useNoTracking=false)
        {
            if(useNoTracking)
            return await _dbContext.students.AsNoTracking().Where(n => n.Id == id).FirstOrDefaultAsync();
            else
            return await _dbContext.students.Where(n => n.Id == id).FirstOrDefaultAsync();

        }

        public async Task<Student> GetByNameAsync(string name)
        {
            return await _dbContext.students.Where(n => n.StudentName.ToLower().Contains(name.ToLower())).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(Student student)
        {
           _dbContext.Update(student);
           await  _dbContext.SaveChangesAsync();
            return student.Id;
        }
    }
}
