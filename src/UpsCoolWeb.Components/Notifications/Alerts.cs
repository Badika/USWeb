using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Components.Notifications
{
    public class Alerts : List<Alert>
    {
        public void Merge(Alerts alerts)
        {
            if (alerts == this)
                return;

            AddRange(alerts);
        }

        public void AddInfo(String message, Int32 timeout = 0)
        {
            Add(new Alert { Type = AlertType.Info, Message = message, Timeout = timeout });
        }
        public void AddError(String message, Int32 timeout = 0)
        {
            Add(new Alert { Type = AlertType.Danger, Message = message, Timeout = timeout });
        }
        public void AddSuccess(String message, Int32 timeout = 0)
        {
            Add(new Alert { Type = AlertType.Success, Message = message, Timeout = timeout });
        }
        public void AddWarning(String message, Int32 timeout = 0)
        {
            Add(new Alert { Type = AlertType.Warning, Message = message, Timeout = timeout });
        }
    }
}
