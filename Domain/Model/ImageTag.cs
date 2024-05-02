using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace MeWhen.Domain.Model
{
    [Table("tr_image_tag")]
    public class ImageTag : BaseModel
    {
        [Column("image_id")]
        public Guid ImageID { get; set; }
        
        [Column("tag_id")]
        public Guid TagID { get; set; }

        [NotMapped]
        [ForeignKey(nameof(TagID))]
        public Tag Tag { get; set; } = default!;

        [NotMapped]
        [ForeignKey(nameof(ImageID))]
        public Image Image { get; set; } = default!;
    }
}
