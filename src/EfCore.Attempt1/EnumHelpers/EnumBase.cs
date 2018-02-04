using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attempt1.EnumHelpers
{
    public class EnumBase<TEnum> where TEnum : struct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public virtual string Name { get; set; }

        [MaxLength(100)]
        public virtual string Description { get; set; }
    }
}
