using UpsCoolWeb.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Tests
{
    public class TestView : BaseView
    {
        [StringLength(128)]
        public String Title { get; set; }
    }
}
