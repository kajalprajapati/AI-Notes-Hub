using System.IO;
using AINotesHub.API.Data;
using AINotesHub.Shared.DTOs;
using AINotesHub.Shared.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/files")]
    [Authorize]
    public class FileUploadController : Controller
    {
        private readonly NotesDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadController> _logger;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] AllowedExtensions =
{
    ".pdf",
    ".doc",
    ".docx",
    ".jpg",
    ".jpeg",
    ".png",
    ".txt",
    ".xls",
    ".xlsx"
};

        public FileUploadController(
            NotesDbContext context,
            IWebHostEnvironment environment, ILogger<FileUploadController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;

        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
    [FromForm] FileUploadRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File is required."
                    });
                }


                // Check note exists
                var noteExists = await _context.Notes
                    .AnyAsync(n => n.Id == request.NoteId);

                if (!noteExists)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Note not found."
                    });
                }


                // File size validation
                if (request.File.Length > MaxFileSize)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File size cannot exceed 5 MB."
                    });
                }


                var extension =
                    Path.GetExtension(request.File.FileName)
                        .ToLower();


                // Extension validation
                if (!AllowedExtensions.Contains(extension))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File type not allowed."
                    });
                }


                // Create uploads folder
                var uploadFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads");


                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }


                // Generate unique filename
                var storedFileName =
                    $"{Guid.NewGuid()}{extension}";


                var filePath = Path.Combine(
                    uploadFolder,
                    storedFileName);


                // Save physical file
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }


                // Save database information
                var attachment = new NoteAttachment
                {
                    Id = Guid.NewGuid(),

                    NoteId = request.NoteId,

                    OriginalFileName =
                        request.File.FileName,

                    StoredFileName =
                        storedFileName,

                    FilePath =
                        $"uploads/{storedFileName}",

                    ContentType =
                        request.File.ContentType,

                    FileExtension =
                        extension,

                    FileSize =
                        request.File.Length,

                    UploadedAt =
                        DateTime.UtcNow
                };


                _context.NoteAttachments.Add(attachment);

                await _context.SaveChangesAsync();


                _logger.LogInformation(
                    "File uploaded successfully: {FileName}",
                    request.File.FileName);


                return Created("", new
                {
                    success = true,
                    message = "File uploaded successfully.",
                    data = new
                    {
                        attachment.Id,
                        attachment.OriginalFileName,
                        attachment.FileSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "File upload failed");

                throw; // Global Exception Middleware handles it
            }
        }

    }
}
