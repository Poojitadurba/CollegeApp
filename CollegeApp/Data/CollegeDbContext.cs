using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Data
{
    public class CollegeDbContext : DbContext
    {
        public CollegeDbContext(DbContextOptions<CollegeDbContext> options):base(options)
        {
            
        }
        DbSet<Student> students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(new List<Student>()
            {
                new Student{Id=1,StudentName="Poojita",Address="Hyderabad",Email="abc@gmail.com"},
                new Student{Id=2,StudentName="Keerthi Kalyan",Address="Coimbatore",Email="xyz@gmail.com"}
            });
        }
    }
}

