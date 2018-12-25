using UpsCoolWeb.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class ProfileEditView : BaseView
    {
        [Required]
        [StringLength(32)]
        public String Username { get; set; }

        [Required]
        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }

        [NotTrimmed]
        [StringLength(32)]
        public String NewPassword { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }
    }
}
