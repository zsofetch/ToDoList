namespace ToDoMaui_Listview;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        // Navigate to the Main ToDo List Page
        await Navigation.PushAsync(new MainPage());
    }
}