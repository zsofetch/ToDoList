using System.Collections.ObjectModel;

namespace ToDoMaui_Listview;

public partial class CompletedPage : ContentPage
{
    private DatabaseHelper _dbHelper = new DatabaseHelper();
    public ObservableCollection<ToDoClass> CompletedToDos { get; set; } = new ObservableCollection<ToDoClass>();
    private ToDoClass? _selectedToDo;
    private int _currentUserId;

    public CompletedPage(int userId)
    {
        InitializeComponent();
        _currentUserId = userId;
        completedLV.ItemsSource = CompletedToDos;
    }

    // Refreshes the list every time the user clicks on the "Completed" tab
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var tasksFromDb = await _dbHelper.GetTasksByStatus(_currentUserId, "Completed");
        CompletedToDos.Clear();
        foreach (var task in tasksFromDb)
        {
            CompletedToDos.Add(task);
        }
    }

    private void TriggerEditMode(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass item)
        {
            _selectedToDo = item;
            nameEntry.Text = item.item_name;
            descEntry.Text = item.item_description;

            editPanel.IsVisible = true; // Show the hidden input area
        }
    }

    private async void SaveEditItem(object sender, EventArgs e)
    {
        if (_selectedToDo != null)
        {
            _selectedToDo.item_name = nameEntry.Text;
            _selectedToDo.item_description = descEntry.Text;

            await _dbHelper.SaveToDo(_selectedToDo); // Save to Database
            CancelEdit(null, null);
        }
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        nameEntry.Text = string.Empty;
        descEntry.Text = string.Empty;
        _selectedToDo = null;
        editPanel.IsVisible = false; // Hide the input area again
    }

    private async void DeleteToDoItem(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass itemToDelete)
        {
            await _dbHelper.DeleteToDo(itemToDelete);
            CompletedToDos.Remove(itemToDelete);

            if (_selectedToDo == itemToDelete) CancelEdit(null, null);
        }
    }

    // This moves the task back to the Main page when unchecked!
    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoClass task)
        {
            // We only check if the box is UNCHECKED (e.Value is false)
            if (!e.Value)
            {
                task.status = "Pending"; // Ensure database gets the right string
                await _dbHelper.SaveToDo(task); // Save to database
                CompletedToDos.Remove(task); // Remove it from this tab
            }
        }
    }
}
