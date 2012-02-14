using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KinectMenu
{
    /// <summary>
    /// Interaction logic for New_Game.xaml
    /// </summary>
    public partial class New_Game : KinectPage, System.Windows.Markup.IComponentConnector
    {

		public New_Game()
		{
            InitializeComponent();
            kinectRightButton_newGame.Click += new RoutedEventHandler(kinectButton_Clicked);
            kinectLeftButton_newGame.Click += new RoutedEventHandler(kinectButton_Clicked);
		}

        protected override void InitializeButtons()
        {
            buttons = new List<Button>
			    {
			        campaign, 
					zen_mode, 
					score_mode,
                    multiplayer,
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

        private void campaign_game_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Campaign Clicked");
        }

        private void zen_mode_game_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Zen Mode Clicked");
        }

        private void score_mode_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Score Mode Clicked");

        }

        private void multiplayer_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            MessageBox.Show("Multiplayer Clicked");
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            inAction = true;
            this.NavigationService.GoBack();
        }
	}
}
