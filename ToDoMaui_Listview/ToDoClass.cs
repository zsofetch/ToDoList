using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization; // Added for API

namespace ToDoMaui_Listview;

public class ToDoClass : INotifyPropertyChanged
{
    int _item_id;
    string _item_name;
    string _item_description;
    string _status;
    int _user_id;

    public int item_id
    {
        get => _item_id;
        set { _item_id = value; OnPropertyChanged(); }
    }

    public string item_name
    {
        get => _item_name;
        set { _item_name = value; OnPropertyChanged(); }
    }

    public string item_description
    {
        get => _item_description;
        set { _item_description = value; OnPropertyChanged(); }
    }

    public string status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCompleted)); // Notify CheckBox
        }
    }

    public int user_id
    {
        get => _user_id;
        set { _user_id = value; OnPropertyChanged(); }
    }

    // Tells the JSON converter to ignore this property when sending data to the API
    [JsonIgnore]
    public bool IsCompleted
    {
        get => status == "inactive";
        set => status = value ? "inactive" : "active";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}