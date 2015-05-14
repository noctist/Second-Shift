using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using SecondShiftMobile;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Game1 _game;

        TaskCompletionSource<string> tcs;
        // Constructor
        public GamePage()
        {
            InitializeComponent();

            _game = XamlGame<Game1>.Create("", this);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        public Task<string> EnterText()
        {
            textGrid.Visibility = System.Windows.Visibility.Visible;
            tcs = new TaskCompletionSource<string>();
            textBox.Focus();
            return tcs.Task;
        }

        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textGrid.Visibility = System.Windows.Visibility.Collapsed;
                    tcs.TrySetResult(textBox.Text.Trim());
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textGrid.Visibility = System.Windows.Visibility.Collapsed;
                tcs.TrySetResult(textBox.Text.Trim());
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}