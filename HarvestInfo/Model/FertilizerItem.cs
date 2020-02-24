using System;
using System.Collections.Generic;

namespace HarvestInfo.Model
{
    public class FertilizerEventArgs : EventArgs
    {
        public int Index;
        public int ID;
        public string Name;
        public int Amount;

        public FertilizerEventArgs(FertilizerItem m)
        {
            this.Index = m.Index;
            this.ID = m.FertilizerId;
            this.Name = m.FertilizerName;
            this.Amount = m.Duration;
        }
    }

    public class FertilizerItem
    {
        private static Dictionary<int, string> _strings = new Dictionary<int, string>
        {
           { 0, "Empty" },
           { 1, "Plant Harvest Up (S)" },
           { 2, "Plant Harvest Up (L)" },
           { 3, "Fungi Harvest Up (S)" },
           { 4, "Fungi Harvest Up (L)" },
           { 5, "Bug/Honey Harvest Up (S)" },
           { 6, "Bug/Honey Harvest Up (L)" },
           { 7, "Growth Up (S)" },
           { 8, "Growth Up (L)" },
        };
        public int Index { get; set; }

        private int _id;
        public int FertilizerId 
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    _onFertilizerChange();
                }
            }
        }

        private int _duration;
        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    _onDurationUpdate();
                }
            }
        }
        public string FertilizerName => _strings[FertilizerId];

        // Fertilizer Events
        public delegate void FertilizerEvents(object source, FertilizerEventArgs args);
        public event FertilizerEvents OnFertilizerChange;
        public event FertilizerEvents OnDurationChange;

        protected virtual void _onFertilizerChange()
        {
            FertilizerEventArgs args = new FertilizerEventArgs(this);
            OnFertilizerChange?.Invoke(this, args);
        }

        protected virtual void _onDurationUpdate()
        {
            FertilizerEventArgs args = new FertilizerEventArgs(this);
            OnDurationChange?.Invoke(this, args);
        }
    }
}