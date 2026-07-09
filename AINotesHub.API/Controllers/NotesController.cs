using System.Security.Claims;
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

    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]

    public class NotesController : ControllerBase
    {

        private readonly DapperService _dapperService; // Dapper
        private readonly NotesDbContext _context; //EF Core

        public NotesController(NotesDbContext context, DapperService dapperService)
        {
            _context = context;
            _dapperService = dapperService;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}


        //[HttpGet]
        //[MapToApiVersion("1.0")]
        //public IActionResult GetV1()
        //{
        //    return Ok("Version 1");
        //}

        //[HttpGet]
        //[MapToApiVersion("2.0")]
        //public IActionResult GetV2()
        //{
        //    return Ok("Version 2");
        //}


        //[Authorize]//Protected
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            return await _context.Notes.ToListAsync();
        }

        //With Pagination
        // [AllowAnonymous]
        // GET: api/notes

        //[HttpGet("?page={page}&pageSize={pageSize}/")]
        [HttpGet("paged")]
        //[HttpGet]

        //GET /api/notes/paged?page=1&pageSize=10
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByPage(int page = 1, int pageSize = 10)
        {
            ///var Result = ((page - 1) * pageSize);

            var totalCount = await _context.Notes.CountAsync();

            var notes = await _context.Notes
    .OrderBy(n => n.Id)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

            //return await _context.Notes.OrderBy(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Notes = notes
            });
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

        // GET: api/notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(Guid id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        // POST: api/notes
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {

            var userIdClaim = User.FindFirst("id")?.Value;
            if (userIdClaim == null)
                return Unauthorized("Invalid token or user not found.");

            note.UserId = Guid.Parse(userIdClaim);
            note.CreatedAt = DateTime.UtcNow;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }

        // PUT: api/notes/5
        [HttpPut("{id}")]
        //public async Task<IActionResult> UpdateNote(Guid id, Note note)
        public async Task<IActionResult> UpdateNote(Guid id, UpdateNoteDto dto)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
                return NotFound();

            note.Title = dto.Title;
            note.Content = dto.Content;
            note.Category = dto.Category;

            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Notes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        //public async Task<IActionResult> PutNote(Guid id, Note note)
        //{
        //    if (id != note.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(note).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.Notes.Any(e => e.Id == id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // DELETE: api/notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
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

    }
}
