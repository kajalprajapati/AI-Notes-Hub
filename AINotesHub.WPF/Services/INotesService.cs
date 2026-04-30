using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.Shared.DTOs;
using AINotesHub.Shared.Entities;

namespace AINotesHub.WPF.Services
{
    public interface INotesService
    {

        //In Interface → NO Access Modifier Needed
        Task<(bool IsSuccess, Note? Data, string Message)> AddNoteAsync(Note note);
        Task<(bool IsSuccess, string Message)> UpdateNoteAsync(Note note);
        Task<(bool IsSuccess, string Message)> DeleteNoteAsync(Guid id);
        Task<ApiResponse<List<Note>>> SearchNotes(string keyword);
        Task<List<Note>> GetNotesAsync();


        //public async Task AddNoteAsync(Note note) { }

        //public async Task DeleteNoteAsync(int id) { }

        //public async Task<List<Note>> GetAllAsync() { return new(); }
    }
}
