using Microsoft.EntityFrameworkCore;

namespace EfCore.Attempt3.Model
{
    public class StudentDbContext: DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options): base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Ignore(s => s.RatingId) //Ignore enum
                .Property<int>("_ratingId"); //Define backing field with no property

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Rating)
                .WithMany()
                .HasForeignKey("_ratingId"); //Set FK to backing field
        }
    }
}