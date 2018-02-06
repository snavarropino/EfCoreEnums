using System.ComponentModel.DataAnnotations.Schema;

namespace EfCore.Attempt1.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
    }
}
