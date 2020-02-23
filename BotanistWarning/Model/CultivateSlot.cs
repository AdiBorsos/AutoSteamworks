using System;

namespace BotanistWarning.Model
{
    public class CultivateSlotArgs : EventArgs
    {
        public int ID;
        public int SlotNumber;
        public string Name;

        public CultivateSlotArgs(CultivateSlot m)
        {
            this.ID = m.ItemId;
            this.Name = m.CultivateSlotName;
            this.SlotNumber = m.SlotNumber;
        }
    }

    public class CultivateSlot
    {
        private int _id;
        public int ItemId
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
                    _onCultivationItemIdChange();
                }
            }
        }

        public int SlotNumber { get; set; }
        public string CultivateSlotName { get; set; }

        public delegate void CultivateEvents(object source, CultivateSlotArgs args);
        public event CultivateEvents OnCultivationItemIdChange;

        protected virtual void _onCultivationItemIdChange()
        {
            CultivateSlotArgs args = new CultivateSlotArgs(this);
            OnCultivationItemIdChange?.Invoke(this, args);
        }
    }
}