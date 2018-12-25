using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class AccountView : BaseView
    {
        [Required]
        [StringLength(32)]
        public String Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }

        public Boolean IsLocked { get; set; }

        public String RoleTitle { get; set; }
    }
}
