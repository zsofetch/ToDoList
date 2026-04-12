using System.Net.Http.Json;
using System.Text.Json;

namespace ToDoMaui_Listview;

// Existing Authentication Responses
public class ApiResponse { public int status { get; set; } public string message { get; set; } public UserData data { get; set; } }
public class UserData { public int id { get; set; } public string fname { get; set; } public string lname { get; set; } public string email { get; set; } }

// NEW: Task Response Model
public class GetItemsResponse
{
    public int status { get; set; }
    // The API returns an object with numbers as keys (e.g., "0": { task... })
    public Dictionary<string, ToDoClass>? data { get; set; }
}

public class ApiService
{
    private readonly string _baseUrl = "https://todo-list.dcism.org";
    private readonly HttpClient _client = new HttpClient();

    // --- SIGN UP / SIGN IN ---
    public async Task<ApiResponse> SignUpAsync(string fName, string lName, string email, string password, string confirmPassword)
    {
        var payload = new { first_name = fName, last_name = lName, email = email, password = password, confirm_password = confirmPassword };
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/signup_action.php", payload);
        return JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<ApiResponse> SignInAsync(string email, string password)
    {
        var response = await _client.GetAsync($"{_baseUrl}/signin_action.php?email={email}&password={password}");
        return JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync());
    }

    // --- NEW: TO-DO TASK APIs ---

    // 1. Get Tasks (Filter by active/inactive)
    public async Task<List<ToDoClass>> GetTasksAsync(int userId, string status)
    {
        try
        {
            var response = await _client.GetAsync($"{_baseUrl}/getItems_action.php?status={status}&user_id={userId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetItemsResponse>(json);

            if (result != null && result.data != null)
                return result.data.Values.ToList(); // Convert the dictionary back into a normal List
        }
        catch { } // If the API returns empty/null, catch the error safely

        return new List<ToDoClass>();
    }
    // 2. Add Task
    public async Task<ApiResponse> AddTaskAsync(string name, string desc, int userId)
    {
        var payload = new Dictionary<string, object>
        {
            { "item_name", name },
            { "item_description", desc },
            { "user_id", userId }
        };

        // FIX: Changed "additem_action.php" to "addItem_action.php" (Capital I)
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/addItem_action.php", payload);
        var json = await response.Content.ReadAsStringAsync();

        try { return JsonSerializer.Deserialize<ApiResponse>(json); }
        catch { return new ApiResponse { status = 500, message = json }; }
    }
    // 3. Update Task Text
    public async Task<bool> UpdateTaskAsync(int itemId, string name, string desc)
    {
        var payload = new Dictionary<string, object>
        {
            { "item_id", itemId }, // <-- Changed back to underscore!
            { "item_name", name },
            { "item_description", desc }
        };

        var response = await _client.PutAsJsonAsync($"{_baseUrl}/editItem_action.php", payload);
        return response.IsSuccessStatusCode;
    }

    // 4. Change Status (Check/Uncheck)
    public async Task<ApiResponse> ChangeStatusAsync(int itemId, string status)
    {
        var payload = new { item_id = itemId, status = status };

        // Guessed capital 'I' to match addItem and editItem
        var response = await _client.PutAsJsonAsync($"{_baseUrl}/statusItem_action.php", payload);
        var json = await response.Content.ReadAsStringAsync();

        try { return JsonSerializer.Deserialize<ApiResponse>(json); }
        catch { return new ApiResponse { status = 500, message = json }; }
    }

    // 5. Delete Task
    public async Task<bool> DeleteTaskAsync(int itemId)
    {
        var response = await _client.DeleteAsync($"{_baseUrl}/deleteItem_action.php?item_id={itemId}");
        return response.IsSuccessStatusCode;
    }
}