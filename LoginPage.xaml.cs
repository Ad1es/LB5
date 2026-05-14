using HourseLB3.Models; // Если подчеркнуто красным — проверьте User.cs
// using HourseLB3; // Возможно, User лежит прямо здесь?

namespace HourseLB3;

public partial class LoginPage : ContentPage
{
    // Если ошибка на слове User, значит проект не видит класс User
    public static List<User> UsersList = new();

    public LoginPage()
    {
        InitializeComponent();
        // Убедитесь, что DataService тоже находится в namespace HourseLB3
        UsersList = DataService.Load<User>("users.json");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var user = UsersList.FirstOrDefault(u => u.Username == UserEntry.Text && u.Password == PassEntry.Text);

        if (user != null)
        {
            // ЗАПОМИНАЕМ ПОЛЬЗОВАТЕЛЯ
            UserService.CurrentUser = user.Username;
            await Shell.Current.GoToAsync("//ListBuildingsPage");
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверные данные", "OK");
        }
    }

    private async void OnRegClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new RegistrationPage());
}
