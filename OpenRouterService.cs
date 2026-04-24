using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ML_Log_Analyzer;

public class OpenRouterRequest
{
    public string model { get; set; } = string.Empty;
    public List<Message> messages { get; set; } = new();
}

public class Message
{
    public string role { get; set; } = "user";
    public string content { get; set; } = string.Empty;
}

public class OpenRouterResponse
{
    public List<Choice> choices { get; set; } = new();
}

public class Choice
{
    public Message message { get; set; } = new();
}

public static class OpenRouterService
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<string> AnalyzeLogAsync(string apiKey, string logContent, string model)
    {
        var prompt = $@"You are analyzing MelonLoader logs for Unity games. 
Analyze the following log content and identify any errors. 
For each error found, explain:
1. What is causing the error
2. How to fix it

Provide your answer in English. If there are no errors, simply state that the log appears clean with no issues found.

Log content:
{logContent}";

        var request = new OpenRouterRequest
        {
            model = model,
            messages = new List<Message>
            {
                new Message { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/AeroChoclate/ML-Log-Analyzer");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "ML-Log-Analyzer");

        var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return $"API Error: {response.StatusCode}\n{responseContent}";
        }

        var result = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent);
        return result?.choices?.FirstOrDefault()?.message?.content ?? "No response from API";
    }
}