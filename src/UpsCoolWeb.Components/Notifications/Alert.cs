using System;

namespace UpsCoolWeb.Components.Notifications
{
    public class Alert
    {
        public String Id { get; set; }
        public Int32 Timeout { get; set; }
        public AlertType Type { get; set; }
        public String Message { get; set; }
    }
}
