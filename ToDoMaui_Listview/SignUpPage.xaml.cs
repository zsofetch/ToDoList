namespace ToDoMaui_Listview;

public partial class SignUpPage : ContentPage
{
    private DatabaseHelper _dbHelper = new DatabaseHelper();

    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        try
        {
            // preevents null/empty crashes
            if (string.IsNullOrWhiteSpace(userEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text) ||
                string.IsNullOrWhiteSpace(passEntry.Text))
            {
                await DisplayAlert("Wait", "Please fill out all fields before signing up.", "OK");
                return;
            }

            // check if passwords match
            if (passEntry.Text != confirmPassEntry.Text)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var newUser = new UserClass
            {
                username = userEntry.Text,
                email = emailEntry.Text,
                password = passEntry.Text
            };

            // attempt to save to the database
            bool success = await _dbHelper.RegisterUser(newUser);

            //if everything looks good, it packages that data into a UserClass object and hands it to the DatabaseHelper to save.
            //If the email is already taken, the database rejects it, and the page shows an error alert.
            if (success)
            {
                await DisplayAlert("Success!", "Account created safely.", "OK");
                await Navigation.PopAsync(); // Send back to login
            }
            else
            {
                await DisplayAlert("Error", "An account with that email already exists.", "OK");
            }
        }
        catch (Exception ex)
        {
            // if SQLite throws an error, it will show up here instead of closing the app!
            await DisplayAlert("System Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }

    private async void GoBackToLogin(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}