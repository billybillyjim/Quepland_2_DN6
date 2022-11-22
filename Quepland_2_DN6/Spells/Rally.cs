namespace Quepland_2_DN6.Spells
{
    public class Rally : ISpell
    {
        public string Name { get; set; } = "Hypnotize";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Rally() { }
        

        public void Cast(Monster m)
        {
            m.AddStatusEffect(new HypnotizeEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = Power, Speed = 5}));
            MessageManager.AddMessage(m.Name + " grows drowsy, it's doing everything it can to just stay awake!");
        }

        public ISpell Copy()
        {
            return new Rally() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
