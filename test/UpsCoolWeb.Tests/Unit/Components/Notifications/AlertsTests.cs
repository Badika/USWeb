using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Components.Notifications.Tests
{
    public class AlertsTests
    {
        private Alerts alerts;

        public AlertsTests()
        {
            alerts = new Alerts();
        }

        #region Merge(Alerts alerts)

        [Fact]
        public void Merge_DoesNotMergeItself()
        {
            alerts.Add(new Alert());
            IEnumerable<Alert> original = alerts.ToArray();

            alerts.Merge(alerts);

            IEnumerable<Alert> expected = alerts;
            IEnumerable<Alert> actual = original;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Merge_Alerts()
        {
            Alerts part = new Alerts();
            part.AddError("SecondError");
            alerts.AddError("FirstError");

            IEnumerable<Alert> expected = alerts.Union(part);
            IEnumerable<Alert> actual = alerts;
            alerts.Merge(part);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddInfo(String message, Int32 timeout = 0)

        [Fact]
        public void AddInfo_Message()
        {
            alerts.AddInfo("Message", 1);

            Alert actual = alerts.Single();

            Assert.Equal(AlertType.Info, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddError(String message, Int32 timeout = 0)

        [Fact]
        public void AddError_Message()
        {
            alerts.AddError("Message", 1);

            Alert actual = alerts.Single();

            Assert.Equal(AlertType.Danger, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddSuccess(String message, Int32 timeout = 0)

        [Fact]
        public void AddSuccess_Message()
        {
            alerts.AddSuccess("Message", 1);

            Alert actual = alerts.Single();

            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddWarning(String message, Int32 timeout = 0)

        [Fact]
        public void AddWarning_Message()
        {
            alerts.AddWarning("Message", 1);

            Alert actual = alerts.Single();

            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion
    }
}
