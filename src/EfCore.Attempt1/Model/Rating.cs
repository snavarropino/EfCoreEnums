using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using EfCore.Attempt1.EnumHelpers;

namespace EfCore.Attempt1.Model
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
    }

    public class Rating: EnumBase<RatingEnum>
    {
    }
}
