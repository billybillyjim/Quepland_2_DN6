namespace Quepland_2_DN6.Spells
{
    public class Entangle : ISpell
    {
        public string Name { get; set; } = "Entangle";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Entangle() { }
        

        public void Cast(Monster m)
        {
            m.AddStatusEffect(new EntangleEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = Power, Speed = 5}));
            MessageManager.AddMessage(m.Name + "is entangle by a magical vine!");
        } 

        public ISpell Copy()
        {
            return new Entangle() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
