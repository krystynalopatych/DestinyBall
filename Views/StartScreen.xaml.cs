using DestinyBall.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DestinyBall.Views
{
    public partial class StartScreen : Window
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void ThrowerBorder_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StartScreenViewModel vm)
            {
                vm.IsThrowerSelected = true;
            }
        }

        private void CatcherBorder_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StartScreenViewModel vm)
            {
                vm.IsCatcherSelected = true;
            }
        }
    }
}
