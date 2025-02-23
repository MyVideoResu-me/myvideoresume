using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Server.Controllers;
using Xunit;

namespace MyVideoResume.Tests
{
    public class ExportControllerTests
    {
        private readonly ExportController _controller;

        public ExportControllerTests()
        {
            _controller = new ExportController();
        }

        [Fact]
        public void ToCSV_ShouldReturnCSVFileStreamResult()
        {
            // Arrange
            var data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            }.AsQueryable();

            // Act
            var result = _controller.ToCSV(data, "test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.csv", result.FileDownloadName);
            Assert.Equal("text/csv", result.ContentType);
        }

        [Fact]
        public void ToExcel_ShouldReturnExcelFileStreamResult()
        {
            // Arrange
            var data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            }.AsQueryable();

            // Act
            var result = _controller.ToExcel(data, "test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.xlsx", result.FileDownloadName);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }

        [Fact]
        public void GetValue_ShouldReturnPropertyValue()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "Test1" };

            // Act
            var result = ExportController.GetValue(entity, "Name");

            // Assert
            Assert.Equal("Test1", result);
        }

        [Fact]
        public void GetProperties_ShouldReturnProperties()
        {
            // Act
            var result = ExportController.GetProperties(typeof(TestEntity));

            // Assert
            Assert.Contains(result, p => p.Key == "Id" && p.Value == typeof(int));
            Assert.Contains(result, p => p.Key == "Name" && p.Value == typeof(string));
        }

        [Fact]
        public void IsSimpleType_ShouldReturnTrueForSimpleTypes()
        {
            // Assert
            Assert.True(ExportController.IsSimpleType(typeof(int)));
            Assert.True(ExportController.IsSimpleType(typeof(string)));
            Assert.True(ExportController.IsSimpleType(typeof(DateTime)));
            Assert.True(ExportController.IsSimpleType(typeof(Guid)));
        }

        [Fact]
        public void IsSimpleType_ShouldReturnFalseForComplexTypes()
        {
            // Assert
            Assert.False(ExportController.IsSimpleType(typeof(TestEntity)));
        }

        [Fact]
        public void ApplyQuery_ShouldHandleNullQuery()
        {
            // Arrange
            var data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            }.AsQueryable();

            // Act
            var result = _controller.ApplyQuery(data, null);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyQuery_ShouldHandleEmptyQuery()
        {
            // Arrange
            var data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            }.AsQueryable();

            var queryCollection = new QueryCollection();

            // Act
            var result = _controller.ApplyQuery(data, queryCollection);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ToCSV_ShouldHandleEmptyData()
        {
            // Arrange
            var data = new List<TestEntity>().AsQueryable();

            // Act
            var result = _controller.ToCSV(data, "test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.csv", result.FileDownloadName);
            Assert.Equal("text/csv", result.ContentType);
        }

        [Fact]
        public void ToExcel_ShouldHandleEmptyData()
        {
            // Arrange
            var data = new List<TestEntity>().AsQueryable();

            // Act
            var result = _controller.ToExcel(data, "test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.xlsx", result.FileDownloadName);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
