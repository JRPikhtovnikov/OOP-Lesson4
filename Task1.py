import json


def open_file(file_name: str):
    with open(file_name, 'r', encoding='utf-8') as file:
        library = json.load(file)
    return library


def residents_by_flat(library: dict, street: str, house: str, floor: str, apartment: str):
    try:
        residents = library[street]["houses"][house]["floors"][floor]["apartments"][apartment]["residents"]
        return f"В квартире {apartment} на улице {street}, дом {house}, этаж {floor} проживает {residents} жителей."
    except KeyError:
        return "Указанное местоположение не найдено."


def max_residents_by_flat(library: dict, street: str = None, house: str = None, floor: str = None):
    max_residents = 0
    location = None

    for s, street_data in library.items():
        if street and s != street:
            continue

        for h, house_data in street_data["houses"].items():
            if house and h != house:
                continue

            for f, floor_data in house_data["floors"].items():
                if floor and f != floor:
                    continue

                for a, apartment_data in floor_data["apartments"].items():
                    num_residents = apartment_data.get("residents", 0)
                    if num_residents > max_residents:
                        max_residents = num_residents
                        location = {"street": s, "house": h, "floor": f, "apartment": a}

    if location:
        return (f"Максимальное количество жителей в квартире: {max_residents}\n"
                f"Местоположение: Улица {location['street']}, Дом {location['house']}, "
                f"Этаж {location['floor']}, Квартира {location['apartment']}")
    else:
        return "Не найдено подходящих квартир с жителями."


def street_with_max_residents(library: dict):
    max_residents = 0
    max_street = None

    for street, street_data in library.items():
        street_residents = 0
        for house_data in street_data["houses"].values():
            for floor_data in house_data["floors"].values():
                for apartment_data in floor_data["apartments"].values():
                    street_residents += apartment_data.get("residents", 0)

        if street_residents > max_residents:
            max_residents = street_residents
            max_street = street

    if max_street:
        return (f"Улица с наибольшим числом жителей: {max_street}\n"
                f"Общее количество жителей: {max_residents}")
    else:
        return "Не найдено улиц с жителями."


def house_with_max_residents(library: dict, street: str = None):
    max_residents = 0
    max_house_location = None

    for s, street_data in library.items():
        if street and s != street:
            continue

        for h, house_data in street_data["houses"].items():
            house_residents = 0
            for floor_data in house_data["floors"].values():
                for apartment_data in floor_data["apartments"].values():
                    house_residents += apartment_data.get("residents", 0)

            if house_residents > max_residents:
                max_residents = house_residents
                max_house_location = {"street": s, "house": h}

    if max_house_location:
        return (f"Дом с максимальным количеством жителей: {max_house_location['house']}, "
                f"Улица {max_house_location['street']}\n"
                f"Общее количество жителей в доме: {max_residents}")
    else:
        return "Не найдено домов с жителями."


def user_action(library: dict):
    actions = {
        "1": lambda: residents_by_flat(
            library,
            input("Введите улицу: "),
            input("Введите дом: "),
            input("Введите этаж: "),
            input("Введите квартиру: ")
        ),
        "2": lambda: max_residents_by_flat(
            library,
            street=input("Введите улицу (оставьте пустым для всех улиц): ") or None,
            house=input("Введите дом (оставьте пустым для всех домов): ") or None,
            floor=input("Введите этаж (оставьте пустым для всех этажей): ") or None
        ),
        "3": lambda: street_with_max_residents(library),
        "4": lambda: house_with_max_residents(
            library,
            street=input("Введите улицу (оставьте пустым для поиска по всем улицам): ") or None
        )
    }

    while True:
        print("\nВыберите действие:\n"
              "1. Узнать, сколько жителей в квартире\n"
              "2. Вывести макс. количество жителей на уровне улицы/дома/этажа\n"
              "3. Найти улицу с максимальным количеством жителей\n"
              "4. Найти дом с максимальным количеством жителей\n"
              "0. Выйти из программы\n")

        choice = input("Введите номер действия: ")

        if choice == "0":
            print("Выход из программы. До свидания!")
            break
        elif choice in actions:
            result = actions[choice]()
            print(f"\nРезультат:\n{result}\n")
        else:
            print("Некорректный ввод. Попробуйте снова.")


def main():
    library = open_file("city.json")
    user_action(library)


if __name__ == '__main__':
    main()
