namespace Quepland_2_DN6.Spells
{
    public class TemporalLeap : ISpell
    {
        public string Name { get; set; } = "Temporal Leap";
        public string Description { get; set; }
        public int Power { get; set; } = 1;
        public string Message { get; set; } = "The world begins to blur into a fog. Reality flows past you at in incomprehensible speed. You stumble forward, a little bit older.";
        public int Duration { get; set; }
        public string Target { get; set; } = "None";
        public int TimeRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public TemporalLeap() { }
        

        public void Cast()
        {
            foreach (Building b in AreaManager.Instance.Buildings)
            {
                if (b.TanningSlots.Count > 0)
                {
                    foreach (TanningSlot slot in b.TanningSlots)
                    {
                        var newTime = slot.FinishTime.AddHours(-6);
                        slot.FinishTime = newTime;
                    }
                }
            }
            foreach (Dojo d in AreaManager.Instance.Dojos)
            {
                d.LastWinTime = d.LastWinTime?.AddHours(-6);
                d.LastLossTime = d.LastLossTime?.AddHours(-6);
            }
            foreach(Area a in AreaManager.Instance.Areas)
            {
                if(a.TrapSlot != null)
                {
                    a.TrapSlot.HarvestTime = a.TrapSlot.HarvestTime.AddHours(-6);
                }
            }
            MessageManager.AddMessage(Message);
        }
        public ISpell Copy()
        {
            return new TemporalLeap() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
