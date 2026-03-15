namespace ToDoMaui_Listview;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(string username)
    {
        InitializeComponent();
        usernameLabel.Text = $"Hello, {username}!";
    }

    private void OnSignOutClicked(object sender, EventArgs e)
    {
        // Dismantle the TabbedPage and return to the Welcome screen
        Application.Current.MainPage = new NavigationPage(new WelcomePage());
    }
}