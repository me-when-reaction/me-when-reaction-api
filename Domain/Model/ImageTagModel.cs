using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace MeWhen.Domain.Model
{
    [Table("tr_image_tag")]
    public class ImageTagModel : BaseModel
    {
        [Column("image_id")]
        public Guid ImageID { get; set; }

        [Column("tag_id")]
        public Guid TagID { get; set; }

        [ForeignKey(nameof(TagID))]
        public TagModel Tag { get; set; } = default!;

        [ForeignKey(nameof(ImageID))]
        public ImageModel Image { get; set; } = default!;
    }
}
