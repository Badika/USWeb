using System;

namespace UpsCoolWeb.Objects
{
    public class RolePermission : BaseModel
    {
        public Int32 RoleId { get; set; }
        public virtual Role Role { get; set; }

        public Int32 PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
