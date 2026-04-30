using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using AINotesHub.Shared.DTOs;
using AINotesHub.Shared.Entities; 
using AINotesHub.WPF.Helpers;
using Serilog;


namespace AINotesHub.WPF.Services
{
    public class NotesApiService : INotesService
    {
        private readonly HttpClient _httpClient;
        private string? _jwtToken; // 🔹 Store JWT token

        public NotesApiService(HttpClient httpClient)
        {
            // ⚠️ Change base URL to match your running API (check your launchSettings.json)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44357/")
            };
        }

        public async Task<int> GetNextUntitledNumberFromApi(Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/notes/next-untitled?userId={userId}");

            // response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                // Optional: log error
                Debug.WriteLine($"API Error: {error}");

                return 0; // fallback
            }

            var result = await response.Content.ReadAsStringAsync();

            //return int.Parse(result);
            return int.TryParse(result, out var number) ? number : 0;
        }

        public async Task<ApiResponse<List<Note>>> SearchNotes(string keyword)
        {
            var url = $"api/notes/search?keyword={keyword}";

            return await _httpClient.GetFromJsonAsync<ApiResponse<List<Note>>>(url);
        }

        /// <summary>
        /// Set the JWT token for authorized calls.
        /// </summary>
        public Task SetJwtToken(string token)
        {
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
            {
                // Token already set → do nothing
                return Task.CompletedTask;
            }

            _jwtToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            Log.Information("JWT token set for API client");

            return Task.CompletedTask;
        }

        // 🔒 Guard method
        private void EnsureReady()
        {
            if (_httpClient.BaseAddress == null)
                throw new InvalidOperationException("BaseAddress not set");

            if (_httpClient.DefaultRequestHeaders.Authorization == null)
                throw new InvalidOperationException("JWT token not set");
        }

        // 🔥 Centralized method for token expiry handling
        public async Task<HttpResponseMessage> SendRequest(Func<Task<HttpResponseMessage>> apiCall)
        {
            try
            {
                var response = await apiCall();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Clear session only
                    //SessionHelper.ClearSession("Session expired. Please login again.");
                    await AppSession.Clear();
                    MessageBox.Show("Session expired. Please login again.");
                }

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "API request failed.");
                MessageBox.Show("Something went wrong while connecting to server.");
                throw;
            }
        }

        /// <summary>
        /// Get all notes from the API.Get all notes (authorized).
        /// </summary>
        public async Task<List<Note>> GetNotesAsync()
        {
            try
            {
                EnsureReady(); // 👈 ADD HERE
                //var response = await _httpClient.GetAsync("api/notes");
                var response = await SendRequest(() => _httpClient.GetAsync("api/notes"));
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

        ///<summary>
        /// Generate Title the API.
        ///</summary>

        public async Task<string> GenerateTitle(string content)
        {
            var prompt = $@"
Generate a short, meaningful title (max 5 words).
Do not include quotes or extra explanation.

Content:
{content}
";

            var response = await AIService.Instance.GetAIResponse(prompt);

            return response.Trim().Replace("\"", "");
        }

        /// <summary>
        /// Add a new note to the API.
        /// </summary>
        public async Task<(bool IsSuccess, Note? Data, string Message)> AddNoteAsync(Note note)
        {
            try
            {
                EnsureReady(); // 👈 ADD HERE
                var response = await _httpClient.PostAsJsonAsync("api/notes", note);

                if (response.IsSuccessStatusCode)
                {
                    var createdNote = await response.Content.ReadFromJsonAsync<Note>();

                    //MessageBox.Show($"Login successful! Welcome, {loginResponse.Role}");
                    // Let UI refresh immediately

                    return (true, createdNote, "Note added successfully.");
                }
                // 🔒 Handle unauthorized or other API errors gracefully
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (false, null, "Unauthorized. Please log in again.");
                }
                string serverError = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Failed to add note: {response.StatusCode}\nDetails: {serverError}");

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
                EnsureReady(); // 👈 ADD HERE
                var response = await _httpClient.PutAsJsonAsync($"api/notes/{note.Id}", note);

                await Task.Delay(100);

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

                //HttpClient _httpClient = new HttpClient();
                //await SetJwtToken(AppSession.JwtToken);
                EnsureReady(); // 👈 ADD HERE
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
