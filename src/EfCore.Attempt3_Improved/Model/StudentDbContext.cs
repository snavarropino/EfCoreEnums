using Microsoft.EntityFrameworkCore;

namespace EfCore.Attempt3_Improved.Model
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
                .Ignore(s => s.Rating)      // Ignore enum property
                .Property<int>("_ratingId") // Define backing field with no property
                .HasColumnName("RatingId")  // Set proper column name for foreign key
                .IsRequired();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.RatingCatalogue)
                .WithMany()
                .HasForeignKey("_ratingId") // Set foreign key to backing field
                .IsRequired();
        }
    }
}