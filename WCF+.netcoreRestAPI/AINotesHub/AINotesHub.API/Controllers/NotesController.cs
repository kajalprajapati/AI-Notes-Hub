using AINotesHub.API.Data;
using AINotesHub.API.Services;
using AINotesHub.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AINotesHub.Shared.DTOs;

namespace AINotesHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
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

        // GET: api/notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            return await _context.Notes.ToListAsync();
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
        public async Task<IActionResult> PutNote(Guid id, Note note)
        {
            if (id != note.Id)
            {
                return BadRequest();
            }

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
