namespace EfCore.Attempt3_Improved.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public RatingEnum Rating //Improved Name
        {
            get => (RatingEnum)_ratingId;
            set => _ratingId = (int)value;
        }

        private int _ratingId;
        public Rating RatingCatalogue { get; set; }
    }
}