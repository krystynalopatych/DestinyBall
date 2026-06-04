using DestinyBall.Models;
using DestinyBall.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DestinyBall.Views
{
    public partial class GameScreen : Window
    {
        private readonly GameViewModel _vm;
        private Storyboard? _spinStoryboard;

        public GameScreen(GameModel model)
        {
            InitializeComponent();
            _vm = new GameViewModel(model);
            DataContext = _vm;

            // Wire up animation triggers
            _vm.StartThrowAnimation += OnStartThrowAnimation;

            Loaded += (s, e) =>
            {
                // Start ball spinning after elements are loaded
                _spinStoryboard = (Storyboard)Resources["SpinBallStoryboard"];
                _spinStoryboard.Begin(this, true);
            };
        }

        private void OnStartThrowAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                Storyboard sb;
                if (_vm.ThrowingToBot)
                    sb = (Storyboard)Resources["ThrowToBotStoryboard"];
                else
                    sb = (Storyboard)Resources["ThrowToPlayerStoryboard"];

                sb.Begin(this, true);
            });
        }

        private void ThrowToBotStoryboard_Completed(object sender, EventArgs e)
        {
            // Animation finished — ViewModel handles the outcome via its own timer
        }

        private void ThrowToPlayerStoryboard_Completed(object sender, EventArgs e)
        {
            // Same — VM timer controls outcome
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _vm.OnPlayerClick();
        }

        private void ShowResults_Click(object sender, RoutedEventArgs e)
        {
            var resultWindow = new ResultScreen(_vm.GetModel());
            resultWindow.Show();
            Close();
        }
    }
}
