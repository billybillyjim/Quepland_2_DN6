namespace Quepland_2_DN6.Spells
{
    public class TemporalLeap : ISpell
    {
        public string Name { get; set; } = "Temporal Leap";
        public string Description { get; set; }
        public int Power { get; set; } = 1;
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "None";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public TemporalLeap() { }
        

        public void Cast()
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            foreach (Building b in AreaManager.Instance.Buildings)
            {
                if (b.TanningSlots.Count > 0)
                {
                    foreach (TanningSlot slot in b.TanningSlots)
                    {
                        if(slot.FinishTime.Year > 1999)
                        {
                            try
                            {
                                var newTime = slot.FinishTime.AddHours(-6);
                                slot.FinishTime = newTime;
                            }
                            catch
                            {

                            }
                        }
                        
                    }
                }
            }
            foreach (Dojo d in AreaManager.Instance.Dojos)
            {
                try
                {
                    d.LastWinTime = d.LastWinTime?.AddHours(-6);
                    d.LastLossTime = d.LastLossTime?.AddHours(-6);
                }
                catch
                {

                }
                
            }
            foreach(Area a in AreaManager.Instance.Areas)
            {
                if(a.TrapSlot != null)
                {
                    try
                    {
                        a.TrapSlot.HarvestTime = a.TrapSlot.HarvestTime.AddHours(-6);
                    }
                    catch
                    {

                    }
                }
            }
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 600);
        }
        public ISpell Copy()
        {
            return new TemporalLeap() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
