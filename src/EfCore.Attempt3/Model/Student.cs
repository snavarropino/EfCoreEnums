namespace EfCore.Attempt3.Model
{
    public class Student
    {
        
        public int Id { get; set; }
        public string Name { get; set; }


        public RatingEnum RatingId
        {
            get => (RatingEnum)_ratingId;
            set => _ratingId = (int)value;
        }

        private int _ratingId; 

        public Rating Rating { get; set; }
    }
}
