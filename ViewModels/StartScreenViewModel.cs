using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DestinyBall.Models;
using DestinyBall.Views;
using System.Windows;

namespace DestinyBall.ViewModels
{
    public partial class StartScreenViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _playerName = "";

        [ObservableProperty]
        private bool _isThrowerSelected = true;

        [ObservableProperty]
        private bool _isCatcherSelected = false;

        [ObservableProperty]
        private string _nameError = "";

        partial void OnIsThrowerSelectedChanged(bool value)
        {
            if (value) IsCatcherSelected = false;
        }

        partial void OnIsCatcherSelectedChanged(bool value)
        {
            if (value) IsThrowerSelected = false;
        }

        [RelayCommand]
        private void StartGame()
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                NameError = "Введи своё имя!";
                return;
            }

            var model = new GameModel
            {
                PlayerName = PlayerName.Trim(),
                PlayerRole = IsThrowerSelected ? PlayerRole.Thrower : PlayerRole.Catcher
            };

            var gameWindow = new GameScreen(model);
            gameWindow.Show();

            // Close start screen
            foreach (Window w in Application.Current.Windows)
            {
                if (w is StartScreen)
                {
                    w.Close();
                    break;
                }
            }
        }

        partial void OnPlayerNameChanged(string value)
        {
            NameError = "";
        }
    }
}
