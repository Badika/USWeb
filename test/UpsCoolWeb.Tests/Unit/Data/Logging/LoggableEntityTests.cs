using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Tests;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace UpsCoolWeb.Data.Logging.Tests
{
    public class LoggableEntityTests : IDisposable
    {
        private EntityEntry<BaseModel> entry;
        private TestingContext context;
        private TestModel model;

        public LoggableEntityTests()
        {
            using (context = new TestingContext())
            {
                context.Add(model = ObjectsFactory.CreateTestModel());
                context.SaveChanges();
            }

            context = new TestingContext(context.DatabaseName);
            entry = context.Entry<BaseModel>(model);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        #region LoggableEntity(EntityEntry<BaseModel> entry)

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAddedEntity()
        {
            entry.State = EntityState.Added;

            AsssertProperties(entry.CurrentValues);
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForModifiedEntity()
        {
            String title = model.Title;
            entry.State = EntityState.Modified;
            entry.CurrentValues["Title"] = "Role";
            entry.OriginalValues["Title"] = "Role";

            LoggableProperty expected = new LoggableProperty(entry.Property("Title"), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAttachedEntity()
        {
            context.Dispose();
            String title = model.Title;

            context = new TestingContext(context.DatabaseName);
            context.Set<TestModel>().Attach(model);

            entry = context.Entry<BaseModel>(model);
            entry.OriginalValues["Title"] = "Role";
            entry.CurrentValues["Title"] = "Role";
            entry.State = EntityState.Modified;

            LoggableProperty expected = new LoggableProperty(entry.Property("Title"), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForDeletedEntity()
        {
            entry.State = EntityState.Deleted;

            AsssertProperties(entry.GetDatabaseValues());
        }

        [Fact]
        public void LoggableEntity_SetsAction()
        {
            entry.State = EntityState.Deleted;

            String actual = new LoggableEntity(entry).Action;
            String expected = entry.State.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityTypeName()
        {
            String actual = new LoggableEntity(entry).Name;
            String expected = typeof(TestModel).Name;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityId()
        {
            Int32 actual = new LoggableEntity(entry).Id();
            Int32 expected = model.Id;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region ToString()

        [Fact]
        public void ToString_FormsEntityChanges()
        {
            StringBuilder changes = new StringBuilder();
            LoggableEntity loggableEntity = new LoggableEntity(entry);
            foreach (LoggableProperty property in loggableEntity.Properties)
                changes.AppendFormat("{0}{1}", property, Environment.NewLine);

            String actual = loggableEntity.ToString();
            String expected = changes.ToString();

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Test helpers

        private void AsssertProperties(PropertyValues newValues)
        {
            LoggableProperty[] actual = new LoggableEntity(entry).Properties.ToArray();
            LoggableProperty[] expected = newValues.Properties.Where(property => property.Name != "Id")
                .Select(property => new LoggableProperty(entry.Property(property.Name), newValues[property])).ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].IsModified, actual[i].IsModified);
                Assert.Equal(expected[i].ToString(), actual[i].ToString());
            }
        }

        #endregion
    }
}
