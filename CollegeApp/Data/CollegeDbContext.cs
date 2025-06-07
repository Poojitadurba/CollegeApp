using CollegeApp.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Data
{
    public class CollegeDbContext : DbContext
    {
        public CollegeDbContext(DbContextOptions<CollegeDbContext> options):base(options)
        {
            
        }
        public DbSet<Student> students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Student>().HasData(new List<Student>()
            //{
            //    new Student{Id=1,StudentName="Poojita",Address="Hyderabad",Email="abc@gmail.com"},
            //    new Student{Id=2,StudentName="Keerthi Kalyan",Address="Coimbatore",Email="xyz@gmail.com"}
            //});

            //modelBuilder.Entity<Student>(entity =>
            //{
            //    entity.Property(n => n.StudentName).IsRequired();
            //    entity.Property(n => n.StudentName).HasMaxLength(250);
            //    entity.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
            //    entity.Property(n => n.Email).IsRequired().HasMaxLength(250);
            //});
            modelBuilder.ApplyConfiguration(new StudentConfig());
        }
    }
}

