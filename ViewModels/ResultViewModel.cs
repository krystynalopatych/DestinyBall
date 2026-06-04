using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DestinyBall.Models;
using System.Windows;

namespace DestinyBall.ViewModels
{
    public partial class ResultViewModel : ObservableObject
    {
        private readonly GameModel _model;

        [ObservableProperty] private string _playerName = "";
        [ObservableProperty] private string _botName = "Бот Судьбыч";

        [ObservableProperty] private string _playerName2 = "?";
        [ObservableProperty] private string _playerAge = "?";
        [ObservableProperty] private string _playerJob = "?";
        [ObservableProperty] private string _playerHouse = "?";
        [ObservableProperty] private string _playerCar = "?";
        [ObservableProperty] private string _playerPartner = "?";
        [ObservableProperty] private string _playerKids = "?";
        [ObservableProperty] private string _playerHobby = "?";

        [ObservableProperty] private string _botNameFate = "?";
        [ObservableProperty] private string _botAge = "?";
        [ObservableProperty] private string _botJob = "?";
        [ObservableProperty] private string _botHouse = "?";
        [ObservableProperty] private string _botCar = "?";
        [ObservableProperty] private string _botPartner = "?";
        [ObservableProperty] private string _botKids = "?";
        [ObservableProperty] private string _botHobby = "?";

        [ObservableProperty] private string _copyButtonText = "📋 Скопировать судьбу";

        public ResultViewModel(GameModel model)
        {
            _model = model;
            PlayerName = model.PlayerName;

            string Get(Dictionary<GameCategory, string> d, GameCategory k) =>
                d.TryGetValue(k, out var v) ? v : "???";

            PlayerName2 = Get(model.PlayerDestiny, GameCategory.Name);
            PlayerAge = Get(model.PlayerDestiny, GameCategory.Age);
            PlayerJob = Get(model.PlayerDestiny, GameCategory.Job);
            PlayerHouse = Get(model.PlayerDestiny, GameCategory.House);
            PlayerCar = Get(model.PlayerDestiny, GameCategory.Car);
            PlayerPartner = Get(model.PlayerDestiny, GameCategory.Partner);
            PlayerKids = Get(model.PlayerDestiny, GameCategory.Kids);
            PlayerHobby = Get(model.PlayerDestiny, GameCategory.Hobby);

            BotNameFate = Get(model.BotDestiny, GameCategory.Name);
            BotAge = Get(model.BotDestiny, GameCategory.Age);
            BotJob = Get(model.BotDestiny, GameCategory.Job);
            BotHouse = Get(model.BotDestiny, GameCategory.House);
            BotCar = Get(model.BotDestiny, GameCategory.Car);
            BotPartner = Get(model.BotDestiny, GameCategory.Partner);
            BotKids = Get(model.BotDestiny, GameCategory.Kids);
            BotHobby = Get(model.BotDestiny, GameCategory.Hobby);
        }

        [RelayCommand]
        private void CopyResults()
        {
            string text = $"""
                🔮 ШАР СУДЬБЫ — РЕЗУЛЬТАТЫ 🔮

                👤 {PlayerName}:
                  🏷️  Имя: {PlayerName2}
                  🎂  Возраст: {PlayerAge}
                  💼  Работа: {PlayerJob}
                  🏠  Дом: {PlayerHouse}
                  🚗  Машина: {PlayerCar}
                  💑  Муж/Жена: {PlayerPartner}
                  👶  Дети: {PlayerKids}
                  🎮  Хобби: {PlayerHobby}

                🤖 {BotName}:
                  🏷️  Имя: {BotNameFate}
                  🎂  Возраст: {BotAge}
                  💼  Работа: {BotJob}
                  🏠  Дом: {BotHouse}
                  🚗  Машина: {BotCar}
                  💑  Муж/Жена: {BotPartner}
                  👶  Дети: {BotKids}
                  🎮  Хобби: {BotHobby}
                """;

            Clipboard.SetText(text);
            CopyButtonText = "✅ Скопировано!";
        }

        [RelayCommand]
        private void PlayAgain()
        {
            var start = new Views.StartScreen();
            start.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.ResultScreen)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}
