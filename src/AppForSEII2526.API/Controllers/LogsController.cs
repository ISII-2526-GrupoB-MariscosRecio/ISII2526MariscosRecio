using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;
        private readonly string _logDirectory;

        public LogsController(ILogger<LogsController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _logDirectory = Path.Combine(env.ContentRootPath, "logs");
        }

        /// <summary>
        /// Get list of available log files
        /// </summary>
        [HttpGet]
        [Route("files")]
        public ActionResult<IEnumerable<string>> GetLogFiles()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    return Ok(Array.Empty<string>());
                }

                var files = Directory.GetFiles(_logDirectory, "*.txt")
                    .Select(Path.GetFileName)
                    .OrderByDescending(f => f)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving log files");
                return StatusCode(500, "Error retrieving log files");
            }
        }

        /// <summary>
        /// Get content of a specific log file
        /// </summary>
        [HttpGet]
        [Route("content")]
        public ActionResult<string> GetLogContent([FromQuery] string fileName, [FromQuery] int? lines = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return BadRequest("File name is required");
                }

                // Validate file name to prevent directory traversal
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest("Invalid file name");
                }

                var filePath = Path.Combine(_logDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Log file not found");
                }

                var content = System.IO.File.ReadAllLines(filePath);

                if (lines.HasValue && lines.Value > 0)
                {
                    content = content.TakeLast(lines.Value).ToArray();
                }

                return Ok(string.Join(Environment.NewLine, content));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading log file: {FileName}", fileName);
                return StatusCode(500, "Error reading log file");
            }
        }

        /// <summary>
        /// Delete a specific log file
        /// </summary>
        [HttpDelete]
        [Route("{fileName}")]
        public ActionResult DeleteLogFile(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return BadRequest("File name is required");
                }

                // Validate file name to prevent directory traversal
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest("Invalid file name");
                }

                var filePath = Path.Combine(_logDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Log file not found");
                }

                System.IO.File.Delete(filePath);
                _logger.LogInformation("Log file deleted: {FileName}", fileName);

                return Ok("Log file deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting log file: {FileName}", fileName);
                return StatusCode(500, "Error deleting log file");
            }
        }
    }
}
