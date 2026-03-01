namespace ToDoMaui_Listview;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // wrap the WelcomePage in a NavigationPage
        MainPage = new NavigationPage(new WelcomePage());
    }
}