namespace Quepland_2_DN6.Spells
{
    public class Reflect : ISpell
    {
        public string Name { get; set; } = "Reflect";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Reflect() { }
        

        public void Cast(Monster m)
        {
            m.AddStatusEffect(new ReflectEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = Power, Speed = 5}));
            MessageManager.AddMessage(m.Name + " grows drowsy, it's doing everything it can to just stay awake!");
        }
        public void Cast(Player player)
        {
            GameState.AddActiveSpell(this.Copy(), Duration);
            MessageManager.AddMessage(Message);
        }

        public ISpell Copy()
        {
            return new Reflect() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
