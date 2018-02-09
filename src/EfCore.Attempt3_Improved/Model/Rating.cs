using System.ComponentModel;
using EfCore.Attempt3_Improved.EnumHelpers;

namespace EfCore.Attempt3_Improved.Model
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
    
    public class Rating: EnumBase<RatingEnum>
    {
    }
}