namespace HourseLB3;

public static class UserService
{
    // Хранит логин текущего пользователя
    public static string CurrentUser { get; set; }

    // Метод для получения имени файла текущего пользователя
    public static string GetFileName() => $"houses_{CurrentUser}.json";
}