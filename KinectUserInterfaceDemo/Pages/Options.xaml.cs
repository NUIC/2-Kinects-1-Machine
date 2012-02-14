using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Coding4Fun.Kinect.Wpf.Controls;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using System.IO;
using System.Windows.Navigation;

namespace KinectMenu
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Options : KinectPage, System.Windows.Markup.IComponentConnector
    {

		public Options()
		{
            InitializeComponent();
            kinectRightButton.Click += kinectButton_Clicked;
            kinectLeftButton.Click += kinectButton_Clicked;
		}

		protected override void InitializeButtons()
		{
			buttons = new List<Button>
			    {
			        difficulty, 
					volume, 
					screen_brightness,
                    cancel
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

        private void difficulty_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
            MessageBox.Show("Difficulty Clicked");
		}

        private void volume_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
			MessageBox.Show("Volume Clicked");
		}

        private void screen_brightness_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
            MessageBox.Show("Screen Brightness Clicked");
		}

		private void cancel_Click(object sender, RoutedEventArgs e)
		{
            inAction = true;
            this.NavigationService.GoBack();
		}
	}
}
