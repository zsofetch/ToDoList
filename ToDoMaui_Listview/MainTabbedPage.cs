namespace ToDoMaui_Listview;

//inherits the TabbedPage class, which is a built-in page type in .NET MAUI that allows for multiple tabs with different content.
//holds three children, MainPage, CompletedPage, ProfilePage and passes the user_id and username to those childern so they know whose data to display
public class MainTabbedPage : TabbedPage
{
    public MainTabbedPage(int userId, string username)
    {
        BarBackgroundColor = Color.FromArgb("#F5F0E6"); 
        SelectedTabColor = Color.FromArgb("#5C6B47"); 
        UnselectedTabColor = Color.FromArgb("#A0A0A0");

        // load the 3 pages and pass the user's data to them
        Children.Add(new MainPage(userId) { Title = "Home" });
        Children.Add(new CompletedPage(userId) { Title = "Completed" });
        Children.Add(new ProfilePage(username) { Title = "Profile" });
    }
}