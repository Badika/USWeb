using UpsCoolWeb.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class AccountLoginView : BaseView
    {
        [Required]
        [StringLength(32)]
        public String Username { get; set; }

        [Required]
        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }
    }
}
