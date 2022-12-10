namespace Quepland_2_DN6.Spells
{
    public class Drain : ISpell
    {
        public string Name { get; set; } = "Drain";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false; 

        public Drain() { }
        

        public void Cast(Monster m)
        {
            var dmg = Power * Player.Instance.GetLevel("Magic");
            m.AddStatusEffect(new DrainEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = dmg, Speed = 5}));
            MessageManager.AddMessage("You sap the " + m.Name + " of its energy!");
            Player.Instance.GainExperience("Magic", dmg);
        }

        public ISpell Copy()
        {
            return new Drain() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
