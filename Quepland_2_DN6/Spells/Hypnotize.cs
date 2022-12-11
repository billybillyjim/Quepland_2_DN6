namespace Quepland_2_DN6.Spells
{
    public class Hypnotize : ISpell
    {
        public string Name { get; set; } = "Hypnotize";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Hypnotize() { }
        

        public void Cast(Monster m)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            m.AddStatusEffect(new HypnotizeEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = Power, Speed = 5}));
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(m.Name + " grows drowsy, it's doing everything it can to just stay awake!");
            Player.Instance.GainExperience("Magic", 50);
        }

        public ISpell Copy()
        {
            return new Hypnotize() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
