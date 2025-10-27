using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AINotesHub.Shared;

namespace AINotesHub.WPF.Services
{
    public class NotesApiService
    {
        private readonly HttpClient _httpClient;

        public NotesApiService()
        {
            // ⚠️ Change base URL to match your running API (check your launchSettings.json)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44357/")
            };
        }

        /// <summary>
        /// Get all notes from the API.
        /// </summary>
        public async Task<List<Note>> GetAllNotesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/notes");

                if (response.IsSuccessStatusCode)
                {
                    var notes = await response.Content.ReadFromJsonAsync<List<Note>>();
                    return notes ?? new List<Note>(); // return empty list if null
                }

                // Optionally, log error or show message
                MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<Note>();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Network error: {ex.Message}");
                return new List<Note>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}");
                return new List<Note>();
            }
        }

        /// <summary>
        /// Add a new note to the API.
        /// </summary>
        public async Task<(bool IsSuccess, Note? Data, string Message)> AddNoteAsync(Note note)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/notes", note);

                if (response.IsSuccessStatusCode)
                {
                    var createdNote = await response.Content.ReadFromJsonAsync<Note>();
                    return (true, createdNote, "Note added successfully.");
                }

                return (false, null, $"Failed to add note: {response.StatusCode} - {response.ReasonPhrase}");
            }
            catch (HttpRequestException ex)
            {
                return (false, null, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, null, $"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing note.
        /// </summary>
        public async Task<(bool IsSuccess, string Message)> UpdateNoteAsync(Note note)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/notes/{note.Id}", note);

                if (response.IsSuccessStatusCode)
                    return (true, "Note updated successfully.");

                return (false, $"Failed to update note: {response.StatusCode} - {response.ReasonPhrase}");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a note by Id.
        /// </summary>
        public async Task<(bool IsSuccess, string Message)> DeleteNoteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/notes/{id}");

                if (response.IsSuccessStatusCode)
                    return (true, "Note deleted successfully.");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return (false, "Note not found.");

                return (false, $"Failed to delete note: {response.StatusCode} - {response.ReasonPhrase}");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
