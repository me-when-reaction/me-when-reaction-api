using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeWhen.Domain.Model
{
    public class BaseModel
    {
        [Key]
        [Column("id")]
        public Guid ID { get; set; }

        [Column("date_in")]
        public DateTime DateIn { get; set; } = DateTime.Now;

        [Column("date_up")]
        public DateTime? DateUp { get; set; }

        [Column("date_del")]
        public DateTime? DateDel { get; set; }

        [Column("user_in")]
        public Guid UserIn { get; set; }

        [Column("user_up")]
        public Guid? UserUp { get; set; }

        [Column("user_del")]
        public Guid? UserDel { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; } = false;
    }
}
