using HourseLB3.Models;

namespace HourseLB3;

public partial class MainPage : ContentPage
{
    private ResidentialBuilding _item;
    private bool _isNew;

    public MainPage(ResidentialBuilding item)
    {
        InitializeComponent();

        // Заполняем выпадающий список материалами
        pMat.ItemsSource = Enum.GetValues(typeof(BuildingMaterial));

        _isNew = item == null;
        _item = item ?? new ResidentialBuilding();

        if (!_isNew)
        {
            LoadDataToUI();
        }
    }

    // Заполняем поля интерфейса данными из модели
    private void LoadDataToUI()
    {
        eAddr.Text = _item.Address;
        pDate.Date = _item.ConstructionDate;
        pTime.Time = _item.InspectionTime;
        sLift.IsToggled = _item.HasElevator;
        pMat.SelectedItem = _item.Material;
        imgV.Source = _item.ImagePath;
    }

    // Кнопка сохранения
    private async void OnSave(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(eAddr.Text))
        {
            await DisplayAlert("Ошибка", "Введите адрес дома", "OK");
            return;
        }

        UpdateModelFromUI();

        // Получаем имя файла текущего пользователя
        string fileName = UserService.GetFileName();

        // Загружаем список домов пользователя
        var currentData = DataService.Load<ResidentialBuilding>(fileName);

        if (_isNew)
        {
            currentData.Add(_item);
        }
        else
        {
            // Обновляем существующую запись
            var index = currentData.FindIndex(x => x.Address == _item.Address);
            if (index != -1)
                currentData[index] = _item;
            else
                currentData.Add(_item);
        }

        // Сохраняем обратно в файл
        DataService.Save(currentData, fileName);

        await Navigation.PopAsync();
    }

    // Переносим данные из UI в объект _item
    private void UpdateModelFromUI()
    {
        _item.Address = eAddr.Text;
        _item.ConstructionDate = (DateTime)pDate.Date;
        _item.InspectionTime = (TimeSpan)pTime.Time;
        _item.HasElevator = sLift.IsToggled;

        if (pMat.SelectedItem != null)
        {
            _item.Material = (BuildingMaterial)pMat.SelectedItem;
        }
    }

    private async void OnExportSingleClicked(object sender, EventArgs e)
    {
        UpdateModelFromUI();

        // Запрос формата у пользователя
        string format = await DisplayActionSheet("Выберите формат:", "Отмена", null, ".html", ".pdf", ".docx", ".xlsx");

        if (format != "Отмена" && format != null)
        {
            await ReportService.ExportAsync("single", _item, format);
        }
    }

    // Выбор фотографии
    private async void OnImg(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите фото дома",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                _item.ImagePath = result.FullPath;
                imgV.Source = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выбрать фото: {ex.Message}", "OK");
        }
    }
}