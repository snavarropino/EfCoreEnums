using System.ComponentModel.DataAnnotations.Schema;

namespace Attempt1.Model
{

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingId { get; set; }
        
        [ForeignKey("RatingId")]
        public Rating Rating { get; set; }
    }
}
