using System.Collections.ObjectModel;

namespace ToDoMaui_Listview;

public partial class MainPage : ContentPage
{
    private DatabaseHelper _dbHelper = new DatabaseHelper();
    public ObservableCollection<ToDoClass> ToDos { get; set; } = new ObservableCollection<ToDoClass>(); //an ObservableCollection is a special type of list that automatically updates the screen.
                                                                                                        //When you say ToDos.Add(newTask) or ToDos.Remove(task), the XAML sees it happen and instantly redraws the UI 
    private ToDoClass? _selectedToDo;
    private int _currentUserId;

    public MainPage(int userId)
    {
        InitializeComponent();
        _currentUserId = userId;
        todoLV.ItemsSource = ToDos;
    }

    //for this method, evry single time the user clicks on a tab, this method fires.
    //It clears out the old list on the screen, asks the database for a fresh list of tasks, and repopulates the screen.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var tasksFromDb = await _dbHelper.GetTasksByStatus(_currentUserId, "Pending");
        ToDos.Clear();
        foreach (var task in tasksFromDb) ToDos.Add(task);
    }

    private async void AddToDoItem(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) || string.IsNullOrWhiteSpace(descEntry.Text))
        {
            await DisplayAlert("Wait!", "Please enter both a tag and a detail.", "OK");
            return;
        }

        var newTask = new ToDoClass
        {
            item_name = nameEntry.Text,          // Updated to match your ToDoClass
            item_description = descEntry.Text,   // Updated to match your ToDoClass
            status = "Pending",
            user_id = _currentUserId
        };

        await _dbHelper.SaveToDo(newTask);
        ToDos.Add(newTask);
        ClearInputs();
    }

    private void TriggerEditMode(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass item)
        {
            _selectedToDo = item;
            nameEntry.Text = item.item_name;         // Updated
            descEntry.Text = item.item_description;  // Updated

            addBtn.IsVisible = false;
            editBtn.IsVisible = true;
            cancelBtn.IsVisible = true;
        }
    }

    private async void SaveEditItem(object sender, EventArgs e)
    {
        if (_selectedToDo != null)
        {
            _selectedToDo.item_name = nameEntry.Text;         // Updated
            _selectedToDo.item_description = descEntry.Text;  // Updated
            await _dbHelper.SaveToDo(_selectedToDo);
            CancelEdit(null, null);
        }
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        ClearInputs();
        addBtn.IsVisible = true;
        editBtn.IsVisible = false;
        cancelBtn.IsVisible = false;
    }

    private async void DeleteToDoItem(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ToDoClass itemToDelete)
        {
            await _dbHelper.DeleteToDo(itemToDelete);
            ToDos.Remove(itemToDelete);

            if (_selectedToDo == itemToDelete) CancelEdit(null, null);
        }
    }

    // This is the method that fires when the user checks the box to mark a task as completed. It updates the status in the database, and then removes it from this list (since this list only shows pending tasks).
    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoClass task)
        {
            // We only check if the box is checked (e.Value is true)
            if (e.Value)
            {
                task.status = "Completed"; // changes the task.status from "Pending" to "Completed"
                await _dbHelper.SaveToDo(task); // Save to database
                ToDos.Remove(task); // Remove it from this tab
            }
        }
    }

    private void ClearInputs()
    {
        nameEntry.Text = string.Empty;
        descEntry.Text = string.Empty;
        _selectedToDo = null;
    }
}