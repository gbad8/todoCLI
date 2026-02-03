using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TodoCLI.Tasks;
using TodoCLI.Sync;

namespace TodoCLI.Sync;

/// <summary>
/// HTTP client for GitHub Gist operations
/// </summary>
public class HttpGistClient : IGistClient
{
    private readonly HttpClient _httpClient;
    private readonly string _gistFileName = "todolist.json";

    public HttpGistClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TodoCLI", "1.0"));
    }

    public async Task<GistResult<IEnumerable<TodoTask>>> GetTasksAsync(string token)
    {
        try
        {
            // First, find the TodoCLI gist
            var gistId = await FindTodoGistAsync(token);
            if (gistId == null)
            {
                return new GistResult<IEnumerable<TodoTask>>
                {
                    Success = false,
                    ErrorMessage = "TodoCLI gist not found"
                };
            }

            // Get the gist content
            using var request = new HttpRequestMessage(HttpMethod.Get, $"gists/{gistId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new GistResult<IEnumerable<TodoTask>>
                {
                    Success = false,
                    ErrorMessage = $"Failed to fetch gist: {response.StatusCode}"
                };
            }

            var content = await response.Content.ReadAsStringAsync();
            var gist = JsonSerializer.Deserialize<JsonElement>(content);

            if (!gist.GetProperty("files").TryGetProperty(_gistFileName, out var fileElement))
            {
                return new GistResult<IEnumerable<TodoTask>>
                {
                    Success = false,
                    ErrorMessage = "todolist.json not found in gist"
                };
            }

            var fileContent = fileElement.GetProperty("content").GetString() ?? "[]";
            var tasks = ParseTasksFromJson(fileContent);

            return new GistResult<IEnumerable<TodoTask>>
            {
                Success = true,
                Data = tasks
            };
        }
        catch (Exception ex)
        {
            return new GistResult<IEnumerable<TodoTask>>
            {
                Success = false,
                ErrorMessage = $"Error fetching tasks: {ex.Message}"
            };
        }
    }

    public async Task<GistResult> CreateGistAsync(string token, IEnumerable<TodoTask> tasks)
    {
        try
        {
            var tasksJson = SerializeTasksToJson(tasks);
            
            var gistData = new
            {
                description = "TodoCLI Task List",
                @public = false,
                files = new Dictionary<string, object>
                {
                    [_gistFileName] = new { content = tasksJson }
                }
            };

            var json = JsonSerializer.Serialize(gistData, new JsonSerializerOptions { WriteIndented = true });
            
            using var request = new HttpRequestMessage(HttpMethod.Post, "gists")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new GistResult { Success = true };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new GistResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to create gist: {response.StatusCode} - {errorContent}"
                };
            }
        }
        catch (Exception ex)
        {
            return new GistResult
            {
                Success = false,
                ErrorMessage = $"Error creating gist: {ex.Message}"
            };
        }
    }

    public async Task<GistResult> UpdateGistAsync(string token, IEnumerable<TodoTask> tasks)
    {
        try
        {
            // Find the existing gist
            var gistId = await FindTodoGistAsync(token);
            if (gistId == null)
            {
                return new GistResult
                {
                    Success = false,
                    ErrorMessage = "TodoCLI gist not found for update"
                };
            }

            var tasksJson = SerializeTasksToJson(tasks);
            
            var updateData = new
            {
                files = new Dictionary<string, object>
                {
                    [_gistFileName] = new { content = tasksJson }
                }
            };

            var json = JsonSerializer.Serialize(updateData, new JsonSerializerOptions { WriteIndented = true });
            
            using var request = new HttpRequestMessage(HttpMethod.Patch, $"gists/{gistId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new GistResult { Success = true };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new GistResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to update gist: {response.StatusCode} - {errorContent}"
                };
            }
        }
        catch (Exception ex)
        {
            return new GistResult
            {
                Success = false,
                ErrorMessage = $"Error updating gist: {ex.Message}"
            };
        }
    }

    private async Task<string?> FindTodoGistAsync(string token)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "gists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var gists = JsonSerializer.Deserialize<JsonElement[]>(content);

            foreach (var gist in gists)
            {
                var description = gist.GetProperty("description").GetString();
                if (description == "TodoCLI Task List")
                {
                    return gist.GetProperty("id").GetString();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private string SerializeTasksToJson(IEnumerable<TodoTask> tasks)
    {
        var taskData = tasks.Select(t => new
        {
            hash = t.Hash,
            description = t.Description,
            status = t.Status.ToString(),
            createdAt = t.CreatedAt.ToString("O") // ISO 8601 format
        });

        return JsonSerializer.Serialize(taskData, new JsonSerializerOptions { WriteIndented = true });
    }

    private List<TodoTask> ParseTasksFromJson(string json)
    {
        try
        {
            var taskElements = JsonSerializer.Deserialize<JsonElement[]>(json);
            var tasks = new List<TodoTask>();

            foreach (var element in taskElements)
            {
                var hash = element.GetProperty("hash").GetString();
                var description = element.GetProperty("description").GetString();
                var statusStr = element.GetProperty("status").GetString();
                var createdAtStr = element.GetProperty("createdAt").GetString();

                if (hash != null && description != null && 
                    Enum.TryParse<TodoStatus>(statusStr, out var status) &&
                    DateTime.TryParse(createdAtStr, out var createdAt))
                {
                    tasks.Add(new TodoTask(hash, description, status, createdAt));
                }
            }

            return tasks;
        }
        catch
        {
            return new List<TodoTask>();
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}