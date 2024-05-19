using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace MeWhenAPI.Domain.Model
{
    [Table("tr_image_tag")]
    public class ImageTagModel : BaseModel
    {
        [Column("image_id")]
        public Guid ImageID { get; set; }

        [Column("tag_id")]
        public Guid TagID { get; set; }

        [ForeignKey(nameof(TagID))]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public TagModel Tag { get; set; } = default!;

        [ForeignKey(nameof(ImageID))]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ImageModel Image { get; set; } = default!;
    }
}
