namespace DestinyBall.Models
{
    public class GameModel
    {
        public string PlayerName { get; set; } = "Игрок";
        public string BotName { get; set; } = "Бот Судьбыч";
        public PlayerRole PlayerRole { get; set; } = PlayerRole.Thrower;

        public Dictionary<GameCategory, string> PlayerDestiny { get; } = new();
        public Dictionary<GameCategory, string> BotDestiny { get; } = new();

        public GameCategory CurrentCategory { get; set; } = GameCategory.Name;
        public int CategoryIndex { get; set; } = 0;

        // Whose turn is it to receive destiny this round?
        // We alternate: player first, then bot, through all 8 categories each
        // Actually both get all 8, we process player first then bot
        public bool ProcessingPlayer { get; set; } = true;

        public bool IsGameOver => PlayerDestiny.Count == 8 && BotDestiny.Count == 8;

        private static readonly GameCategory[] CategoryOrder =
        {
            GameCategory.Name, GameCategory.Age, GameCategory.Job, GameCategory.House,
            GameCategory.Car, GameCategory.Partner, GameCategory.Kids, GameCategory.Hobby
        };

        public GameCategory GetCurrentCategory()
        {
            return CategoryOrder[CategoryIndex % 8];
        }

        public void AdvanceCategory()
        {
            CategoryIndex++;
            CurrentCategory = GetCurrentCategory();
        }

        public string GetCategoryEmoji(GameCategory cat)
        {
            return cat switch
            {
                GameCategory.Name => "🏷️",
                GameCategory.Age => "🎂",
                GameCategory.Job => "💼",
                GameCategory.House => "🏠",
                GameCategory.Car => "🚗",
                GameCategory.Partner => "💑",
                GameCategory.Kids => "👶",
                GameCategory.Hobby => "🎮",
                _ => "❓"
            };
        }
    }
}
