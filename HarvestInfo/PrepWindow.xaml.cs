using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using HarvestInfo.Core.Utility;
using HarvestInfo.Model;
using Keystroke.API;

namespace HarvestInfo
{
    /// <summary>
    /// Interaction logic for PrepWindow.xaml
    /// </summary>
    public partial class PrepWindow : Window
    {
        private static Random rnd = new Random();
        private static KeystrokeAPI api = new KeystrokeAPI();

        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CancellationToken tkn => cts.Token;

        public delegate void KeyboardEventHandler(object source, EventArgs args);
        public event KeyboardEventHandler OnToggleEvent;
        public event KeyboardEventHandler OnExitEvent;

        public Storyboard ANIM_FERTILIZER_EXPIRE = null;

        public PrepWindow()
        {
            InitializeComponent();

            HookToKeyboardEvents();
            HookEvents();
            StartReading();
            ChangeHarvestBoxPosition();
         
            ANIM_FERTILIZER_EXPIRE = FindResource("FertilizerExpiring") as Storyboard;
        }

        public void ChangeHarvestBoxPosition()
        {
            double w_Height = Screen.PrimaryScreen.Bounds.Height;
            double w_Width = Screen.PrimaryScreen.Bounds.Width;

            double X = Settings.OverlaySettings.Position[0] % w_Width;
            double Y = Settings.OverlaySettings.Position[1] % w_Height;

            double Left = HarvestBoxComponent.Margin.Left;
            double Top = HarvestBoxComponent.Margin.Top;
            double Right = HarvestBoxComponent.Margin.Right;
            double Bottom = HarvestBoxComponent.Margin.Bottom;

            Dispatch(() =>
            {
                if (X != Left || Y != Top)
                {
                    HarvestBoxComponent.Margin = new Thickness(X, Y, Right, Bottom);
                }
            });
        }

        private void Dispatch(Action function)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, function);
        }

        private void HookEvents()
        {
            OnToggleEvent += PrepWindow_OnToggleEvent;
            OnExitEvent += PrepWindow_OnExitEvent;
            Game.OnFertilizerCountLow += On_ThresholdsExceded;
            Game.OnHarvestCountHigh += On_ThresholdsExceded;
        }

        private void PrepWindow_OnExitEvent(object source, EventArgs args)
        {
            Dispatch(() =>
            {
                cts.Cancel();

                api.Dispose();

                UnHookEvents();

                this.Close();
            });

        }

        private void PrepWindow_OnToggleEvent(object source, EventArgs args)
        {
            Dispatch(() =>
            {
                if (this.HarvestBoxComponent.Visibility == Visibility.Visible)
                {
                    this.HarvestBoxComponent.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.HarvestBoxComponent.Visibility = Visibility.Visible;

                    ANIM_FERTILIZER_EXPIRE.Remove(this.warning);

                    this.warning.Visibility = Visibility.Hidden;
                }
            });
        }

        private void UnHookEvents()
        {
            OnToggleEvent -= PrepWindow_OnToggleEvent;
            OnExitEvent -= PrepWindow_OnExitEvent;

            Game.OnFertilizerCountLow -= On_ThresholdsExceded;
            Game.OnHarvestCountHigh -= On_ThresholdsExceded;
        }

        private void On_ThresholdsExceded(object source, EventArgs args)
        {
            Dispatch(() =>
            {
                if (this.HarvestBoxComponent.Visibility != Visibility.Visible)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(this.warning, true);

                    this.warning.Visibility = Visibility.Visible;
                }
            });
        }

        private void StartReading()
        {
            Task.Run(() =>
            {
                while (!tkn.IsCancellationRequested)
                {
                    Game.LoadMemData();

                    Thread.Sleep(Settings.Delay);
                }
            },
            tkn);
        }

        private void HookToKeyboardEvents()
        {
            Task.Run(() =>
            {
                api.CreateKeyboardHook((character) =>
                {
                    if (character.KeyCode == (KeyCode)Settings.KeyCodeToggle)
                    {
                        Logger.LogInfo($"Captured Toggle!");

                        OnToggleEvent?.Invoke(this, null);
                    }

                    if (character.KeyCode == (KeyCode)Settings.KeyCodeExit)
                    {
                        Logger.LogInfo($"Quiting..");

                        OnExitEvent?.Invoke(this, null);

                        System.Windows.Forms.Application.Exit();
                    }
                });

                System.Windows.Forms.Application.Run();
            });
        }
    }
}
