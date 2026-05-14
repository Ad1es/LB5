using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace HourseLB3;

public static class DataService  // Убедись, что есть public
{
    public static void Save<T>(List<T> data, string fileName)
    {
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(fileName, json);
    }

    public static List<T> Load<T>(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }
}