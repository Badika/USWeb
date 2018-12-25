using Microsoft.AspNetCore.Http;
using UpsCoolWeb.Resources;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class AcceptFilesAttributeTests
    {
        private AcceptFilesAttribute attribute;

        public AcceptFilesAttributeTests()
        {
            attribute = new AcceptFilesAttribute(".docx,.xlsx");
        }

        #region AcceptFilesAttribute(String extensions)

        [Fact]
        public void AcceptFilesAttribute_SetsExtensions()
        {
            String actual = new AcceptFilesAttribute(".docx,.xlsx").Extensions;
            String expected = ".docx,.xlsx";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region FormatErrorMessage(String name)

        [Fact]
        public void FormatErrorMessage_ForProperty()
        {
            attribute = new AcceptFilesAttribute(".docx,.xlsx");

            String expected = Validation.For("AcceptFiles", "File", attribute.Extensions);
            String actual = attribute.FormatErrorMessage("File");

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsValid(Object value)

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Fact]
        public void IsValid_NotFileReturnsFalse()
        {
            Assert.False(attribute.IsValid("100"));
        }

        [Fact]
        public void IsValid_FileWithoutNameReturnsFalse()
        {
            IFormFile file = Substitute.For<IFormFile>();
            file.FileName.ReturnsNull();

            Assert.False(attribute.IsValid(file));
        }

        [Fact]
        public void IsValid_AnyFileWithoutNameReturnsFalse()
        {
            IFormFile[] files = { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() };
            files[0].FileName.Returns("File.docx");
            files[1].FileName.ReturnsNull();

            Assert.False(attribute.IsValid(files));
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData(".doc")]
        [InlineData("docx")]
        [InlineData(".docx.doc")]
        public void IsValid_DifferentExtensionReturnsFalse(String fileName)
        {
            IFormFile file = Substitute.For<IFormFile>();
            file.FileName.Returns(fileName);

            Assert.False(attribute.IsValid(file));
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData(".doc")]
        [InlineData("docx")]
        [InlineData(".docx.doc")]
        public void IsValid_DifferentExtensionsReturnsFalse(String fileName)
        {
            IFormFile[] files = { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() };
            files[0].FileName.Returns("File.docx");
            files[1].FileName.Returns(fileName);

            Assert.False(attribute.IsValid(files));
        }

        [Theory]
        [InlineData(".docx")]
        [InlineData(".xlsx")]
        [InlineData("docx.docx")]
        [InlineData("docx..docx")]
        [InlineData("xlsx.doc.xlsx")]
        public void IsValid_Extension(String fileName)
        {
            IFormFile file = Substitute.For<IFormFile>();
            file.FileName.Returns(fileName);

            Assert.True(attribute.IsValid(file));
        }

        [Theory]
        [InlineData("docx.docx", ".docx")]
        [InlineData("docx..docx", ".xlsx")]
        [InlineData(".xlsx", "docx..docx")]
        [InlineData(".docx", "xlsx.doc.xlsx")]
        [InlineData("xlsx.doc.xlsx", ".docx.docx")]
        public void IsValid_Exntesions(String firstFileName, String secondFileName)
        {
            IFormFile[] files = { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() };
            files[1].FileName.Returns(secondFileName);
            files[0].FileName.Returns(firstFileName);

            Assert.True(attribute.IsValid(files));
        }

        #endregion
    }
}
