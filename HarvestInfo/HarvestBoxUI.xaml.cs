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
            
            HookEvents();
            GetAnimations();
        }

        private void HookEvents()
        {
            Context.OnCounterChange += OnCounterChange;

            Context.Fertilizers[0].OnDurationChange += UpdateFertilizer;
            Context.Fertilizers[0].OnFertilizerChange += UpdateFertilizer;

            Context.Fertilizers[1].OnDurationChange += UpdateFertilizer;
            Context.Fertilizers[1].OnFertilizerChange += UpdateFertilizer;

            Context.Fertilizers[2].OnDurationChange += UpdateFertilizer;
            Context.Fertilizers[2].OnFertilizerChange += UpdateFertilizer;

            Context.Fertilizers[3].OnDurationChange += UpdateFertilizer;
            Context.Fertilizers[3].OnFertilizerChange += UpdateFertilizer;

            Context.Fertilizers[4].OnDurationChange += UpdateFertilizer;
            Context.Fertilizers[4].OnFertilizerChange += UpdateFertilizer;
        }

        public void UnhookEvents()
        {
            Context.OnCounterChange -= OnCounterChange;
            Context.Fertilizers[0].OnDurationChange -= UpdateFertilizer;
            Context.Fertilizers[0].OnFertilizerChange -= UpdateFertilizer;

            Context.Fertilizers[1].OnDurationChange -= UpdateFertilizer;
            Context.Fertilizers[1].OnFertilizerChange -= UpdateFertilizer;

            Context.Fertilizers[2].OnDurationChange -= UpdateFertilizer;
            Context.Fertilizers[2].OnFertilizerChange -= UpdateFertilizer;

            Context.Fertilizers[3].OnDurationChange -= UpdateFertilizer;
            Context.Fertilizers[3].OnFertilizerChange -= UpdateFertilizer;

            Context.Fertilizers[4].OnDurationChange -= UpdateFertilizer;
            Context.Fertilizers[4].OnFertilizerChange -= UpdateFertilizer;
        }

        private void UpdateFertilizer(object source, FertilizerEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Amount < Settings.ThresholdDuration)
            {
                ApplyAnimation = true;
            }

            Dispatch(() =>
            {
                Label lblFertName = null;
                Label lblFertCounter = null;
                
                foreach (var control in this.HarvestBoxComponent.Children)
                {
                    var lbl = control as FrameworkElement;
                    
                    if (lbl != null && lbl.Name == $"fert{args.Index}Name")
                    {
                        lblFertName = control as Label;
                    }

                    if (lbl != null && lbl.Name == $"fert{args.Index}Counter")
                    {
                        lblFertCounter = control as Label;
                    }
                }

                if (lblFertName != null && lblFertCounter != null)
                {
                    if (ApplyAnimation)
                    {
                        ANIM_FERTILIZER_EXPIRE.Begin(lblFertCounter, true);
                        ANIM_FERTILIZER_EXPIRE.Begin(lblFertName, true);
                    }
                    else
                    {
                        ANIM_FERTILIZER_EXPIRE.Remove(lblFertCounter);
                        ANIM_FERTILIZER_EXPIRE.Remove(lblFertName);
                    }

                    lblFertName.Content = args.Name;
                    lblFertCounter.Content = $"x{args.Amount}";
                }
            });
        }

        private void OnCounterChange(object source, HarvestBoxEventArgs args)
        {
            bool ApplyAnimation = false;
            if (args.Counter > Settings.ThresholdHarvest)
            {
                ApplyAnimation = true;
            }

            Dispatch(() =>
            {
                if (ApplyAnimation)
                {
                    ANIM_FERTILIZER_EXPIRE.Begin(HarvestBoxItemsCounter, true);
                }
                else
                {
                    ANIM_FERTILIZER_EXPIRE.Remove(HarvestBoxItemsCounter);
                }

                this.HarvestBoxItemsCounter.Content = $"{args.Counter}/{args.Max}";
            });
        }

        private void GetAnimations()
        {
            ANIM_FERTILIZER_EXPIRE = FindResource("FertilizerExpiring") as Storyboard;
        }

        private void Dispatch(Action function)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, function);
        }
    }
}
