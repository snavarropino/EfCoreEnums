using Microsoft.EntityFrameworkCore;

namespace Attempt1.Model
{    public class StudentDbContext: DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options): base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
