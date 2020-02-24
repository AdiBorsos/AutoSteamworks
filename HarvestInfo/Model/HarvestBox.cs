using System;
using System.Collections.Generic;

namespace HarvestInfo.Model
{
    public class HarvestBoxEventArgs : EventArgs
    {
        public int Counter;
        public int Max;

        public HarvestBoxEventArgs(HarvestBox m)
        {
            this.Counter = m.Counter;
            this.Max = m.Max;
        }

    }

    public class HarvestBox
    {
        private int _counter;

        public readonly int Max = 40;

        public readonly List<FertilizerItem> Fertilizers = new List<FertilizerItem>();
        public readonly List<CultivateSlot> CultivateSlots = new List<CultivateSlot>();

        public int Counter
        {
            get { return _counter; }
            set
            {
                if (_counter != value)
                {
                    _counter = value;
                    _onCounterChange();
                }
            }
        }

        public HarvestBox()
        {
            PopulateBoxes();
        }

        // Harvest Box Events
        public delegate void HarvestBoxEvents(object source, HarvestBoxEventArgs args);
        public event HarvestBoxEvents OnCounterChange;

        protected virtual void _onCounterChange()
        {
            HarvestBoxEventArgs args = new HarvestBoxEventArgs(this);
            OnCounterChange?.Invoke(this, args);
        }

        private void PopulateBoxes()
        {
            for (int i = 0; i < 5; i++)
            {
                Fertilizers.Add(new FertilizerItem
                {
                    Index = i,
                    FertilizerId = 0,
                    Duration = 0
                });
            }

            for (int i = 0; i < 4; i++)
            {
                CultivateSlots.Add(new CultivateSlot
                {
                    Index = i,
                    ItemId = 0,
                    SlotNumber = i
                });
            }
        }
    }
}