﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for EndWindow.xaml
    /// </summary>
    public partial class EndWindow : Page
    {
        //Check whether the user won or lost
        private bool currentPlayerWon;
        private GameClient gc = GameClient.GetInstance();

        public EndWindow()
        {
            InitializeComponent();
            currentPlayerWon = (gc.GetGameState().getResult() == Util.myPlayerNum());

            //Generate the text to indicate won or lost
            if (currentPlayerWon)
            {
                gameResultWon.Visibility = Visibility.Visible;
            }else
            {
                gameResultLost.Visibility = Visibility.Visible;
            }
        }

        //Go back to the gamebrowser window
        protected void navigateToGameBrowserWindow(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.NavigationService.Navigate(new Uri("GameBrowserWindow.xaml", UriKind.Relative));
        }

        //exit the game
        protected void CloseGame(object sender, RoutedEventArgs e)
        {
            gc.Disconnect();
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }
}
