using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeApp.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("students");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.StudentName).IsRequired();
            builder.Property(n => n.StudentName).HasMaxLength(250);
            builder.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
            builder.Property(n => n.Email).IsRequired().HasMaxLength(250);
            builder.HasData(new List<Student>()
            {
                new Student{Id=1,StudentName="Poojita",Address="Hyderabad",Email="abc@gmail.com"},
                new Student{Id=2,StudentName="Keerthi Kalyan",Address="Coimbatore",Email="xyz@gmail.com"}
            });
        }
    }
}
