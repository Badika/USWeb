using UpsCoolWeb.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Tests
{
    public class TestModel : BaseModel
    {
        [StringLength(128)]
        public String Title { get; set; }
    }
}
