using System.Security.Claims;
using System.Security.Policy;
using AINotesHub.API.Data;
using AINotesHub.API.Services;
using AINotesHub.Shared.DTOs;
using AINotesHub.Shared.Entities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Controllers
{
    //REST-style endpoints.
    //[Authorize] //Protect Entire Controller
    //[Route("api/[controller]")]
    [ApiController]    //AfterAdding APivesion
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    //[ApiVersion("10.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]

    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> _logger;
        private readonly DapperService _dapperService; // Dapper
        private readonly NotesDbContext _context; //EF Core

        public NotesController(NotesDbContext context, DapperService dapperService, ILogger<NotesController> logger)
        {
            _context = context;
            _dapperService = dapperService;
            _logger = logger;

        }

        //[HttpGet]
        //[Authorize]//Protected
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            var notes = await _context.Notes.ToListAsync();

            //if (!notes.Any())
            //{
            //    return Ok(new ApiResponse<List<Note>>
            //    {
            //        Success = false,
            //        StatusCode = StatusCodes.Status404NotFound,
            //        Message = "No notes found.",
            //        Data = new List<Note>(),
            //        Count = 0
            //    });
            //}

            return Ok(new ApiResponse<List<Note>>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = notes.Any()
            ? "Notes retrieved successfully."
            : "No notes found.",
                Data = notes,
                Count = notes.Count
            });
        }

        //With Pagination
        //[AllowAnonymous]
        [HttpGet("paged")]
        //GET /api/notes/paged?page=1&pageSize=10
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByPage(int page = 1, int pageSize = 10)
        {
            ///var Result = ((page - 1) * pageSize);

            // Validate input
            if (page <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Page number must be greater than 0."
                });
            }

            if (pageSize <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Page size must be greater than 0."
                });
            }


            var totalCount = await _context.Notes.CountAsync();

            var notes = await _context.Notes
    .OrderBy(n => n.Id)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

            // No records found
            if (!notes.Any())
            {
                return Ok(new PagedResponse<Note>
                {
                    Items = new List<Note>(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    HasNextPage = false,
                    HasPreviousPage = page > 1
                });
            }

            return Ok(new PagedResponse<Note>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Notes retrieved successfully.",
                Items = notes,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            });
            //return Ok(new PagedResponse<Note>
            //{
            //    Items = notes,
            //    CurrentPage = page,
            //    PageSize = pageSize,
            //    TotalCount = totalCount,
            //    HasNextPage = page * pageSize < totalCount,
            //    HasPreviousPage = page > 1
            //});
        }

        // GET: api/notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(Guid id)
        {
            //var note = await _context.Notes.FindAsync(id);
            var note = await _context.Notes.FindAsync(id);


            if (note == null)
            {
                //return NotFound();
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Count = 0,
                    Message = $"Note with ID '{id}' was not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Note retrieved successfully.",
                Count = 1,
                Data = note
            });
            //return note;
        }


        // POST: api/notes
        [HttpPost]
        public async Task<ActionResult> CreateNote(CreateNoteDto dto)

        {


            // _logger.LogInformation("Claims:");
            //var userIdClaim = User.FindFirst("id")?.Value;
            // get userId from JWT token
            //        foreach (var claim in User.Claims)
            //        {
            //            _logger.LogInformation(
            //"USER CLAIM -> Type: {Type}, Value: {Value}",
            //claim.Type,
            //claim.Value);

            //            //Console.WriteLine($"{claim.Type} : {claim.Value}");
            //        }


            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Invalid token or user not found."
                });
            }

            //note.UserId = Guid.Parse(userIdClaim);
            //note.CreatedAt = DateTime.UtcNow;
            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                CardBackground = dto.CardBackground,
                IsStarred = dto.IsStarred,
                IsImportant = dto.IsImportant,
                IsReminderOn = dto.IsReminderOn,
                ReminderDateTime = dto.ReminderDateTime,
                UserId = Guid.Parse(userId)
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            //return Ok(note);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id },
            new ApiResponse<Note>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = "Note created successfully.",
                Data = note
            });

            //return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }


        // PUT: api/notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id, UpdateNoteDto dto)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Note with ID '{id}' was not found."
                });
            }

            //note.Title = dto.Title;
            //note.Content = dto.Content;
            //note.Category = dto.Category;

            note.Title = dto.Title;
            note.Content = dto.Content;
            note.Category = dto.Category;
            note.CardBackground = dto.CardBackground;
            //note.IsStarred = dto.IsStarred;
            //note.IsImportant = dto.IsImportant;
            //note.IsReminderOn = dto.IsReminderOn;
            //note.ReminderDateTime = dto.ReminderDateTime;

            //_context.Entry(note).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Note>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Note updated successfully.",
                Data = note
            });

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                //return NotFound();
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Note with ID '{id}' was not found."
                });
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Note deleted successfully."
            });

        }


        // PATCH: api/notes/{id}/star
        [HttpPatch("{id}/star")]
        public async Task<IActionResult> ToggleStar(Guid id)
        {
           throw new Exception("Testing Global Exception Middleware");

            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Note with ID '{id}' was not found."
                });
            }

            note.IsStarred = !note.IsStarred;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Note>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = note.IsStarred
                    ? "Note marked as starred."
                    : "Note removed from starred.",
                Data = note
            });
        }


        //Temporarily allow access:[AllowAnonymous]

        // ✅ Dapper - optimized query
        //[Authorize]
        [AllowAnonymous]
        [HttpGet("search")]//APIEndpint
        //[HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var result = await _dapperService.SearchNotes(keyword);

            var response = new ApiResponse<IEnumerable<Note>>
            {
                Data = result,
                Count = result.Count(),
                Message = result.Any() ? "Success" : "No data found",
                Success = result.Any()
            };

            return Ok(response);

        }

        [HttpGet("next-untitled")]

        public async Task<IActionResult> GetNextUntitled(Guid userId)
        {
            var nextNumber = await _dapperService.GetNextUntitledNumber(userId);
            return Ok(nextNumber);
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult Claims()
        {

            // return Ok("Working");

            return Ok(User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
        }
        //[Authorize(Roles = "User,Admin")]
        [Authorize(Roles = "Admin")]//Admin can see all users data...
        [HttpGet("all-users")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            //return await _context.Users.ToListAsync();
            var users = await _context.Users
           .Select(u => new UserListDto
           {
               Id = u.Id,
               Username = u.Username,
               Role = u.Role

           })
           .ToListAsync();
            return Ok(users);


            //return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()
        {
            return Ok("Admin Access Granted");
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-test")]
        public IActionResult UserTest()
        {
            return Ok("User Access Granted");
        }
        [Authorize]
        [HttpGet("my-role")]
        public IActionResult MyRole()
        {
            return Ok(new
            {
                //User is an inbuilt ASP.NET Core property available inside controllers.
                Username = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

    }
}
