using System;
using System.ComponentModel.DataAnnotations.Schema;
using MeWhenAPI.Domain.Constant;

namespace MeWhenAPI.Domain.Model
{
    [Table("tr_tag")]
    public class TagModel : BaseModel
    {
        [Column("name", TypeName = "text")]
        public required string Name { get; set; }

        [Column("age_rating")]
        public required ModelConstant.AgeRating AgeRating { get; set; }

        [InverseProperty("Tag")]
        public List<ImageTagModel> TagsUsed { get; set; } = [];
    }
}
