using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class Permission : BaseModel
    {
        [StringLength(64)]
        public String Area { get; set; }

        [Required]
        [StringLength(64)]
        public String Controller { get; set; }

        [Required]
        [StringLength(64)]
        public String Action { get; set; }
    }
}
