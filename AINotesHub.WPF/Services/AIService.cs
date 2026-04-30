using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AINotesHub.WPF.Helpers;
using Windows.Web.Http;
using HttpClient = System.Net.Http.HttpClient;

namespace AINotesHub.WPF.Services
{
    public partial class AIService
    {
        private static readonly Lazy<AIService> _instance =
    new Lazy<AIService>(() => new AIService());
        public static AIService Instance => _instance.Value;
        //private readonly HttpClient _client;
        private static readonly HttpClient _client = new HttpClient();
        public AIService()
        {
            //_client = new HttpClient();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", OpenAIConfig.ApiKey);

            _client.BaseAddress =
            new Uri("https://api.openai.com/");
        }

        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, int baseDelayMs = 2000)//// wait for 2 seconds
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (HttpRequestException ex) when (IsRetryable(ex))
                {
                    if (attempt == maxRetries)
                        throw;

                    await Task.Delay(baseDelayMs * attempt); // exponential delay
                }
                catch (TaskCanceledException) // timeout case
                {
                    if (attempt == maxRetries)
                        throw;

                    await Task.Delay(baseDelayMs * attempt);
                }
            }

            throw new Exception("Retry failed after max attempts");
        }

        private bool IsRetryable(HttpRequestException ex)
        {
            // You can improve this later with status codes
            return true; // retry all HTTP errors for now
        }

        public async Task<string> GetAIResponse(string prompt)
        {
            try
            {
                var result = await ExecuteWithRetryAsync(async () =>
                {
                    var requestBody = new
                    {
                        model = "gpt-4.1-mini",
                        messages = new[]
                        {
                    new { role = "user", content = prompt }
                }
                    };

                    var response = await _client.PostAsJsonAsync(
                        "v1/chat/completions",
                        requestBody
                    );

                    // 🔥 IMPORTANT: THROW for retry cases
                    if ((int)response.StatusCode == 429)
                        throw new HttpRequestException("Too many requests");

                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"API Error: {response.StatusCode}");

                    var json = await response.Content.ReadAsStringAsync();
                    return ExtractText(json);
                });

                return result;
            }
            catch (HttpRequestException)
            {
                return "Error: Unable to reach AI service";
            }
            catch (TaskCanceledException)
            {
                return "Error: Request timed out";
            }
            catch (Exception)
            {
                return "Error: AI processing failed";
            }
        }

        //    public async Task<string> GetAIResponse(string prompt)
        //    {
        //        try
        //        {
        //            return await ExecuteWithRetryAsync(async () =>
        //        {
        //            var requestBody = new
        //            {
        //                model = "gpt-4.1-mini",
        //                messages = new[]
        //        {
        //        new { role = "user", content = prompt }
        //}
        //            };

        //            var response = await _client.PostAsJsonAsync(
        //                    "v1/chat/completions",   // ✅ no full URL needed
        //                    requestBody
        //                );


        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var result = await response.Content.ReadAsStringAsync();
        //                return ExtractText(result);
        //            }

        //            //response.EnsureSuccessStatusCode();

        //        });

        //            return result;
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // Network/API error
        //            return "Error: Unable to reach AI service";
        //        }
        //        catch (TaskCanceledException)
        //        {
        //            // Timeout
        //            return "Error: Request timed out";
        //        }
        //        catch (Exception)
        //        {
        //            // Any other issue
        //            return "Error: AI processing failed";
        //        }
        //    }

        public async Task<string> AskAIAsync(string prompt)
        {
            var request = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = prompt }
        }
            };

            string url = "v1/responses";

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseText);

            response.EnsureSuccessStatusCode();

            //var result = await response.Content.ReadAsStringAsync();

            return responseText;
        }

        // ✅ Summarize Note
        public async Task<string> SummarizeAsync(string noteText)
        {


            //var request = new
            //{
            //    model = "gpt-4o-mini",
            //    messages = new[]
            //    {
            //        new { role = "system", content = "You are a helpful assistant that summarizes notes." },
            //        new { role = "user", content = $"Summarize this note in simple words:\n{noteText}" }
            //    }
            //};

            string url = "v1/responses";
            //_httpClient = new HttpClient();

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                input = $"Summarize this note in simple words:\n{noteText}"
            };

            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseText);

            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseText);

            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();

        }

        public async Task<DateTime?> ExtractReminderDateAsync(string text)
        {
            var prompt = $"""
Extract reminder date from this text.
Return only date in format: yyyy-MM-dd HH:mm

Text:
{text}
""";

            var response = await CallOpenAIAsync(prompt);

            if (DateTime.TryParse(response, out var date))
                return date;

            return null;
        }

        public async Task<string> CallOpenAIAsync(string prompt)
        {
            var request = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new { role = "user", content = prompt }
            }
            };

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(
                "v1/chat/completions",
                content);

            var result = await response.Content.ReadAsStringAsync();

            return ExtractText(result);
        }

        private string ExtractText(string json)
        {
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }

        public async Task<string> GenerateAsync(string content)
        {
            var prompt = $@"
You are an intelligent assistant.

Analyze the following note and return:

Title: (max 8 words)
Summary: (2-3 lines)
Tags: (3-5 comma separated)

Note:
{content}
";

            var request = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var response = await _client.PostAsJsonAsync(
                "https://api.openai.com/v1/chat/completions",
                request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            var result = json
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result?.Trim();
        }


    }
}
