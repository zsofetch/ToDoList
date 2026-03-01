//teeest
using System.Collections.ObjectModel;

namespace ToDoMaui_Listview;

public partial class MainPage : ContentPage
{
    public ObservableCollection<ToDoClass> ToDos { get; set; } = new ObservableCollection<ToDoClass>();
    private ToDoClass? _selectedToDo;
    private int _nextId = 1;

    public MainPage()
    {
        InitializeComponent();
        todoCV.ItemsSource = ToDos;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void AddToDoItem(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleEntry.Text) || string.IsNullOrWhiteSpace(detailsEntry.Text))
        {
            await DisplayAlert("Wait!", "Please enter both a tag and a detail.", "OK");
            return;
        }

        ToDos.Add(new ToDoClass
        {
            id = _nextId++,
            title = titleEntry.Text,
            detail = detailsEntry.Text,
            IsCompleted = false
        });

        ClearInputs();
    }

    // handles the user clicking the Edit button on a task
    private void OnEditIconClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && int.TryParse(button.ClassId, out int idToEdit))
        {
            var itemToEdit = ToDos.FirstOrDefault(t => t.id == idToEdit);
            if (itemToEdit != null)
            {
                // set the active item and populate the entry fields
                _selectedToDo = itemToEdit;
                titleEntry.Text = itemToEdit.title;
                detailsEntry.Text = itemToEdit.detail;

                // change the UI to show the save/cancel buttons
                SetEditMode(true);
            }
        }
    }

    // saves the changes
    private void EditToDoItem(object? sender, EventArgs e)
    {
        if (_selectedToDo != null)
        {
            _selectedToDo.title = titleEntry.Text;
            _selectedToDo.detail = detailsEntry.Text;
            CancelEdit(null, null);
        }
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        ClearInputs();
        SetEditMode(false);
    }

    private void DeleteToDoItem(object? sender, EventArgs e)
    {
        if (sender is Button button && int.TryParse(button.ClassId, out int idToDelete))
        {
            var itemToDelete = ToDos.FirstOrDefault(t => t.id == idToDelete);
            if (itemToDelete != null)
            {
                ToDos.Remove(itemToDelete);
                if (_selectedToDo == itemToDelete) CancelEdit(null, null);
            }
        }
    }

    private void ClearInputs()
    {
        titleEntry.Text = string.Empty;
        detailsEntry.Text = string.Empty;
        _selectedToDo = null;
    }

    private void SetEditMode(bool isEditing)
    {
        addBtn.IsVisible = !isEditing;
        editBtn.IsVisible = isEditing;
        cancelBtn.IsVisible = isEditing;
        Grid.SetColumnSpan(addBtn, isEditing ? 1 : 2);
    }
}