using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Tests;
using NonFactors.Mvc.Grid;
using NSubstitute;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Xunit;

namespace UpsCoolWeb.Components.Extensions.Tests
{
    public class MvcGridExtensionsTests
    {
        private IGridColumnsOf<AllTypesView> columns;
        private IHtmlGrid<AllTypesView> html;

        public MvcGridExtensionsTests()
        {
            Grid<AllTypesView> grid = new Grid<AllTypesView>(new AllTypesView[0]);
            IHtmlHelper htmlHelper = HtmlHelperFactory.CreateHtmlHelper();
            html = new HtmlGrid<AllTypesView>(htmlHelper, grid);
            columns = new GridColumns<AllTypesView>(grid);
        }

        #region AddAction<T>(this IGridColumnsOf<T> columns, String action, String iconClass)

        [Fact]
        public void AddAction_Unauthorized_Empty()
        {
            IGridColumn<AllTypesView, IHtmlContent> actual = columns.AddAction("Edit", "fa fa-pencil-alt");

            Assert.Empty(actual.ValueFor(null).ToString());
            Assert.Empty(columns);
        }

        [Fact]
        public void AddAction_Authorized_Renders()
        {
            AllTypesView view = new AllTypesView();
            StringWriter writer = new StringWriter();
            LinkGenerator link = Substitute.For<LinkGenerator>();
            IAuthorization authorization = columns.Grid.ViewContext.HttpContext.RequestServices.GetService<IAuthorization>();

            authorization.IsGrantedFor(Arg.Any<Int32?>(), Arg.Any<String>(), Arg.Any<String>(), "Details").Returns(true);
            columns.Grid.ViewContext.HttpContext.RequestServices.GetService<LinkGenerator>().Returns(link);

            IGridColumn<AllTypesView, IHtmlContent> column = columns.AddAction("Details", "fa fa-info");
            column.ValueFor(new GridRow<AllTypesView>(view)).WriteTo(writer, HtmlEncoder.Default);

            String expected = "<a class=\"fa fa-info\" href=\"\"></a>";
            String actual = writer.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_NoAuthorization_Renders()
        {
            AllTypesView view = new AllTypesView();
            StringWriter writer = new StringWriter();
            LinkGenerator link = Substitute.For<LinkGenerator>();

            columns.Grid.ViewContext.HttpContext.RequestServices.GetService<IAuthorization>().Returns((IAuthorization)null);
            columns.Grid.ViewContext.HttpContext.RequestServices.GetService<LinkGenerator>().Returns(link);

            IGridColumn<AllTypesView, IHtmlContent> column = columns.AddAction("Details", "fa fa-info");
            column.ValueFor(new GridRow<AllTypesView>(view)).WriteTo(writer, HtmlEncoder.Default);

            String expected = "<a class=\"fa fa-info\" href=\"\"></a>";
            String actual = writer.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_NoId_Throws()
        {
            html.Grid.ViewContext.HttpContext.RequestServices.GetService<IAuthorization>().Returns((IAuthorization)null);
            IGridColumnsOf<Object> gridColumns = new GridColumns<Object>(new Grid<Object>(new Object[0]));
            gridColumns.Grid.ViewContext = html.Grid.ViewContext;

            IGridColumn<Object, IHtmlContent> column = gridColumns.AddAction("Delete", "fa fa-times");

            String actual = Assert.Throws<Exception>(() => column.ValueFor(new GridRow<Object>(new Object()))).Message;
            String expected = "Object type does not have an id.";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_Column()
        {
            columns.Grid.ViewContext.HttpContext.RequestServices.GetService<IAuthorization>().Returns((IAuthorization)null);

            IGridColumn<AllTypesView, IHtmlContent> actual = columns.AddAction("Edit", "fa fa-pencil-alt");

            Assert.Equal("action-cell edit", actual.CssClasses);
            Assert.Single(columns);
        }

        #endregion

        #region AddDate<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)

        [Fact]
        public void AddDate_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView, DateTime> actual = columns.AddDate(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:d}", actual.Format);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddDate<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)

        [Fact]
        public void AddDate_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView, DateTime?> actual = columns.AddDate(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:d}", actual.Format);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddBoolean<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean>> expression)

        [Fact]
        public void AddBoolean_Column()
        {
            Expression<Func<AllTypesView, Boolean>> expression = (model) => model.BooleanField;

            IGridColumn<AllTypesView, Boolean> actual = columns.AddBoolean(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.False(actual.Filter.IsMulti);
            Assert.Single(columns);
        }

        [Fact]
        public void AddBoolean_True()
        {
            IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = true });

            String actual = column.ValueFor(row).ToString();
            String expected = Resource.ForString("Yes");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBoolean_False()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = false });
            IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);

            String actual = column.ValueFor(row).ToString();
            String expected = Resource.ForString("No");

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddBoolean<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean?>> expression)

