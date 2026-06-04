using DestinyBall.Models;
using DestinyBall.ViewModels;
using System.Windows;

namespace DestinyBall.Views
{
    public partial class ResultScreen : Window
    {
        public ResultScreen(GameModel model)
        {
            InitializeComponent();
            DataContext = new ResultViewModel(model);
        }
    }
}
