using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using EfCore.Attempt3.EnumHelpers;

namespace EfCore.Attempt3.Model
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