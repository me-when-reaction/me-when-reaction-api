using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeWhenAPI.Domain.Constant;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace MeWhenAPI.Domain.Model
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

        [Column("extension", TypeName = "text")]
        public required string Extension { get; set; }

        [Column("age_rating")]
        public required ModelConstant.AgeRating AgeRating { get; set; }

        [InverseProperty("Image")]
        public List<ImageTagModel> Tags { get; set; } = [];
    }
}
