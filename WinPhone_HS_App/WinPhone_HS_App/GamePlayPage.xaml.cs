using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.UI.Core;
using Sphero.Devices;

namespace WinPhone_HS_App
{
    public partial class GamePlayPage : PhoneApplicationPage
    {
        private SpheroDevice _spheroDevice;

        private int _hits;
        private int _time;
        private bool waiting = true; 

        public GamePlayPage()
        {
            InitializeComponent();

        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_spheroDevice != null)
                _spheroDevice.SetBackLED(0.0f);

            spheroJoystick.Stop();
            if (App.CurrentConnection != null)
            {
                await App.CurrentConnection.Disconnect();
                App.CurrentConnection = null;
            }
            base.OnNavigatingFrom(e);
        }

        protected async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                //...has already started
                //upDate Game Object Start time
                //start timer
                //e.InGameActionMessageEvent(state)
                App.Current.CurrentGame = e.CustomGameObject;
                LivesLeftBlock.Text = "Lives Left: " + App.Current.CurrentGame.MaxHits;
                //StateBlock.Text = "State: " + App.Current.CurrentGame.GameState;
                //feed back from Ready 
                if (e.CustomGameObject.GameStatus < 4) //opp not ready
                {
                    // waitingBlock.Text = "Waiting for Opponent...";
                }
                else
                {
                    //Make visible in Visible
                    /*
                    image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    image2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    waitingBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    */
                    //Call Start if you were the last one ready

                }

                if (e.InGameActionMessageEvent.Action == "end")
                {
                    //Triger End event
                    if (App.Current.CurrentGame != null)
                    {
                        if (e.CustomGameObject.SpheroPlayer.UserName == App.Current.AppUser.UserName)
                        {
                            if (e.CustomGameObject.Winner.UserName == App.Current.AppUser.UserName)
                            {
                                App.Current.AppUser.GWAS++;
                            }
                            else
                            {
                                App.Current.AppUser.GLAS++;
                            }


                        }


                        App.Current.CurrentGame = null;
                        EndGame(e.CustomGameObject);
                    }

                }
                else
                {
                    switch (e.CustomGameObject.GameState)
                    {
                        //main state of game   
                        case 0:
                            if (e.InGameActionMessageEvent.Action == "start")
                            {
                                //STOP HERE!!!!!!!!
                                //if I'm a sphero display Warning
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    Message_Block.Text = "One minute before Targeting systems are active.";

                                }

                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    Message_Block.Text = "You have One minute to hide!";
                                }

                            }


                            if (e.InGameActionMessageEvent.Action == "hit")
                            {
                                _hits = e.InGameActionMessageEvent.Hits;
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    Message_Block.Text = "Hit Successful";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    Message_Block.Text = "You've been hit!";
                                    int x = e.CustomGameObject.MaxHits--;
                                    LivesLeftBlock.Text = "Lives left: " + x.ToString();
                                }

                            }
                            if (e.InGameActionMessageEvent.Action == "lost")
                            {
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    Message_Block.Text = "Lost Target!";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    Message_Block.Text = "Evaded the enemy";
                                }


                            }
                            //Actions - update GameObject - start Time, targeted, end
                            break;
                        //targeted state
                        case 1:
                            if (e.InGameActionMessageEvent.Action == "targeted")
                            {
                                //if I'm a sphero display Warning
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    Message_Block.Text = "Locked on Target";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    Message_Block.Text = "TARGETED!";
                                }
                            }
                            //Actions - fire, end(time), end(hits), lost

                            break;
                        //End State (show Winner message)
                        case 2:
                            //Current.Game = null

                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private async void EndGame(Game g)
        {
            string wm;
            ServerMessage sm = new ServerMessage(0, "update", "game ended");
            if (g.Winner.UserId == App.Current.AppUser.UserId)
            {
                wm = "Congrats, you WON!";
            }
            else
            {
                wm = "Sorry, you lost.";
            }

            MessageBoxResult result = MessageBox.Show(wm,
            "GAME OVER", MessageBoxButton.OK);

            if (result == MessageBoxResult.OK)
            {
                App.Current.SignalRHub.UserUpdate(App.Current.AppUser, sm);

                App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.Relative));

            }
        }

        private void spheroJoystick_Calibrating(object sender, Sphero.Controls.JoystickCalibrationEventArgs e)
        {
            if (_spheroDevice != null)
            {
                _spheroDevice.Roll(e.Angle, 0);
            }
        }

        private void spheroJoystick_CalibrationReleased(object sender, Sphero.Controls.JoystickCalibrationEventArgs e)
        {
            if (_spheroDevice != null)
            {
                _spheroDevice.SetHeading(0);
            }
        }

        private void spheroJoystick_moving(object sender, Sphero.Controls.JoystickMoveEventArgs e)
        {
            if (_spheroDevice != null)
            {
                _spheroDevice.Roll(e.Angle, e.Speed);
            }
        }

        private void spheroJoystick_Released(object sender, Sphero.Controls.JoystickMoveEventArgs e)
        {
            if (_spheroDevice != null)
            {
                _spheroDevice.Roll(e.Angle, 0);
            }
        }

        private void spheroJoystick_Loaded(object sender, RoutedEventArgs e)
        {

        }


    }
}