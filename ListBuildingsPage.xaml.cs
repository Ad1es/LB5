using System.Collections.ObjectModel;
using HourseLB3.Models; // <--- ДОБАВЬТЕ ЭТУ СТРОКУ
public partial class ListBuildingsPage : ContentPage
{
    // Список больше не static, чтобы данные не перемешивались при смене аккаунта
    public List<ResidentialBuilding> AllData = new();
    public ObservableCollection<ResidentialBuilding> VisibleData { get; set; } = new();

    public ListBuildingsPage()
    {
        InitializeComponent();
        MatPicker.ItemsSource = Enum.GetValues(typeof(BuildingMaterial));
        ListCol.ItemsSource = VisibleData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Загружаем данные из персонального файла пользователя
        AllData = DataService.Load<ResidentialBuilding>(UserService.GetFileName());
        Apply();
    }

    // ТОТ САМЫЙ МЕТОД APPLY, КОТОРОГО НЕ ХВАТАЛО
    void Apply()
    {
        var filtered = AllData.Where(h =>
            (string.IsNullOrEmpty(SearchBar.Text) || h.Address.ToLower().Contains(SearchBar.Text.ToLower())) &&
            (MatPicker.SelectedItem == null || h.Material.Equals(MatPicker.SelectedItem)) &&
            (!LiftCheck.IsChecked || h.HasElevator)).ToList();

        VisibleData.Clear();
        foreach (var item in filtered)
            VisibleData.Add(item);
    }

    void OnFilterChanged(object s, EventArgs e) => Apply();

    void OnResetClicked(object s, EventArgs e)
    {
        SearchBar.Text = string.Empty;
        MatPicker.SelectedItem = null;
        LiftCheck.IsChecked = false;
        Apply();
    }

    async void OnLogoutClicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("Выход", "Выйти из аккаунта?", "Да", "Нет"))
        {
            AllData.Clear();
            VisibleData.Clear();
            UserService.CurrentUser = null;
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    async void OnAdd(object s, EventArgs e) => await Navigation.PushAsync(new MainPage(null));

    async void OnEdit(object s, EventArgs e)
    {
        var item = (ResidentialBuilding)((Button)s).CommandParameter;
        await Navigation.PushAsync(new MainPage(item));
    }

    // Метод для отчета (Список)
    // Отчет о нескольких объектах
    // Отчет о нескольких объектах (пункт 2 ТЗ)
    async void OnListReport(object s, EventArgs e)
    {
        string format = await DisplayActionSheet("Сохранить список как:", "Отмена", null, ".html", ".pdf", ".docx", ".xlsx");
        if (format != "Отмена" && format != null)
        {
            // Передаем только отфильтрованные (видимые) данные
            await ReportService.ExportAsync("list", VisibleData.ToList(), format);
        }
    }

    // Интегральные характеристики (пункт 3 ТЗ)
    async void OnStatsReport(object s, EventArgs e)
    {
        string format = await DisplayActionSheet("Сохранить статистику как:", "Отмена", null, ".html", ".pdf", ".docx", ".xlsx");
        if (format != "Отмена" && format != null)
        {
            await ReportService.ExportAsync("stats", VisibleData.ToList(), format);
        }
    }
}