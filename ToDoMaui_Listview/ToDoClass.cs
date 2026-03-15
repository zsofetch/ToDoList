using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDoMaui_Listview;

public class ToDoClass : INotifyPropertyChanged
{
    public ToDoClass() { }

    int _item_id;
    string _item_name;
    string _item_description;
    string _status;
    int _user_id;

    [PrimaryKey, AutoIncrement]
    public int item_id
    {
        get { return _item_id; }
        set { _item_id = value; OnPropertyChanged(nameof(item_id)); }
    }
    public string item_name
    {
        get { return _item_name; }
        set { _item_name = value; OnPropertyChanged(nameof(item_name)); }
    }
    public string item_description
    {
        get { return _item_description; }
        set { _item_description = value; OnPropertyChanged(nameof(item_description)); }
    }
    public string status
    {
        get { return _status; }
        set
        {
            _status = value;
            OnPropertyChanged(nameof(status));
            OnPropertyChanged(nameof(IsCompleted)); // Notify CheckBox to update
        }
    }
    public int user_id
    {
        get { return _user_id; }
        set { _user_id = value; OnPropertyChanged(nameof(user_id)); }
    }

    // This property bridges the gap between your string status and the CheckBox
    [Ignore]
    public bool IsCompleted
    {
        get { return status == "Completed"; }
        set { status = value ? "Completed" : "Pending"; }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}