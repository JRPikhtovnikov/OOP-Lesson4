using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static System.Console;

class Program {
    static void Main() {
        JsonElement library = OpenFile("../../../city.json");
        UserAction(library);
    }

    static JsonElement OpenFile(string fileName) {
        using (FileStream fs = File.OpenRead(fileName)) {
            JsonDocument doc = JsonDocument.Parse(fs);
            return doc.RootElement;
        }
    }

    static void UserAction(JsonElement library) {
        while (true) {
            WriteLine("\nВыберите действие:\n" +
                              "1. Узнать, сколько жителей в квартире\n" +
                              "2. Вывести макс. количество жителей на уровне улицы/дома/этажа\n" +
                              "3. Найти улицу с максимальным количеством жителей\n" +
                              "4. Найти дом с максимальным количеством жителей\n" +
                              "0. Выйти из программы\n");

            string choice = ReadLine();

            switch (choice) {
                case "0":
                    WriteLine("Выход из программы. До свидания!");
                    return;
                case "1":
                    WriteLine(ResidentsByFlat(library));
                    break;
                case "2":
                    WriteLine(MaxResidentsByFlat(library));
                    break;
                case "3":
                    WriteLine(StreetWithMaxResidents(library));
                    break;
                case "4":
                    WriteLine(HouseWithMaxResidents(library));
                    break;
                default:
                    WriteLine("Некорректный ввод. Попробуйте снова.");
                    break;
            }
        }
    }

    static string ResidentsByFlat(JsonElement library) {
        Write("Введите улицу: ");
        string street = ReadLine();
        Write("Введите дом: ");
        string house = ReadLine();
        Write("Введите этаж: ");
        string floor = ReadLine();
        Write("Введите квартиру: ");
        string apartment = ReadLine();

        try {
            int residents = library.GetProperty(street).GetProperty("houses").GetProperty(house)
                .GetProperty("floors").GetProperty(floor).GetProperty("apartments")
                .GetProperty(apartment).GetProperty("residents").GetInt32();

            return $"В квартире {apartment} на улице {street}, дом {house}, этаж {floor} проживает {residents} жителей.";
        }
        catch (KeyNotFoundException) {
            return "Указанное местоположение не найдено.";
        }
        catch (Exception ex) {
            return $"Произошла ошибка: {ex.Message}";
        }
    }

    static string MaxResidentsByFlat(JsonElement library, string street = null, string house = null, string floor = null) {
        int maxResidents = 0;
        Dictionary<string, string> location = null;

        foreach (JsonProperty s in library.EnumerateObject()) {
            if (street != null && s.Name != street) {
                continue;
            }

            foreach (JsonProperty h in s.Value.GetProperty("houses").EnumerateObject()) {
                if (house != null && h.Name != house) {
                    continue;
                }

                foreach (JsonProperty f in h.Value.GetProperty("floors").EnumerateObject()) {
                    if (floor != null && f.Name != floor) {
                        continue;
                    }

                    foreach (JsonProperty a in f.Value.GetProperty("apartments").EnumerateObject()) {
                        int numResidents = a.Value.GetProperty("residents").GetInt32();
                        if (numResidents > maxResidents) {
                            maxResidents = numResidents;
                            location = new Dictionary<string, string>
                            {
                                { "street", s.Name },
                                { "house", h.Name },
                                { "floor", f.Name },
                                { "apartment", a.Name }
                            };
                        }
                    }
                }
            }
        }

        if (location != null) {
            return $"Максимальное количество жителей в квартире: {maxResidents}\n" +
                   $"Местоположение: Улица {location["street"]}, Дом {location["house"]}, " +
                   $"Этаж {location["floor"]}, Квартира {location["apartment"]}";
        }
        else {
            return "Не найдено подходящих квартир с жителями.";
        }
    }

    static string StreetWithMaxResidents(JsonElement library) {
        int maxResidents = 0;
        string maxStreet = null;

        foreach (JsonProperty street in library.EnumerateObject()) {
            int streetResidents = 0;

            foreach (JsonProperty house in street.Value.GetProperty("houses").EnumerateObject()) {
                foreach (JsonProperty floor in house.Value.GetProperty("floors").EnumerateObject()) {
                    foreach (JsonProperty apartment in floor.Value.GetProperty("apartments").EnumerateObject()) {
                        streetResidents += apartment.Value.GetProperty("residents").GetInt32();
                    }
                }
            }

            if (streetResidents > maxResidents) {
                maxResidents = streetResidents;
                maxStreet = street.Name;
            }
        }

        if (maxStreet != null) {
            return $"Улица с наибольшим числом жителей: {maxStreet}\nОбщее количество жителей: {maxResidents}";
        }
        else {
            return "Не найдено улиц с жителями.";
        }
    }

    static string HouseWithMaxResidents(JsonElement library, string street = null) {
        int maxResidents = 0;
        string maxHouseLocation = null;

        foreach (JsonProperty s in library.EnumerateObject()) {
            if (street != null && s.Name != street) {
                continue;
            }

            foreach (JsonProperty h in s.Value.GetProperty("houses").EnumerateObject()) {
                int houseResidents = 0;

                foreach (JsonProperty floor in h.Value.GetProperty("floors").EnumerateObject()) {
                    foreach (JsonProperty apartment in floor.Value.GetProperty("apartments").EnumerateObject()) {
                        houseResidents += apartment.Value.GetProperty("residents").GetInt32();
                    }
                }

                if (houseResidents > maxResidents) {
                    maxResidents = houseResidents;
                    maxHouseLocation = $"{h.Name} на улице {s.Name}";
                }
            }
        }

        if (maxHouseLocation != null) {
            return $"Дом с максимальным количеством жителей: {maxHouseLocation}\n" +
                   $"Общее количество жителей в доме: {maxResidents}";
        }
        else {
            return "Не найдено домов с жителями.";
        }
    }
}
