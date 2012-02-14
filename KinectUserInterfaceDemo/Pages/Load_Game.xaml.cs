using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KinectMenu
{
    /// <summary>
    /// Interaction logic for Load_Game.xaml
    /// </summary>
    public partial class Load_Game : KinectPage, System.Windows.Markup.IComponentConnector
    {

        public Load_Game()
        {
            InitializeComponent();
            kinectRightButton_newGame.Click += new RoutedEventHandler(kinectButton_Clicked);
            kinectLeftButton_newGame.Click += new RoutedEventHandler(kinectButton_Clicked);
        }

        protected override void InitializeButtons()
        {
            buttons = new List<Button>
			    {
			        game_1, 
					game_2, 
					game_42,
                    game_666,
                    cancel
			    };
        }

        protected override void InitializeVideoImage()
        {
            liveVideo = videoImage;
        }

        protected override void InitializeKinectHandIcons()
        {
            kinectLeft = kinectLeftButton_newGame;
            kinectRight = kinectRightButton_newGame;
        }

        void kinectButton_Clicked(object sender, RoutedEventArgs e)
        {
            //call the click event of the selected button
            _selectedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selectedButton));
        }

        private void game_1_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Game 1 Clicked");
        }

        private void game_2_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Game 2 Clicked");
        }

        private void game_42_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Game 42 Clicked");

        }

        private void game_666_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Game 666 Clicked");
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            this.NavigationService.GoBack();
        }
    }
}
