using Attempt1.EnumHelpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attempt1.Model
{
    public enum RatingEnum
    {
        [Description("Something really good")]
        Brilliant = 1,
        Good = 2,
        Average = 3,
        Bad = 4,
        [Description("Something really bad")]
        Terrible = 5,
        Jarl=6
    }

    [Table("Ratings")]
    public class Rating: EnumBase<RatingEnum>
    {
    }
}
