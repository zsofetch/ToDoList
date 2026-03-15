using SQLite;

namespace ToDoMaui_Listview;

public class DatabaseHelper
{
    private SQLiteAsyncConnection _db;  //this is what finds a safe place on the drive and creates a file na todo_app_v3.db3

    public DatabaseHelper()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "todo_app_v3.db3");
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<UserClass>().Wait(); //creates user tables
        _db.CreateTableAsync<ToDoClass>().Wait(); //creates task tables
    }

    // --- USER METHODS ---
    public async Task<UserClass> AuthenticateUser(string email, string password)
    {
        return await _db.Table<UserClass>().Where(u => u.email == email && u.password == password).FirstOrDefaultAsync(); //checks if there is a user with the email and password, if there is it returns the user, if not it returns null
    }

    public async Task<bool> CheckEmailExists(string email)
    {
        var existing = await _db.Table<UserClass>().Where(u => u.email == email).FirstOrDefaultAsync();
        return existing != null;
    }

    public async Task<bool> RegisterUser(UserClass user)
    {
        if (await CheckEmailExists(user.email)) return false;
        await _db.InsertAsync(user);
        return true;
    }

    // --- TODO METHODS ---
    public async Task<List<ToDoClass>> GetTasksByStatus(int userId, string taskStatus) 
    {
        return await _db.Table<ToDoClass>().Where(t => t.user_id == userId && t.status == taskStatus).ToListAsync(); //checks which user is logged in and then checks the status of the tasks and returns a list of tasks with that status for that user
    }

    //if the task has an ID of 0 (meaning it's brand new), it InsertAsync (adds) it. If the task already has an ID (meaning the user edited the task and saved the changes), it UpdateAsync (overwrites) the existing row.

    public async Task<int> SaveToDo(ToDoClass todo)
    {
        if (todo.item_id != 0) return await _db.UpdateAsync(todo);
        else return await _db.InsertAsync(todo);
    }

    public async Task<int> DeleteToDo(ToDoClass todo)
    {
        return await _db.DeleteAsync(todo);
    }
}