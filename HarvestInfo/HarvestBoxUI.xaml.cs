using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using HarvestInfo.Model;

namespace HarvestInfo
{
    public partial class HarvestBoxUI : UserControl
    {
        public HarvestBox Context => Game.Harvest;

        // Animations
        Storyboard ANIM_FERTILIZER_EXPIRE;

        public HarvestBoxUI()
        {
            InitializeComponent();

            this.HarvestBoxComponent.Children.Add(new Label()
            {
                Name = "Random",
                Content = "Empty",
                Margin = new Thickness(0, 144, 42, 0)
            });

            SetContext();
        }

        public void FillPage ()
        {
            /*            
             <Label x:Name="fert1Name" 
             Content="Empty" 
             Margin="0,29,42,0" 
             Opacity="0.8" 
             Foreground="White" 
             Padding="5,0,0,0" 
             VerticalContentAlignment="Center" 
             FontFamily="Roboto" 
             Background="#66000000" 
             FontWeight="Light" 
             Height="29" 
             VerticalAlignment="Top"
             FontSize="14" TextOptions.TextHintingMode="Animated"/>
              */
        }

        public void SetContext()
        {
            HookEvents();
            GetAnimations();
        }

        private void Dispatch(Action function)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, function);
        }

        private void GetAnimations()
        {
            ANIM_FERTILIZER_EXPIRE = FindResource("FertilizerExpiring") as Storyboard;
        }

        private void HookEvents()
        {
            Context.OnCounterChange += OnCounterChange;

            Context.Fertilizers[0].OnDurationChange += UpdateFirstFertilizer;
            Context.Fertilizers[0].OnFertilizerChange += UpdateFirstFertilizer;

            Context.Fertilizers[1].OnDurationChange += UpdateSecondFertilizer;
            Context.Fertilizers[1].OnFertilizerChange += UpdateSecondFertilizer;

            Context.Fertilizers[2].OnDurationChange += UpdateThirdFertilizer;
            Context.Fertilizers[2].OnFertilizerChange += UpdateThirdFertilizer;

            Context.Fertilizers[3].OnDurationChange += UpdateFourthFertilizer;
            Context.Fertilizers[3].OnFertilizerChange += UpdateFourthFertilizer;

            Context.Fertilizers[4].OnDurationChange += UpdateFourthFertilizer;
            Context.Fertilizers[4].OnFertilizerChange += UpdateFourthFertilizer;
        }

        public void UnhookEvents()
        {
            Context.OnCounterChange -= OnCounterChange;
            Context.Fertilizers[0].OnDurationChange -= UpdateFirstFertilizer;
            Context.Fertilizers[0].OnFertilizerChange -= UpdateFirstFertilizer;

            Context.Fertilizers[1].OnDurationChange -= UpdateSecondFertilizer;
            Context.Fertilizers[1].OnFertilizerChange -= UpdateSecondFertilizer;

            Context.Fertilizers[2].OnDurationChange -= UpdateThirdFertilizer;
            Context.Fertilizers[2].OnFertilizerChange -= UpdateThirdFertilizer;

            Context.Fertilizers[3].OnDurationChange -= UpdateFourthFertilizer;
            Context.Fertilizers[3].OnFertilizerChange -= UpdateFourthFertilizer;
        }

        private void UpdateFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount <= Settings.ThresholdDuration)
            {
                ApplyAnimation = true;
            }

            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(fert1Counter, true);
                    ANIM_FERTILIZER_EXPIRE.Begin(fert1Name, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(fert1Counter);
                    ANIM_FERTILIZER_EXPIRE.Remove(fert1Name);
                }

                //foreach (var control in this.HarvestBoxComponent.Children)
                //{
                //    var lbl = control as Label;
                //    if (lbl != null && lbl.na)
                //    {

                //    }
                //}
                

                this.fert1Name.Content = args.Name;
                this.fert1Counter.Content = $"x{args.Amount}";
            });
        }

        private void UpdateFirstFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount <= Settings.ThresholdDuration) ApplyAnimation = true;
            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(fert1Counter, true);
                    ANIM_FERTILIZER_EXPIRE.Begin(fert1Name, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(fert1Counter);
                    ANIM_FERTILIZER_EXPIRE.Remove(fert1Name);
                }
                this.fert1Name.Content = args.Name;
                this.fert1Counter.Content = $"x{args.Amount}";
            });
        }

        private void UpdateSecondFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount <= Settings.ThresholdDuration) ApplyAnimation = true;
            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(fert2Counter, true);
                    ANIM_FERTILIZER_EXPIRE.Begin(fert2Name, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(fert2Counter);
                    ANIM_FERTILIZER_EXPIRE.Remove(fert2Name);
                }
                this.fert2Name.Content = args.Name;
                this.fert2Counter.Content = $"x{args.Amount}";
            });
        }

        private void UpdateThirdFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount <= Settings.ThresholdDuration) ApplyAnimation = true;
            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(fert3Counter, true);
                    ANIM_FERTILIZER_EXPIRE.Begin(fert3Name, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(fert3Counter);
                    ANIM_FERTILIZER_EXPIRE.Remove(fert3Name);
                }
                this.fert3Name.Content = args.Name;
                this.fert3Counter.Content = $"x{args.Amount}";
            });
        }

        private void UpdateFourthFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount <= Settings.ThresholdDuration) ApplyAnimation = true;
            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(fert4Counter, true);
                    ANIM_FERTILIZER_EXPIRE.Begin(fert4Name, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(fert4Counter);
                    ANIM_FERTILIZER_EXPIRE.Remove(fert4Name);
                }
                this.fert4Name.Content = args.Name;
                this.fert4Counter.Content = $"x{args.Amount}";
            });
        }

        private void OnCounterChange(object source, HarvestBoxEventArgs args)
        {
            Dispatch(() =>
            {
                this.HarvestBoxItemsCounter.Content = $"{args.Counter}/{args.Max}";
            });
        }
    }
}
