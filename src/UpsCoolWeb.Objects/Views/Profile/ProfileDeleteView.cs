using UpsCoolWeb.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Objects
{
    public class ProfileDeleteView : BaseView
    {
        [Required]
        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }
    }
}
