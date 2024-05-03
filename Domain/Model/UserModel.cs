using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeWhen.Domain.Model
{
    [Table("ms_user")]
    public class UserModel : BaseModel
    {
        [Column("name", TypeName = "text")]
        public required string Name { get; set; }

        [Column("email", TypeName = "text")]
        public required string Email { get; set; }

        [Column("password", TypeName = "text")]
        public required string Password { get; set; }

        [Column("identity_id", TypeName = "text")]
        public required string IdentityID { get; set; }

        [Column("identity_provider", TypeName = "text")]
        public required string IdentityProvider { get; set; }
    }
}
