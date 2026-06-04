namespace DestinyBall.Models
{
    public static class WordLists
    {
        public static readonly string[] Names =
        {
            "Боб", "Бублик", "Витёк", "Снежинка", "Гоша",
            "Лариска", "Зефир", "Брутус", "Кузя", "Тамарочка", "Рыжик"
        };

        public static readonly string[] Ages =
        {
            "3", "7", "14", "19", "25", "33", "47", "62", "80", "99", "101", "0.5"
        };

        public static readonly string[] Jobs =
        {
            "аниматор в крематории", "тайный агент", "пастух котов",
            "менеджер среднего звена", "блогер про капусту", "испытатель диванов",
            "специалист по ничему", "охранник будки", "сомелье компотов"
        };

        public static readonly string[] Houses =
        {
            "шалаш", "замок", "хрущёвка", "плот", "автобус",
            "сарай", "палатка на даче", "вагончик", "землянка"
        };

        public static readonly string[] Cars =
        {
            "Ока 96 года", "верблюд", "скутер без колеса",
            "машина соседа", "трамвай", "телега", "велосипед с мотором"
        };

        public static readonly string[] Partners =
        {
            "Геннадий из Тулы", "никто", "сосед Жора",
            "призрак бывшего", "Илон", "голос из телевизора", "загадочная незнакомка"
        };

        public static readonly string[] Kids =
        {
            "0", "1", "2", "3", "5", "0 но 4 кота", "целый детский сад", "тройня"
        };

        public static readonly string[] Hobbies =
        {
            "коллекционирование пакетов", "ночные крики в подушку",
            "разговоры с холодильником", "йога раз в год",
            "слежка за соседями", "просто лежать"
        };

        public static string[] GetWordsForCategory(GameCategory category)
        {
            return category switch
            {
                GameCategory.Name => Names,
                GameCategory.Age => Ages,
                GameCategory.Job => Jobs,
                GameCategory.House => Houses,
                GameCategory.Car => Cars,
                GameCategory.Partner => Partners,
                GameCategory.Kids => Kids,
                GameCategory.Hobby => Hobbies,
                _ => Names
            };
        }

        public static string GetCategoryDisplayName(GameCategory category)
        {
            return category switch
            {
                GameCategory.Name => "🏷️ Имя",
                GameCategory.Age => "🎂 Возраст",
                GameCategory.Job => "💼 Работа",
                GameCategory.House => "🏠 Дом",
                GameCategory.Car => "🚗 Машина",
                GameCategory.Partner => "💑 Муж/Жена",
                GameCategory.Kids => "👶 Дети",
                GameCategory.Hobby => "🎮 Хобби",
                _ => "???"
            };
        }
    }

    public enum GameCategory
    {
        Name,
        Age,
        Job,
        House,
        Car,
        Partner,
        Kids,
        Hobby
    }

    public enum PlayerRole
    {
        Thrower,
        Catcher
    }
}
