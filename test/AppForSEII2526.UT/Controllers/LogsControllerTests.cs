using AppForSEII2526.API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII2526.UT.Controllers
{
    public class LogsControllerTests
    {
        private readonly Mock<ILogger<LogsController>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly string _testLogDirectory;

        public LogsControllerTests()
        {
            _mockLogger = new Mock<ILogger<LogsController>>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _testLogDirectory = Path.Combine(Path.GetTempPath(), "test-logs-" + Guid.NewGuid());
            _mockEnv.Setup(x => x.ContentRootPath).Returns(_testLogDirectory);
        }

        [Fact]
        public void GetLogFiles_ReturnsEmptyArray_WhenLogDirectoryDoesNotExist()
        {
            // Arrange
            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var files = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Empty(files);
        }

        [Fact]
        public void GetLogFiles_ReturnsListOfFiles_WhenLogFilesExist()
        {
            // Arrange
            Directory.CreateDirectory(Path.Combine(_testLogDirectory, "logs"));
            var logFile1 = Path.Combine(_testLogDirectory, "logs", "log1.txt");
            var logFile2 = Path.Combine(_testLogDirectory, "logs", "log2.txt");
            File.WriteAllText(logFile1, "Log content 1");
            File.WriteAllText(logFile2, "Log content 2");

            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var files = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Equal(2, files.Count());

            // Cleanup
            Directory.Delete(Path.Combine(_testLogDirectory, "logs"), true);
        }

        [Fact]
        public void GetLogContent_ReturnsBadRequest_WhenFileNameIsEmpty()
        {
            // Arrange
            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogContent("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("File name is required", badRequestResult.Value);
        }

        [Fact]
        public void GetLogContent_ReturnsBadRequest_WhenFileNameContainsInvalidCharacters()
        {
            // Arrange
            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogContent("../etc/passwd");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid file name", badRequestResult.Value);
        }

        [Fact]
        public void GetLogContent_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            Directory.CreateDirectory(Path.Combine(_testLogDirectory, "logs"));
            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogContent("nonexistent.txt");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Log file not found", notFoundResult.Value);

            // Cleanup
            Directory.Delete(Path.Combine(_testLogDirectory, "logs"), true);
        }

        [Fact]
        public void GetLogContent_ReturnsContent_WhenFileExists()
        {
            // Arrange
            Directory.CreateDirectory(Path.Combine(_testLogDirectory, "logs"));
            var logFile = Path.Combine(_testLogDirectory, "logs", "test.txt");
            var expectedContent = "Line 1\nLine 2\nLine 3";
            File.WriteAllText(logFile, expectedContent);

            var controller = new LogsController(_mockLogger.Object, _mockEnv.Object);

            // Act
            var result = controller.GetLogContent("test.txt");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var content = Assert.IsType<string>(okResult.Value);
            Assert.Contains("Line 1", content);
            Assert.Contains("Line 2", content);
            Assert.Contains("Line 3", content);

            // Cleanup
            Directory.Delete(Path.Combine(_testLogDirectory, "logs"), true);
        }
    }
}
