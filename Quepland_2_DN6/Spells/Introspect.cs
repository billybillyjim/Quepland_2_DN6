namespace Quepland_2_DN6.Spells
{
    public class Introspect : ISpell
    {
        public string Name { get; set; } = "Introspect";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Introspect() { }
        

        public void Cast()
        {


            MessageManager.AddMessage(Message);
        }
        public ISpell Copy()
        {
            return new Introspect() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
