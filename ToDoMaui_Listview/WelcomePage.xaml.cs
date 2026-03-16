

namespace ToDoMaui_Listview;

public partial class WelcomePage : ContentPage
{
    private DatabaseHelper _dbHelper = new DatabaseHelper();

    public WelcomePage()
    {
        InitializeComponent();
    }

    //When the user clicks "Login", it sends the email and password to the database.
    //If the database gives the thumbs-up, it takes the logged-in user's user_id and passes it into the MainTabbedPage. 
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(emailEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text)) return;

        var user = await _dbHelper.AuthenticateUser(emailEntry.Text, passwordEntry.Text);

        if (user != null)
        {
            passwordEntry.Text = string.Empty;
            // Send user to the Tabbed App layout
            Application.Current.MainPage = new MainTabbedPage(user.user_id, user.username);
        }
        else
        {
            // The Logic you requested: Check if it's a wrong password, or if the account doesn't exist at all
            bool emailExists = await _dbHelper.CheckEmailExists(emailEntry.Text);
            if (!emailExists)
                await DisplayAlert("Notice", "No account created yet. Please sign up.", "OK");
            else
                await DisplayAlert("Error", "Incorrect password.", "OK");
        }
    }

    private async void GoToSignUp(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}