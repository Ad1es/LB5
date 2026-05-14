using HourseLB3.Models; // Добавляем using, чтобы страница видела класс User

namespace HourseLB3; // Убираем .Models отсюда!

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage() => InitializeComponent();

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(RegUser.Text) || string.IsNullOrWhiteSpace(RegPass.Text)) return;

        if (LoginPage.UsersList.Any(u => u.Username == RegUser.Text))
        {
            await DisplayAlert("Ошибка", "Логин занят", "OK");
            return;
        }

        LoginPage.UsersList.Add(new User { Username = RegUser.Text, Password = RegPass.Text, Email = RegEmail.Text });
        DataService.Save(LoginPage.UsersList, "users.json");

        await DisplayAlert("Успех", "Аккаунт создан", "OK");
        await Navigation.PopAsync();
    }
}