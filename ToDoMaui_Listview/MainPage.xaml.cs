using System.Collections.ObjectModel;

namespace ToDoMaui_Listview;

public partial class MainPage : ContentPage
{
    private ApiService _apiService = new ApiService();
    public ObservableCollection<ToDoClass> ToDos { get; set; } = new ObservableCollection<ToDoClass>();
    private ToDoClass? _selectedToDo;
    private int _currentUserId;

    public MainPage(int userId)
    {
        InitializeComponent();
        _currentUserId = userId;
        todoLV.ItemsSource = ToDos;
    }

    // 1. Create a dedicated task loader that is safe to await
    private async Task LoadTasks()
    {
        var tasksFromApi = await _apiService.GetTasksAsync(_currentUserId, "active");
        ToDos.Clear();
        foreach (var task in tasksFromApi) ToDos.Add(task);
    }

    // 2. Update OnAppearing to use the new safe loader
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTasks();
    }

    // 3. Update the Add button to listen for server errors
    private async void AddToDoItem(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) || string.IsNullOrWhiteSpace(descEntry.Text)) return;

        // Post to LIVE API and wait for the response
        var response = await _apiService.AddTaskAsync(nameEntry.Text, descEntry.Text, _currentUserId);

        if (response != null && response.status == 200)
        {
            // Success! Clear the inputs and safely await the list refresh
            ClearInputs();
            await LoadTasks();
        }
        else
        {
            // Fail! Show us exactly what the server said is wrong
            await DisplayAlert("Server Error", response?.message ?? "Unknown Error", "OK");
        }
    }

    private void TriggerEditMode(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass item)
        {
            _selectedToDo = item;
            nameEntry.Text = item.item_name;
            descEntry.Text = item.item_description;

            addBtn.IsVisible = false; editBtn.IsVisible = true; cancelBtn.IsVisible = true;
        }
    }

    private async void SaveEditItem(object sender, EventArgs e)
    {
        if (_selectedToDo != null)
        {
            // Send PUT request to API
            await _apiService.UpdateTaskAsync(_selectedToDo.item_id, nameEntry.Text, descEntry.Text);
            OnAppearing(); // Refresh data
            CancelEdit(null, null);
        }
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        ClearInputs();
        addBtn.IsVisible = true; editBtn.IsVisible = false; cancelBtn.IsVisible = false;
    }

    private async void DeleteToDoItem(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass itemToDelete)
        {
            await _apiService.DeleteTaskAsync(itemToDelete.item_id);
            ToDos.Remove(itemToDelete);
            if (_selectedToDo == itemToDelete) CancelEdit(null, null);
        }
    }

    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoClass task)
        {
            if (e.Value)
            {
                // Wait for the API to respond
                var response = await _apiService.ChangeStatusAsync(task.item_id, "inactive");

                if (response != null && response.status == 200)
                {
                    ToDos.Remove(task); // Success! Safe to remove visually
                }
                else
                {
                    // Fail! Show us exactly what the server complained about
                    await DisplayAlert("Server Error", response?.message ?? "Unknown Error", "OK");
                    task.status = "active"; // Uncheck the box visually since it failed
                }
            }
        }
    }

    private void ClearInputs() { nameEntry.Text = string.Empty; descEntry.Text = string.Empty; _selectedToDo = null; }
}