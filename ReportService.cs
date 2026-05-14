using System.Text;
using System.IO;

namespace HourseLB3;

public static class ReportService
{
    // Новый метод сохранения напрямую в папку "Документы" (без окна "Поделиться")
    private static async Task SaveFileDirectlyAsync(string content, string fileName, string extension)
    {
        try
        {
            // Получаем путь к системной папке "Мои документы" на компьютере
            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Создаем отдельную папку для отчетов нашего приложения
            string appFolder = Path.Combine(docsFolder, "Отчеты_Дома");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            // Формируем полный путь к файлу
            string fullPath = Path.Combine(appFolder, fileName + extension);

            // Записываем файл на диск
            File.WriteAllText(fullPath, content, Encoding.UTF8);

            // Просто показываем окошко с подтверждением и путем
            await Application.Current.MainPage.DisplayAlert(
                "Успешно",
                $"Отчет сохранен в папку:\n{fullPath}",
                "Отлично!");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка сохранения", ex.Message, "OK");
        }
    }

    public static async Task ExportAsync(string reportType, object data, string extension)
    {
        string html = "<html><head><meta charset='utf-8'></head><body>";
        string fileName = "Report";

        if (reportType == "single" && data is ResidentialBuilding b)
        {
            fileName = $"House_{b.Address.Replace(" ", "_")}";
            html += $@"<h1>Карточка объекта</h1>
                       <p><b>Адрес:</b> {b.Address}</p>
                       <p><b>Материал:</b> {b.Material}</p>
                       <p><b>Лифт:</b> {(b.HasElevator ? "Есть" : "Нет")}</p>";
        }
        else if (reportType == "list" && data is List<ResidentialBuilding> list)
        {
            fileName = "Houses_List";
            html += "<h1>Список выбранных домов</h1><table border='1' cellspacing='0' cellpadding='5'><tr><th>Адрес</th><th>Материал</th></tr>";
            foreach (var h in list) html += $"<tr><td>{h.Address}</td><td>{h.Material}</td></tr>";
            html += "</table>";
        }
        else if (reportType == "stats" && data is List<ResidentialBuilding> stats)
        {
            fileName = "Integral_Statistics";
            int total = stats.Count;
            int liftCount = stats.Count(x => x.HasElevator);
            html += $@"<h1>Интегральные характеристики</h1>
                       <p>Общее количество: {total}</p>
                       <p>С лифтом: {liftCount}</p>
                       <p>Без лифта: {total - liftCount}</p>
                       <p>Доля с лифтом: {(total > 0 ? (double)liftCount / total * 100 : 0):F1}%</p>";
        }

        html += "</body></html>";

        // Вызываем наш новый тихий метод сохранения
        await SaveFileDirectlyAsync(html, fileName, extension);
    }
}