using System;
using System.ComponentModel.DataAnnotations.Schema;
using MeWhen.Domain.Constant;

namespace MeWhen.Domain.Model
{
    [Table("tr_tag")]
    public class Tag : BaseModel
    {
        [Column("name", TypeName = "text")]
        public required string Name { get; set; }

        [Column("age_rating")]
        public required ModelConstant.AgeRating AgeRating { get; set; }

        [NotMapped]
        [InverseProperty(nameof(ImageTag.Tag))]
        public List<ImageTag> TagsUsed { get; set; } = [];
    }
}
