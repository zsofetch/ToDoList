using System.Collections.ObjectModel;

namespace ToDoMaui_Listview;

public partial class CompletedPage : ContentPage
{
    private ApiService _apiService = new ApiService();
    public ObservableCollection<ToDoClass> CompletedToDos { get; set; } = new ObservableCollection<ToDoClass>();
    private ToDoClass? _selectedToDo;
    private int _currentUserId;

    public CompletedPage(int userId)
    {
        InitializeComponent();
        _currentUserId = userId;
        completedLV.ItemsSource = CompletedToDos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Fetch inactive items from API
        var tasksFromApi = await _apiService.GetTasksAsync(_currentUserId, "inactive");
        CompletedToDos.Clear();
        foreach (var task in tasksFromApi) CompletedToDos.Add(task);
    }

    private void TriggerEditMode(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass item)
        {
            _selectedToDo = item;
            nameEntry.Text = item.item_name;
            descEntry.Text = item.item_description;
            editPanel.IsVisible = true;
        }
    }

    private async void SaveEditItem(object sender, EventArgs e)
    {
        if (_selectedToDo != null)
        {
            await _apiService.UpdateTaskAsync(_selectedToDo.item_id, nameEntry.Text, descEntry.Text);
            OnAppearing();
            CancelEdit(null, null);
        }
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        nameEntry.Text = string.Empty; descEntry.Text = string.Empty; _selectedToDo = null;
        editPanel.IsVisible = false;
    }

    private async void DeleteToDoItem(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass itemToDelete)
        {
            await _apiService.DeleteTaskAsync(itemToDelete.item_id);
            CompletedToDos.Remove(itemToDelete);
            if (_selectedToDo == itemToDelete) CancelEdit(null, null);
        }
    }

    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoClass task)
        {
            if (!e.Value)
            {
                // Wait for the API to respond
                var response = await _apiService.ChangeStatusAsync(task.item_id, "active");

                if (response != null && response.status == 200)
                {
                    CompletedToDos.Remove(task); // Success! Safe to remove visually
                }
                else
                {
                    // Fail! Show us exactly what the server complained about
                    await DisplayAlert("Server Error", response?.message ?? "Unknown Error", "OK");
                    task.status = "inactive"; // Re-check the box visually since it failed
                }
            }
        }
    }
}