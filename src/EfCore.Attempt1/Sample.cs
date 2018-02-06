using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using EfCore.Attempt1.EnumHelpers;
using EfCore.Attempt1.Model;

namespace EfCore.Attempt3
{
    public class Sample
    {
        public static void Run()
        {
            Log.Information("Starting attempt 1...");
            CreateAndSeed();

            AddStudentPepe();
            ReadStudents();
            ReadStudentsWithInclude();

            UpdateStudentPepe();
            ReadStudents();
            ReadStudentsWithInclude();
        }

        private static void AddStudentPepe()
        {
            using (var context = GetDbContext())
            {
                var pepe = new Student()
                {
                    Name = "Pepe",
                    RatingId = (int)RatingEnum.Bad
                };

                context.Students.Add(pepe);
                context.SaveChanges();
                Log.Information("Student added: {@student}", pepe);
            }
        }

        private static void UpdateStudentPepe()
        {
            using (var context = GetDbContext())
            {
                var pepe = context.Students.First();
                pepe.RatingId = (int)RatingEnum.Brilliant;
                context.SaveChanges();
                Log.Information("Student updated to brilliant");
            }
        }

        private static void ReadStudents()
        {
            using (var context = GetDbContext())
            {
                foreach (var student in context.Students.ToList())
                {
                    Log.Information("Student readed:{@student}", student);
                }
            }
        }

        private static void ReadStudentsWithInclude()
        {
            using (var context = GetDbContext())
            {
                var students = context.Students
                    .Include(s => s.Rating)
                    .ToList();
                foreach (var student in students)
                {
                    Log.Information("Student readed with include:{@student}", student);
                }
            }
        }

        private static void CreateAndSeed()
        {
            using (var context = GetDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Log.Information("Database deleted and created again...");
                Seeder.SeedEnumData<Rating, RatingEnum>(context.Ratings);
                context.SaveChanges();
                Log.Information("Database seeded");
            }
        }

        static StudentDbContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<StudentDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EnumsAttempt1;Trusted_Connection=True;");

            return new StudentDbContext(optionsBuilder.Options);
        }
    }
}
