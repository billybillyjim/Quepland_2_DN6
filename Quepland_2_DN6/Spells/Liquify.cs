namespace Quepland_2_DN6.Spells
{
    public class Liquify : ISpell
    {
        public string Name { get; set; } = "Liquify";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "None";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Liquify() { }
        

        public void Cast()
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 120);
        }
        public ISpell Copy()
        {
            return new Liquify() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
