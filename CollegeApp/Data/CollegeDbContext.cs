using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Data
{
    public class CollegeDbContext : DbContext
    {
        DbSet<Student> students { get; set; }
    }
}

//Data Source=DESKTOP-PG9JRCA\SQLEXPRESS05;Initial Catalog=DemoDB;Integrated Security=True;Trust Server Certificate=True