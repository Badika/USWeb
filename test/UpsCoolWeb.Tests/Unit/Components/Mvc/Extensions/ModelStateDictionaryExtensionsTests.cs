using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class ModelStateDictionaryExtensionsTests
    {
        #region Errors(this ModelStateDictionary modelState)

        [Fact]
        public void Errors_FromModelState()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError("WhitespaceErrors", "           ");
            modelState.AddModelError("WhitespaceErrors", "Whitespace");
            modelState.AddModelError("TwoErrors", "Error1");
            modelState.AddModelError("TwoErrors", "Error2");
            modelState.AddModelError("EmptyErrors", "");
            modelState.AddModelError("EmptyErrors", "E");
            modelState.AddModelError("Error", "Error");
            modelState.AddModelError("Empty", "");

            Dictionary<String, String> actual = modelState.Errors();

            Assert.Equal("           ", actual["WhitespaceErrors"]);
            Assert.Equal("Error1", actual["TwoErrors"]);
            Assert.Equal("E", actual["EmptyErrors"]);
            Assert.Equal("Error", actual["Error"]);
            Assert.Equal(5, actual.Count);
            Assert.Null(actual["Empty"]);
        }

        #endregion
    }
}