        [Fact]
        public void AddBoolean_Nullable_Column()
        {
            Expression<Func<AllTypesView, Boolean?>> expression = (model) => model.NullableBooleanField;

            IGridColumn<AllTypesView, Boolean?> actual = columns.AddBoolean(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.False(actual.Filter.IsMulti);
            Assert.Single(columns);
        }

        [Fact]
        public void AddBoolean_Nullable()
        {
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = null });

            String actual = column.ValueFor(row).ToString();

            Assert.Empty(actual);
        }

        [Fact]
        public void AddBoolean_Nullable_True()
        {
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = true });

            String actual = column.ValueFor(row).ToString();
            String expected = Resource.ForString("Yes");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBoolean_Nullable_False()
        {
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = false });

            String actual = column.ValueFor(row).ToString();
            String expected = Resource.ForString("No");

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddDateTime<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)

        [Fact]
        public void AddDateTime_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView, DateTime> actual = columns.AddDateTime(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Equal("{0:g}", actual.Format);
            Assert.Single(columns);
        }

        #endregion

        #region AddDateTime<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)

        [Fact]
        public void AddDateTime_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView, DateTime?> actual = columns.AddDateTime(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Equal("{0:g}", actual.Format);
            Assert.Single(columns);
        }

        #endregion

        #region AddProperty<T, TProperty>(this IGridColumnsOf<T> columns, Expression<Func<T, TProperty>> expression)

        [Fact]
        public void AddProperty_Column()
        {
            Expression<Func<AllTypesView, AllTypesView>> expression = (model) => model;

            IGridColumn<AllTypesView, AllTypesView> actual = columns.AddProperty(expression);

            Assert.Equal("text-left", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddProperty_SetsCssClassForEnum()
        {
            AssertCssClassFor(model => model.EnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSByte()
        {
            AssertCssClassFor(model => model.SByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForByte()
        {
            AssertCssClassFor(model => model.ByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt16()
        {
            AssertCssClassFor(model => model.Int16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt16()
        {
            AssertCssClassFor(model => model.UInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt32()
        {
            AssertCssClassFor(model => model.Int32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt32()
        {
            AssertCssClassFor(model => model.UInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt64()
        {
            AssertCssClassFor(model => model.Int64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt64()
        {
            AssertCssClassFor(model => model.UInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSingle()
        {
            AssertCssClassFor(model => model.SingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDouble()
        {
            AssertCssClassFor(model => model.DoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDecimal()
        {
            AssertCssClassFor(model => model.DecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForBoolean()
        {
            AssertCssClassFor(model => model.BooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDateTime()
        {
            AssertCssClassFor(model => model.DateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableEnum()
        {
            AssertCssClassFor(model => model.NullableEnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSByte()
        {
            AssertCssClassFor(model => model.NullableSByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableByte()
        {
            AssertCssClassFor(model => model.NullableByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt16()
        {
            AssertCssClassFor(model => model.NullableInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt16()
        {
            AssertCssClassFor(model => model.NullableUInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt32()
        {
            AssertCssClassFor(model => model.NullableInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt32()
        {
            AssertCssClassFor(model => model.NullableUInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt64()
        {
            AssertCssClassFor(model => model.NullableInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt64()
        {
            AssertCssClassFor(model => model.NullableUInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSingle()
        {
            AssertCssClassFor(model => model.NullableSingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDouble()
        {
            AssertCssClassFor(model => model.NullableDoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDecimal()
        {
            AssertCssClassFor(model => model.NullableDecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableBoolean()
        {
            AssertCssClassFor(model => model.NullableBooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDateTime()
        {
            AssertCssClassFor(model => model.NullableDateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForOtherTypes()
        {
            AssertCssClassFor(model => model.StringField, "text-left");
        }

        #endregion

        #region ApplyDefaults<T>(this IHtmlGrid<T> grid)

        [Theory]
        [InlineData("", "table-hover")]
        [InlineData(" ", "table-hover")]
        [InlineData(null, "table-hover")]
        [InlineData("test", "test table-hover")]
        [InlineData(" test", "test table-hover")]
        [InlineData("test ", "test  table-hover")]
        [InlineData(" test ", "test  table-hover")]
        public void ApplyDefaults_Values(String cssClasses, String expectedClasses)
        {
            IGridColumn column = html.Grid.Columns.Add(model => model.ByteField);
            html.Grid.Attributes["class"] = cssClasses;
            column.Filter.IsEnabled = null;
            column.Sort.IsEnabled = null;

            IGrid actual = html.ApplyDefaults().Grid;

            Assert.Equal(Resource.ForString("NoDataFound"), actual.EmptyText);
            Assert.Equal(expectedClasses, html.Grid.Attributes["class"]);
            Assert.True(column.Filter.IsEnabled);
            Assert.True(column.Sort.IsEnabled);
            Assert.NotEmpty(actual.Columns);
        }

        #endregion

        #region Test helpers

        private void AssertCssClassFor<TProperty>(Expression<Func<AllTypesView, TProperty>> property, String cssClasses)
        {
            IGridColumn<AllTypesView, TProperty> column = columns.AddProperty(property);

            String actual = column.CssClasses;
            String expected = cssClasses;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
