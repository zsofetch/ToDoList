using SQLite;

namespace ToDoMaui_Listview;

public class UserClass
{
    [PrimaryKey, AutoIncrement]
    public int user_id { get; set; }

    [Unique]
    public string email { get; set; } // Added Email

    public string username { get; set; }
    public string password { get; set; }
}