using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program {
    static JsonElement OpenFile(string fileName) {
        string jsonContent = File.ReadAllText(fileName);
        JsonDocument document = JsonDocument.Parse(jsonContent);
        return document.RootElement;
    }

    static string ResidentsByFlat(JsonElement library, string street, string house, string floor, string apartment) {
        try {
            int residents = library
                .GetProperty(street)
                .GetProperty("houses")
                .GetProperty(house)
                .GetProperty("floors")
                .GetProperty(floor)
                .GetProperty("apartments")
                .GetProperty(apartment)
                .GetProperty("residents")
                .GetInt32();

            return $"В квартире {apartment} на улице {street}, дом {house}, этаж {floor} проживает {residents} жителей.";
        }
        catch {
            return "Указанное местоположение не найдено.";
        }
    }

    static string MaxResidentsByFlat(JsonElement library, string street = null, string house = null, string floor = null) {
        int maxResidents = 0;
        Dictionary<string, string> location = null;

        foreach (JsonProperty s in library.EnumerateObject()) {
            if (street != null && s.Name != street) continue;

            foreach (JsonProperty h in s.Value.GetProperty("houses").EnumerateObject()) {
                if (house != null && h.Name != house) continue;

                foreach (JsonProperty f in h.Value.GetProperty("floors").EnumerateObject()) {
                    if (floor != null && f.Name != floor) continue;

                    foreach (JsonProperty a in f.Value.GetProperty("apartments").EnumerateObject()) {
                        int numResidents = a.Value.GetProperty("residents").GetInt32();
                        if (numResidents > maxResidents) {
                            maxResidents = numResidents;
                            location = new Dictionary<string, string> {
                                {"street", s.Name },
                                {"house", h.Name },
                                {"floor", f.Name },
                                {"apartment", a.Name }
                            };
                        }
                    }
                }
            }
        }

        if (location != null) {
            return $"Максимальное количество жителей в квартире: {maxResidents}\n" +
                   $"Местоположение: Улица {location["street"]}, Дом {location["house"]}, Этаж {location["floor"]}, Квартира {location["apartment"]}";
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
            return $"Улица с наибольшим числом жителей: {maxStreet}\n" +
                   $"Общее количество жителей: {maxResidents}";
        }
        else {
            return "Не найдено улиц с жителями.";
        }
    }

    static string HouseWithMaxResidents(JsonElement library, string street = null) {
        int maxResidents = 0;
        Dictionary<string, string> maxHouseLocation = null;

        foreach (JsonProperty s in library.EnumerateObject()) {
            if (street != null && s.Name != street) continue;

            foreach (JsonProperty h in s.Value.GetProperty("houses").EnumerateObject()) {
                int houseResidents = 0;
                foreach (JsonProperty floor in h.Value.GetProperty("floors").EnumerateObject()) {
                    foreach (JsonProperty apartment in floor.Value.GetProperty("apartments").EnumerateObject()) {
                        houseResidents += apartment.Value.GetProperty("residents").GetInt32();
                    }
                }

                if (houseResidents > maxResidents) {
                    maxResidents = houseResidents;
                    maxHouseLocation = new Dictionary<string, string> {
                        {"street", s.Name },
                        {"house", h.Name }
                    };
                }
            }
        }

        if (maxHouseLocation != null) {
            return $"Дом с максимальным количеством жителей: {maxHouseLocation["house"]}, " +
                   $"Улица {maxHouseLocation["street"]}\n" +
                   $"Общее количество жителей в доме: {maxResidents}";
        }
        else {
            return "Не найдено домов с жителями.";
        }
    }

    static void UserAction(JsonElement library) {
        while (true) {
            Console.WriteLine("\nВыберите действие:\n" +
                              "1. Узнать, сколько жителей в квартире\n" +
                              "2. Вывести макс. количество жителей на уровне улицы/дома/этажа\n" +
                              "3. Найти улицу с максимальным количеством жителей\n" +
                              "4. Найти дом с максимальным количеством жителей\n" +
                              "0. Выйти из программы\n");

            string choice = Console.ReadLine();

            switch (choice) {
                case "0":
                    Console.WriteLine("Выход из программы. До свидания!");
                    return;
                case "1":
                    Console.WriteLine("Введите улицу: ");
                    string street = Console.ReadLine();
                    Console.WriteLine("Введите дом: ");
                    string house = Console.ReadLine();
                    Console.WriteLine("Введите этаж: ");
                    string floor = Console.ReadLine();
                    Console.WriteLine("Введите квартиру: ");
                    string apartment = Console.ReadLine();
                    Console.WriteLine("\nРезультат:\n" + ResidentsByFlat(library, street, house, floor, apartment) + "\n");
                    break;
                case "2":
                    Console.WriteLine("Введите улицу (оставьте пустым для всех улиц): ");
                    street = Console.ReadLine();
                    Console.WriteLine("Введите дом (оставьте пустым для всех домов): ");
                    house = Console.ReadLine();
                    Console.WriteLine("Введите этаж (оставьте пустым для всех этажей): ");
                    floor = Console.ReadLine();
                    Console.WriteLine("\nРезультат:\n" + MaxResidentsByFlat(library, street, house, floor) + "\n");
                    break;
                case "3":
                    Console.WriteLine("\nРезультат:\n" + StreetWithMaxResidents(library) + "\n");
                    break;
                case "4":
                    Console.WriteLine("Введите улицу (оставьте пустым для поиска по всем улицам): ");
                    street = Console.ReadLine();
                    Console.WriteLine("\nРезультат:\n" + HouseWithMaxResidents(library, street) + "\n");
                    break;
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    break;
            }
        }
    }

    static void Main() {
        JsonElement library = OpenFile("../../../city.json");
        UserAction(library);
    }
}
