using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class AuditLog : BaseModel
    {
        public Int32? AccountId { get; set; }

        [Required]
        [StringLength(16)]
        public String Action { get; set; }

        [Required]
        [StringLength(64)]
        public String EntityName { get; set; }

        public Int32 EntityId { get; set; }

        [Required]
        public String Changes { get; set; }
    }
}
