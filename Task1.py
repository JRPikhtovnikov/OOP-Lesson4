import json

def open_file(file_name: str):
    with open(file_name, 'r', encoding='utf-8') as file:
        library = json.load(file)
    return library


def residents_by_flat(library: list, street: str, house: str, floor: str, apartment: str):
    return library[street]["houses"][house]["floors"][floor]["apartments"][apartment]["residents"]


def max_residents_by_flat(library: list, street: str = None, house: str = None, floor: str = None, apartment: str = None):
    return library, street, house


def books_by_genre(library: list, genre: str):
    pass


def user_action(library: list):
    actions = {
        "1": lambda: residents_by_flat(library, input("Введите улицу: "), input("Введите дом: "), input("Введите этаж: "), input("Введите квартиру: ")),
        "2": lambda: max_residents_by_flat(library, street=None, house = None, floor = None, apartment = None)
    }
    print("Выберите действие:\n"
          "1. Узнать, сколько жителей в квартире\n"


    while True:
        choice = input("Введите номер действия: ")

        if choice in actions:
            result = actions[choice]()
            if result:
                print(f"Результат: {result}")
                break
        else:
            print("Некорректный ввод. Попробуйте снова.")

def main():
    # user_action(open_file("city.json"))
    max_residents_by_flat(open_file("city.json"))

if __name__ == '__main__':
    main()
