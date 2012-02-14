using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KinectMenu
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Home : KinectPage, System.Windows.Markup.IComponentConnector
    {

		public Home()
		{
            InitializeComponent();
            kinectRightButton.Click += new RoutedEventHandler(kinectButton_Clicked);
            kinectLeftButton.Click += new RoutedEventHandler(kinectButton_Clicked);
		}

		protected override void InitializeButtons()
		{
			buttons = new List<Button>
			    {
			        new_game, 
					load_game, 
					options,
                    quit
			    };
		}

        protected override void InitializeVideoImage()
        {
            liveVideo = videoImage;
        }

        protected override void InitializeKinectHandIcons()
        {
            kinectLeft = kinectLeftButton;
            kinectRight = kinectRightButton;
        }

		void kinectButton_Clicked(object sender, RoutedEventArgs e)
		{
			//call the click event of the selected button
			_selectedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selectedButton));
		}

		private void new_game_Click(object sender, RoutedEventArgs e)
		{
            //MessageBox.Show("New Game Clicked");
            inAction = true;
            this.NavigationService.Navigate(new New_Game());
		}

		private void load_game_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
            this.NavigationService.Navigate(new Load_Game());
		}

		private void options_Click(object sender, RoutedEventArgs e)
		{
            //MessageBox.Show("Options Clicked");
            inAction = true;
            this.NavigationService.Navigate(new Options());
		}

		private void quit_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
			MessageBox.Show("Quit Clicked");
		}
	}
}
