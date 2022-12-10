namespace Quepland_2_DN6.Spells
{
    public class Rally : ISpell
    {
        public string Name { get; set; } = "Rally";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "Test";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Player";
        public int TimeRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Rally() { }
        

        public void Cast(Player p)
        {
            p.AddStatusEffect(new RallyEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = Power, Speed = 5}));
            MessageManager.AddMessage(Message);
        }


        public ISpell Copy()
        {
            return new Rally() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
