using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeWhen.Domain.Constant;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace MeWhen.Domain.Model
{
    [Table("tr_image")]
    public class ImageModel : BaseModel
    {
        [Column("name", TypeName = "text")]
        public required string Name { get; set; }

        [Column("link", TypeName = "text")]
        public required string Link { get; set; }

        [Column("description", TypeName = "text")]
        public required string Description { get; set; }

        [Column("source", TypeName = "text")]
        public required string Source { get; set; }

        [Column("upload_date")]
        public required DateTime UploadDate { get; set; }

        [Column("age_rating")]
        public required ModelConstant.AgeRating AgeRating { get; set; }

        [NotMapped]
        [InverseProperty(nameof(ImageTagModel.Image))]
        public List<ImageTagModel> Tags { get; set; } = [];
    }
}
